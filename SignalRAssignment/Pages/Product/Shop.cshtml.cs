using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Product
{
    [AllowAnonymous]
    public class ShopModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public PaginatedList<Models.Product> Products { get; set; } = default!;
        public async Task<IActionResult> OnGet(int pageIndex = 1)
        {
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

    }
}
