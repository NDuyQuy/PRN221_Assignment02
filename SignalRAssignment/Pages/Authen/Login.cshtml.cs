using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SignalRAssignment.DataContext;
using SignalRAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SignalRAssignment.Pages.Authen
{
    [AllowAnonymous]
    public class LoginModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;

        [BindProperty]
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserName == Username && a.HashPassword == Password);
                if (account == null)
                {
                    ErrorMessage = "Username or password invalid";
                    return Page();
                }
                else
                {
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name,account.UserName),
                        new(ClaimTypes.Role,account.Type==1?"Admin":"User")
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.Session.SetInt32("UserID",account.AccountId);
                    HttpContext.Session.SetString("IsLogin","True");
                    HttpContext.Session.SetString("IsAdmin",account.Type==1?"True":"False");
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToPage("/Index");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
