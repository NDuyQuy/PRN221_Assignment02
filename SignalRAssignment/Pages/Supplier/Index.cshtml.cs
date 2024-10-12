using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Supplier
{
    [AllowAnonymous]
    public class IndexModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;

        public PaginatedList<Models.Supplier> Suppliers { get; set; } = default!;
        [BindProperty]
        public Models.Supplier Supplier { get; set; } = default!;
        public async Task OnGetAsync(int pageIndex = 1)
        {
            var pageSize = 10; // Define the number of items per page
            IQueryable<Models.Supplier> suppliersIQ = (from s in _context.Suppliers select s);
            Suppliers = await PaginatedList<Models.Supplier>.CreateAsync(suppliersIQ, pageIndex, pageSize);
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

        public async Task<IActionResult> OnPostDeleteAsync(int supplierID)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s=>s.SupplierID == supplierID);
            if (supplier == null) return NotFound();
            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
