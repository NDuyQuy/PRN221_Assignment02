using CoffeeShop.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Product
{
    public class CartModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public CartModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public class Cart
        {
            public bool IsSelected { get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public Models.Product? Product { get; set; } = default!;
        }

        [BindProperty]
        public List<Cart> CartList { get; set; } = default!;

        public void OnGet(int pageIndex = 1)
        {
            Dictionary<int, int>? cartMap;
            var jsonCart = HttpContext.Session.GetString("cart");
            cartMap = string.IsNullOrEmpty(jsonCart) ? new Dictionary<int, int>() : JsonHelper.DeserializeObject<Dictionary<int, int>>(jsonCart);
            cartMap ??= new Dictionary<int, int>();

            CartList = cartMap
                .Select(kvp => new Cart
                {
                    ProductID = kvp.Key,
                    Quantity = kvp.Value,
                    Product = _context.Products.Find(kvp.Key),
                    TotalPrice = _context.Products.Find(kvp.Key).Price * kvp.Value,
                })
                .ToList();
        }

        public async Task<IActionResult> OnPostAsync(string selectedProducts)
        {
            var userID = HttpContext.Session.GetInt32("UserID");
            if (!userID.HasValue) return RedirectToPage("/Authen/Login");
            var user = await _context.Accounts.FindAsync(userID.Value);
            if (user == null) return NotFound();
            var products = JsonHelper.DeserializeObject<List<ProductOrder>>(selectedProducts);
            List<OrderDetail> odList = [];
            Models.Order order = new();
            if (products != null && products.Count > 0)
            {
                order.AccountID = user.AccountId;
                order.OrderDate = DateTime.Now;
                order.ShipAddress = user?.Address??"";
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                foreach (var product in products)
                {
                    var existingProduct = await _context.Products.FindAsync(product.Id);
                    if (existingProduct != null)
                    {
                        odList.Add(
                            new OrderDetail
                            {
                                OrderID = order.OrderID,
                                ProductID = product.Id,
                                Quantity = product.Quantity,
                                UnitPrice = existingProduct.Price
                            });

                    }
                }
            }
            await _context.OrderDetails.AddRangeAsync(odList);
            await _context.SaveChangesAsync();
            if (userID.HasValue)
            {
                return RedirectToPage("/Order/Index", new { accountId = userID.Value });
            }

            return RedirectToPage();
        }

        public class ProductOrder
        {
            public int Id { get; set; }
            public int Quantity { get; set; }
        }
    }
}
