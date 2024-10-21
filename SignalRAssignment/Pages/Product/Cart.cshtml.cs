using CoffeeShop.Helper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;

namespace SignalRAssignment.Pages.Product
{
    public class CartModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        public class Cart
        {
            public bool IsSelected {  get; set; }
            public int ProductID { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
            public Models.Product? Product { get; set; } = default!;
        }

        [BindProperty]
        public List<Cart> CartList { get; set; } = default!;
        public void OnGet(int pageIndex = 1)
        {
            Dictionary<int, int>? cartMap;
            var jsonCart = HttpContext.Session.GetString("cart");
            cartMap = string.IsNullOrEmpty(jsonCart) ? [] : JsonHelper.DeserializeObject<Dictionary<int, int>>(jsonCart);
            cartMap ??= [];
            CartList = cartMap
                .Select(kvp => new Cart { ProductID = kvp.Key, 
                    Quantity = kvp.Value,
                    Product = _context.Products.Find(kvp.Key),
                    TotalPrice = _context.Products.Find(kvp.Key).Price*kvp.Value,
                })
                .ToList();
        }

        public async Task<IActionResult>OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if(userId.HasValue)
                return RedirectToPage("/Order/Index", new { accountId = userId.Value });
            return RedirectToPage("/Authen/Login");
        }
    }
}
