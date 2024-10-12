using CoffeeShop.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Product
{
    public class IndexModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public PaginatedList<Models.Product> Products { get; set; } = default!;
        public async Task OnGet(int pageIndex = 1)
        {
            var productIQ = (from prod in _context.Products 
                             .Include(p=>p.Category)
                             .Include(p=>p.Supplier)
                             select prod);
            Products = await  PaginatedList<Models.Product>.CreateAsync(productIQ, pageIndex, 10);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int productID)
        {
            var prod = await _context.Products.FindAsync(productID);
            _context.Products.Remove(prod);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
