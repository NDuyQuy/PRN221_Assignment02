using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Category
{
    [AllowAnonymous]
    public class IndexModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public PaginatedList<Models.Category> Categories { get; set; } = default!;
        [BindProperty]
        public Models.Category Category { get; set; } = default!;
        public async Task OnGetAsync(int? pageIndex)
        {
            int pageSize = 10;
            IQueryable<Models.Category> categoryIQ = (from c in _context.Categories select c);
            Categories = await PaginatedList<Models.Category>.CreateAsync(categoryIQ, pageIndex ?? 1, pageSize);
        }
        public async Task<IActionResult> OnPostAddAsync()
        {
            // Logic for adding a category
            if (ModelState.IsValid)
            {
                await _context.Categories.AddAsync(Category);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(Category);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int categoryID)
        {
            var category = await _context.Categories.FindAsync(categoryID);
            if (category == null) return RedirectToPage();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // Logic for deleting a category
            return RedirectToPage();
        }
    }
}
