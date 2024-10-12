using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Account
{
    [Authorize(Roles ="Admin")]
    public class IndexModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public PaginatedList<Models.Account> Accounts { get; set; } = default!;

        [BindProperty]
        public Models.Account Account { get; set; } = default!;
        public async Task OnGetAsync(int pageIndex = 1)
        {
            int pageSize = 10;
            IQueryable<Models.Account> accountsIQ = (from a in _context.Accounts select a);
            //Accounts = new PaginatedList<Models.Account>(await accountsIQ.ToListAsync(),await accountsIQ.CountAsync(),pageIndex,pageSize);
            Accounts = await PaginatedList<Models.Account>.CreateAsync(accountsIQ, pageIndex, pageSize);
        }
        public async Task<IActionResult> OnPostAddAsync()
        {
            if(ModelState.IsValid)
            {
                await _context.AddAsync(Account);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostEditAsync()
        {
            if(ModelState.IsValid)
            {
                _context.Accounts.Update(Account);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int accountID)
        {
            var account = await _context.Accounts.FindAsync(accountID);
            if(account == null) return NotFound();
            else
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}
