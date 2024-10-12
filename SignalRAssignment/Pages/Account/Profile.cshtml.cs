using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Account
{
    public class ProfileModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        [BindProperty]
        public Models.Account? Account { get; set; } = default!;
        public async Task<IActionResult> OnGet(bool isEditing = false)
        {
            var id = HttpContext.Session.GetInt32("UserID");
            Account = await _context.Accounts.FirstOrDefaultAsync(c=>c.AccountId==id);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(ModelState.IsValid)
            {
                _context.Accounts.Update(Account);
                await _context.SaveChangesAsync();
            }
            return Page();
        }
    }
}
