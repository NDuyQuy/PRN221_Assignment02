using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;
using SignalRAssignment.Models;

namespace SignalRAssignment.Pages.Order
{
    public class DetailsModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public Models.Order? Order { get; set; } = default!;
        public List<OrderDetail> OrderDetails { get; set; } = [];
        public async Task<IActionResult> OnGet(int id)
        {
            Order = await _context.Orders.Include(o=>o.Account)
                .FirstOrDefaultAsync(o=>o.OrderID == id);
            if (OrderDetails == null) return NotFound();

            OrderDetails = await _context.OrderDetails
                .Where(od => od.OrderID == id)
                .Include(od => od.Product)
                .ToListAsync();
            return Page();
        }
    }
}
