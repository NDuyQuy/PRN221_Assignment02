using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;
using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment.Pages.Authen
{
    [BindProperties]
    [AllowAnonymous]
    public class RegisterModel(ApplicationDBContext context) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required")]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        [MaxLength(100)]
        [Required(ErrorMessage = "Username is required")]
        public string ConfirmPassword { get; set; } = string.Empty;


        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                Models.Account account = new()
                {
                    UserName = Username,
                    HashPassword = Password,
                    FullName = string.Empty
                };
                await _context.Accounts.AddAsync(account);
                await _context.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            else return Page();
        }
    }
}
