using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Supplier
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public IndexModel(ApplicationDBContext context)
        {
            _context = context;
        }

        public PaginatedList<Models.Supplier> Suppliers { get; set; } = default!;
        public Models.Supplier Supplier { get; set; } = default!;
        public async Task OnGetAsync(int pageIndex = 1)
        {
            var pageSize = 10; // Define the number of items per page
            var suppliers = _context.Suppliers.AsQueryable(); // Fetch all suppliers
            Suppliers = await PaginatedList<Models.Supplier>.CreateAsync(suppliers, pageIndex, pageSize);
        }
        public async Task<IActionResult> OnPostAddAsync()
        {
            // Logic for adding a Supplier
            if (ModelState.IsValid)
            {
                await _context.Suppliers.AddAsync(Supplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Update(Supplier);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var Supplier = await _context.Suppliers.FindAsync(id);
            if (Supplier == null) return RedirectToPage();
            _context.Suppliers.Remove(Supplier);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
