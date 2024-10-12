using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Order
{
    public class UpdateModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        [BindProperty]public Models.Order? Order { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Order = await _context.Orders.FindAsync(id);
            if(Order == null) return NotFound();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                _context.Orders.Update(Order);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            return RedirectToPage();
        }

    }
}
