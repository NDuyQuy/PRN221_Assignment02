using CoffeeShop.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;
using System.Security.Claims;

namespace SignalRAssignment.Pages.Order
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDBContext _context;

        public IndexModel(ApplicationDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public bool IsAdmin { get; set; } = false;

        public PaginatedList<Models.Order> OrderList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1, int? accountId = null)
        {
            // Check if user is an admin
            bool isAdmin = User.IsInRole("Admin");
            IsAdmin = isAdmin;

            if (!isAdmin && !accountId.HasValue)
            {
                // Non-admin users must provide an account ID
                return Forbid();
            }

            IQueryable<Models.Order> ordersIQ = _context.Orders.Include(o => o.Account);

            // Filter by account ID if provided and user is not admin
            if (!isAdmin && accountId.HasValue)
            {
                // Ensure the logged-in user can only access their own orders
                var UserID = HttpContext.Session.GetInt32("UserID");
                if (UserID != accountId)
                    return Forbid();
                // Filter by the provided AccountID
                ordersIQ = ordersIQ.Where(o => o.AccountID == accountId.Value);
            }

            // Apply pagination
            OrderList = await PaginatedList<Models.Order>.CreateAsync(ordersIQ, pageIndex, 10);
            return Page();
        }
    }
}
