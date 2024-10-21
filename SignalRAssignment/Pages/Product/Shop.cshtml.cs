using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Model.Map;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;
using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment.Pages.Product
{
    [AllowAnonymous]
    public class ShopModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public PaginatedList<Models.Product> Products { get; set; } = default!;
        public Models.Account? CurrentUser
        {
            get
            {
                var userID = HttpContext.Session.GetInt32("UserID");
                if (userID == null) return null;
                else
                {
                    return _context.Accounts.FindAsync(userID).Result;
                }
            }
        }

        [BindProperty]
        public Models.Order Order { get; set; } = default!;
        public Filter CurrentFilter { get; set; } = Filter.BestSelling;

        [BindProperty]
        [Required]
        [MinLength(3, ErrorMessage = "Name to search at least has 3 character")]
        public string NameSearch { get; set; } = string.Empty;
        public async Task<IActionResult> OnGet(int pageIndex = 1)
        {
            CurrentFilter = Filter.BestSelling;
            Products = default!;
            // Keep the query as IQueryable without AsEnumerable()
            var productsQuery = _context.Products
            .Join(_context.OrderDetails,
                    product => product.ProductID,
                    orderDetail => orderDetail.ProductID,
                    (product, orderDetail) => new { product, orderDetail.Quantity })
            .GroupBy(p => new
            {
                p.product.ProductID,
                p.product.ProductName,
                p.product.ProductImage,
                p.product.Price,
                p.product.Category.CategoryName // Include Category Name
            })
            .Select(group => new
            {
                Product = group.Key,
                TotalSold = group.Sum(g => g.Quantity)
            })
            .OrderByDescending(p => p.TotalSold)
            .Select(p => new Models.Product
            {
                ProductID = p.Product.ProductID,
                ProductName = p.Product.ProductName,
                ProductImage = p.Product.ProductImage,
                Price = p.Product.Price,
                Category = new Models.Category { CategoryName = p.Product.CategoryName } // Include Category
            });

            // Fetch the list asynchronously

            // Apply pagination after fetching the products
            Products = await PaginatedList<Models.Product>.CreateAsync(productsQuery, pageIndex, 3);

            return Page();
        }

        public async Task<IActionResult> OnGetCategoryAsync(int categoryID, int pageIndex = 1)
        {
            Products = default!;//reset products
            CurrentFilter = Filter.Category;
            var prodsIQ = (from prod in _context.Products
                       .Include(p => p.Category)
                           where prod.CategoryID == categoryID
                           select prod);
            Products = await PaginatedList<Models.Product>.CreateAsync(prodsIQ, pageIndex, 3);

            return Page();
        }

        public async Task<IActionResult> OnGetAllAsync(int pageIndex)
        {
            CurrentFilter = Filter.All;
            var prodsIQ = (from prod in _context.Products
                           .Include(p => p.Category)
                           select prod);
            Products = await PaginatedList<Models.Product>.CreateAsync(prodsIQ, pageIndex, 3);
            return Page();
        }
        public async Task<IActionResult> OnPostSearchAsync(int pageIndex)
        {
            ModelState.Remove(nameof(Order));
            ModelState.Remove("ShipAddress");
            if (ModelState.IsValid)
            {
                CurrentFilter = Filter.SearchResult;
                var prodIQ = from prod in _context.Products.Include(p => p.Category)
                             where prod.ProductName.Contains(NameSearch)
                             select prod;
                Products = await PaginatedList<Models.Product>.CreateAsync(prodIQ, pageIndex, 3);
            }
            return Page();
        }

        public IActionResult OnPostAddToCart(int productID, int quantity)
        {
            if (!User.Identity.IsAuthenticated)
                return new JsonResult
                    (new { success = false, message = "To use this feature you must have log in first!" });
            Dictionary<int, int>? cartMap;
            var jsonCart = HttpContext.Session.GetString("cart");
            cartMap = string.IsNullOrEmpty(jsonCart) ? [] : JsonHelper.DeserializeObject<Dictionary<int, int>>(jsonCart);
            cartMap ??= [];
            if (cartMap.ContainsKey(productID))
                cartMap[productID] += quantity;
            else cartMap.Add(productID, quantity);
            HttpContext.Session.SetString("cart", JsonHelper.SerializeObject(cartMap));
            return new JsonResult(new { success = true, message = "Add to cart successfully" });
        }

        public async Task<IActionResult> OnPostOrder(int productID, int quantity)
        {
            ModelState.Remove(nameof(NameSearch));
            if (ModelState.IsValid)
            {
                _context.Orders.Add(Order);
                await _context.SaveChangesAsync();
                var product = await _context.Products.FindAsync(productID);
                if (product == null)
                {
                    return RedirectToPage();
                }
                var orderDetail = new Models.OrderDetail()
                {
                    ProductID = productID,
                    Quantity = quantity,
                    OrderID = Order.OrderID,
                    UnitPrice = product.Price
                };
                await _context.OrderDetails.AddAsync(orderDetail);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }

    public enum Filter
    {
        BestSelling,
        Category,
        All,
        SearchResult
    }
}
