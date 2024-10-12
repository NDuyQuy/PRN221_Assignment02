using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SignalRAssignment.DataContext;
using Microsoft.AspNetCore.Mvc.Rendering;
using SignalRAssignment.Models;
using System.IO;
using System.ComponentModel.DataAnnotations;

namespace SignalRAssignment.Pages.Product
{
    public class AddUpdateModel(ApplicationDBContext context, IWebHostEnvironment environment) : PageModel
    {
        private readonly ApplicationDBContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;
        [BindProperty]
        public Models.Product? Product { get; set; } = default!;
        [BindProperty]

        [Required]
        public IFormFile ProductImage { get; set; }
        [BindProperty]
        public List<SelectListItem> SupplierList { get; set; }
        [BindProperty]
        public List<SelectListItem> CategoryList { get; set; }
        
        [BindProperty]
        public bool IsEdit { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id = null)
        {
            SupplierList = [.. _context.Suppliers
                .Select(s => new SelectListItem { Value = s.SupplierID.ToString(), Text = s.CompanyName })];
            CategoryList = [.. _context.Categories
                .Select(c => new SelectListItem { Value = c.CategoryID.ToString(), Text = c.CategoryName })];
            if (id != null)
            {
                IsEdit = true;
                Product = await _context.Products.FindAsync(id);
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {

            if (ProductImage != null)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "images", ProductImage.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    ProductImage.CopyTo(stream);
                }
                Product.ProductImage = "/images/" + ProductImage.FileName;
            }

            if (IsEdit)
            {

                _context.Products.Update(Product);
            }
            else
            {
                await _context.Products.AddAsync(Product);
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");

        }
    }
}
