using DataBaseAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModelClasses;
using ModelClasses.ViewModel;

namespace E_CommerceWebApp.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(x => x.Category).ToList();
            return View(products);
        }
        // Create Method To Open Create View
        public IActionResult Create()
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Inventories = new Inventory(),
                ProductImages = new ProductImages(),
                CategoriesList = _context.Categories.ToList().Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }),
            };
            return View(productViewModel);
        }

        // Create Method
        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            string homeImageUrl = "";
            if (productViewModel.Images != null)
            {
                foreach (var image in productViewModel.Images)
                {
                    homeImageUrl = image.FileName;
                    if (homeImageUrl.Contains("Home"))
                    {
                        homeImageUrl = UploadFiles(image);
                        break;
                    }
                }
            }

            productViewModel.Products!.HomeImgUrl = homeImageUrl;
            await _context.AddAsync(productViewModel.Products);
            await _context.SaveChangesAsync();

            var newProduct = await _context.Products.Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.ProductName == productViewModel.Products.ProductName);
            productViewModel.Inventories!.InventoryName = newProduct!.ProductName;
            productViewModel.Inventories!.Category = newProduct.Category!.CategoryName;
            await _context.Inventories.AddAsync(productViewModel.Inventories);
            await _context.SaveChangesAsync();

            if (productViewModel.Images != null)
            {
                foreach (var image in productViewModel.Images)
                {
                    string tempFileName = image.FileName;
                    if (!tempFileName.Contains("Home"))
                    {
                        string stringFileName = UploadFiles(image);
                        var addressImage = new ProductImages
                        {
                            ImgUrl = stringFileName,
                            ProductId = newProduct.Id,
                            ProductName = newProduct.ProductName,
                        };

                        await _context.ProductImages.AddAsync(addressImage);
                    }
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Product");
        }

        // Edit Method To Open Edit View
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Products = _context.Products.FirstOrDefault(x => x.Id == id),
                CategoriesList = _context.Categories.ToList().Select(x => new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }),
            };
            productViewModel.Products!.ImgUrls = _context.ProductImages
                .Where(x => x.ProductId == id).ToList();

            return View(productViewModel);
        }

        //Edit Product 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductViewModel productViewModel)
        {
            var productToEdit = _context.Products
                .FirstOrDefault(x => x.Id == productViewModel.Products!.Id);

            if (productToEdit != null)
            {
                productToEdit.ProductName = productViewModel.Products!.ProductName;
                productToEdit.Price = productViewModel.Products!.Price;
                productToEdit.Description = productViewModel.Products!.Description;
                productToEdit.CategoryId = productViewModel.Products!.CategoryId;
                if (productViewModel.Images != null)
                {
                    foreach (var item in productViewModel.Images)
                    {
                        string tempFileName = item.FileName;
                        if (!tempFileName.Contains("Home"))
                        {
                            string stringFileName = UploadFiles(item);
                            var addressImage = new ProductImages
                            {
                                ImgUrl = stringFileName,
                                ProductId = productViewModel.Products!.Id,
                                ProductName = productViewModel.Products!.ProductName,
                            };
                            _context.ProductImages.Add(addressImage);
                        }
                        else
                        {
                            if (productToEdit.HomeImgUrl == "")
                            {
                                string homeImageUrl = item.FileName;
                                if (homeImageUrl.Contains("Home"))
                                {
                                    homeImageUrl = UploadFiles(item);
                                    productToEdit.HomeImgUrl = homeImageUrl;
                                }
                            }
                        }
                    }

                    _context.Products.Update(productToEdit);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Product");
        }

        // Delete Method 
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if(id != 0)
            {
                var productToDelete = _context.Products.FirstOrDefault(x => x.Id == id);

                var imagesToDelete = _context.ProductImages
                    .Where(x => x.ProductId == id)
                    .Select(x => x.ImgUrl);

                foreach (var image in imagesToDelete)
                {
                    string imageUrl = "Images\\" + image;
                    var toDeleteImageFromFolder =
                        Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                    DeleteImages(toDeleteImageFromFolder);
                }

                if (productToDelete!.HomeImgUrl != "")
                {
                    string imageUrl = "Images\\" + productToDelete.HomeImgUrl;
                    var toDeleteImageFromFolder =
                        Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('\\'));
                    DeleteImages(toDeleteImageFromFolder);
                }

                _context.Products.Remove(productToDelete);
                _context.SaveChanges();
            }
            else
            {
                return Json(new { success = false, message = "Failed To Delete the item" });
            }

            return Json(new { success = true, message = "Delete successfully" });

            //return RedirectToAction("Index", "Product");
        }

        // to delete product image or update
        public IActionResult DeleteProductImage(string id)
        {
            int routeId = 0;
            if (id != null)
            {
                if (!id.Contains("Home"))
                {
                    var ImageToDeleteProductImage = _context.ProductImages
                        .FirstOrDefault(x => x.ImgUrl == id);
                    if (ImageToDeleteProductImage != null)
                    {
                        routeId = ImageToDeleteProductImage.ProductId;
                        _context.ProductImages.Remove(ImageToDeleteProductImage);
                    }
                }
                else
                {
                    var ImageToDeleteProductImage = _context.Products
                        .FirstOrDefault(x => x.HomeImgUrl == id);
                    if (ImageToDeleteProductImage != null)
                    {
                        ImageToDeleteProductImage.HomeImgUrl = "";
                        routeId = ImageToDeleteProductImage.Id;
                        _context.Products.Update(ImageToDeleteProductImage);
                    }
                }
                string imageUrl = "Images\\" + id;
                var toDeleteImageFromFolder = Path.Combine(_hostEnvironment.WebRootPath, imageUrl);
                DeleteImages(toDeleteImageFromFolder);
                _context.SaveChanges();
                return Json(new { success = true, message = "Picture was Deleted successfully", id = routeId});
            }

            return Json(new { success = false, message = "Failed to Delete the picture" });
        }

        // to delete images
        private void DeleteImages(string toDeleteImageFromFolder)
        {
            if (System.IO.File.Exists(toDeleteImageFromFolder))
            {
                System.IO.File.Delete(toDeleteImageFromFolder);
            }
        }

        // to save image at Images Folder and return url 
        private string UploadFiles(IFormFile image)
        {
            string fileName = null!;
            if (image != null)
            {
                string uploadDirLocation = Path.Combine(_hostEnvironment.WebRootPath, "Images");
                fileName = image.FileName;
                string filePath = Path.Combine(uploadDirLocation, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return fileName;
        }

    }
}