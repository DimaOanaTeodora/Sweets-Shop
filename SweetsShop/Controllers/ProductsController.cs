using Microsoft.AspNet.Identity;
using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{ 
    //[Authorize]
    public class ProductsController : Controller
    {
        private int _perPage = 3;
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Product
        //[Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Index(string search)
        {
            //var products = db.Products.Include("Category").Include("User");
            ViewBag.butoaneSortari = true;
            //ViewBag.Products = products;
            if (search != null)
            {
                ViewBag.butoaneSortari = false;
                List<String> searchItems = new List<string>(search.Split(" .,?!()[]{};:".ToCharArray()));

                searchItems = searchItems.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                var products = db.Products;
                List<Product> selectedProducts = new List<Product>();
                if (searchItems.Count() > 0)
                {
                    foreach (var product in products)
                    {
                        foreach (var item in searchItems.ToArray())
                        {
                            if (product.ProductName.Contains(item) || product.ProductDescription.Contains(item))
                            {
                                selectedProducts.Add(product);
                                break;
                            }
                        }
                    }
                }

                var totalItems = selectedProducts.Count();
                var currentPage = Convert.ToInt32(Request.Params.Get("page"));

                var offset = 0;

                if (!currentPage.Equals(0))
                {
                    offset = (currentPage - 1) * this._perPage;
                }

                var paginatedProducts = selectedProducts.Skip(offset).Take(this._perPage);

                ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
                ViewBag.Products = paginatedProducts ;
                ViewBag.SearchString = search;
            }
            else
            {
                ViewBag.Products = db.Products.Include("Category").Include("User") ;
            }
            SetAccessRights();

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.esteColaborator = User.IsInRole("Colaborator");
            

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
          
            
            return View();
        }

        // GET: Product
        //[Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult SortarePret(string search)
        {
            
            var products = db.Products.Include("Category").Include("User").OrderBy(a=> a.ProductPrice);
            ViewBag.Products = products;


            return View();
        }
        public ActionResult SortareRating(string search)
        {

            var products = db.Products.Include("Category").Include("User").OrderByDescending(a => a.ProductRating);
            ViewBag.Products = products;


            return View();
        }
        public ActionResult Requests()
        {
            var products = db.Products.Include("Category").Include("User");
            ViewBag.Products = products;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult ValidateRequest(int id)
        {
            Product product = db.Products.Find(id);
            product.Request = true;
            if (ModelState.IsValid)
            {
                db.SaveChanges();
                TempData["message"] = "Ati aprobat produsul cu succes";
                
            }
            return Redirect(Request.UrlReferrer.ToString());
        }


        // GET: Show
        //[Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Show(int id)
        {
            float rating = 0;
            Product product = db.Products.Find(id);
            var ratings = db.Comments.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
            if (ratings.Count() > 0)
            {
                var ratingSum = ratings.Sum(d => d.Rating.Value);
                ViewBag.RatingSum = ratingSum;
                var ratingCount = ratings.Count(d=> d.Rating.Value>0);
                ViewBag.RatingCount = ratingCount;
                 rating = (ratingSum / ratingCount);
                ViewBag.Rating = rating;
                product.ProductRating = rating;
                db.SaveChanges();
            }
            else
            {
                ViewBag.RatingSum = 0;
                ViewBag.RatingCount = 0;
            }
            SetAccessRights();
            
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Colaborator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.esteColaborator = User.IsInRole("Colaborator");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();

           
            return View(product);
        }
 
        [HttpPost]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Show(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    float rating = 0;
                    Product a = db.Products.Find(comm.ProductId);
                    var ratings = db.Comments.Where(d => d.ProductId.Equals(comm.ProductId)).ToList();
                    if (ratings.Count() > 0)
                    {
                        var ratingSum = ratings.Sum(d => d.Rating.Value);
                        ViewBag.RatingSum = ratingSum;
                        var ratingCount = ratings.Count();
                        ViewBag.RatingCount = ratingCount;
                        rating = (ratingSum / ratingCount);
                        ViewBag.Rating = rating;
                        a.ProductRating = rating;
                    }
                    else
                    {   
                        ViewBag.RatingSum = 0;
                        ViewBag.RatingCount = 0;
                    }
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Products/Show/" + comm.ProductId);
                   
                    
                }

                else
                {
                    Product a = db.Products.Find(comm.ProductId);
                    var ratings = db.Comments.Where(d => d.ProductId.Equals(comm.ProductId)).ToList();
                    if (ratings.Count() > 0)
                    {
                        var ratingSum = ratings.Sum(d => d.Rating.Value);
                        ViewBag.RatingSum = ratingSum;
                        var ratingCount = ratings.Count();
                        ViewBag.RatingCount = ratingCount;
                        var rating = (ratingSum / ratingCount);
                        a.ProductRating = rating;
                    }
                    else
                    {
                        ViewBag.RatingSum = 0;
                        ViewBag.RatingCount = 0;
                    }
                    SetAccessRights();
                    return View(a);
                }

            }

            catch (Exception e)
            {
                Product a = db.Products.Find(comm.ProductId);
                SetAccessRights();
                return View(a);
            }

        }
        

        // GET: New
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New()
        {
            Product product = new Product();

            // preluam lista de categorii din metoda GetAllCategories()
            product.Categ = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            product.UserId = User.Identity.GetUserId();
           
       
            return View(product);
        }

        // POST: New
        [HttpPost]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult New(Product product, HttpPostedFileBase uploadedFile)
        {
            product.Categ = GetAllCategories();
            //fara asta imi pune NULL in baza de date a Products la UserId
            product.UserId = User.Identity.GetUserId();

            try
            {
                if (ModelState.IsValid)
                {
                    string uploadedFileName = uploadedFile.FileName;
                    string uploadedFileExtension = Path.GetExtension(uploadedFileName);
                    if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg" || uploadedFileExtension == ".pdf")
                    {
                        string uploadFolderPath = Server.MapPath("~//Files//");
                        uploadedFile.SaveAs(uploadFolderPath + uploadedFileName);
                        product.Photo = "/Files/" + uploadedFileName;
                    }
                    if (User.IsInRole("Admin"))
                        product.Request = true;
                    if (User.IsInRole("Colaborator"))
                        product.Request = false;
                    db.Products.Add(product);
                    db.Products.Add(product);
                    db.SaveChanges();
                    if (User.IsInRole("Admin"))
                        TempData["message"] = "Produsul a fost adaugat";
                    if (User.IsInRole("Colaborator"))
                        TempData["message"] = "Cererea de adaugare a produsului a fost trimisa";
                    return RedirectToAction("Index");
                }
                else
                {
                    product.Categ = GetAllCategories();
                    return View(product);
                }
            }
            catch (Exception e)
            {
                return View(product);
            }
        }
        // GET: Edit
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id)
        {
            Product product = db.Products.Find(id);
            //ViewBag.Product = product;
            product.Categ = GetAllCategories();
            if (product.UserId == User.Identity.GetUserId() ||
            User.IsInRole("Admin"))
            {
                return View(product);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui produs care nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        // PUT: Edit
        [HttpPut]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Edit(int id, Product requestProduct, HttpPostedFileBase uploadedFile)
        {
           // requestProduct.Categ = GetAllCategories();
            try
            {
                if (ModelState.IsValid)
                {
                    Product product = db.Products.Find(id);

                    if (product.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(product))
                        {
                            product.ProductName = requestProduct.ProductName;
                            product.ProductDescription = requestProduct.ProductDescription;
                            product.ProductPrice = requestProduct.ProductPrice;
                            product.ProductStoc = requestProduct.ProductStoc;
                            product.CategoryId = requestProduct.CategoryId;

                            //pt imagine
                            string uploadedFileName = uploadedFile.FileName;
                            string uploadedFileExtension = Path.GetExtension(uploadedFileName);
                            if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg" || uploadedFileExtension == ".pdf")
                            {
                                string uploadFolderPath = Server.MapPath("~//Files//");
                                uploadedFile.SaveAs(uploadFolderPath + uploadedFileName);
                                product.Photo = "/Files/" + uploadedFileName;
                            }
                           

                            db.SaveChanges();
                            TempData["message"] = "Produsul a fost modificat";                          

                        }
                        return RedirectToAction("Index");
                    }


                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    requestProduct.Categ = GetAllCategories();
                    return View(requestProduct);
                }
            }
            catch (Exception e)
            {
                requestProduct.Categ = GetAllCategories();
                return View(requestProduct);
            }
        }

        // DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "Colaborator,Admin")]
        public ActionResult Delete(int id)
        {
            Product product = db.Products.Find(id);

            if (product.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete1(int id)
        {
            Product product = db.Products.Find(id);

            if (product.UserId == User.Identity.GetUserId() ||
                User.IsInRole("Admin"))
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["message"] = "Produsul a fost sters!";
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return RedirectToAction("Index");
            }

        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            /*
            foreach (var category in categories)
            {
                var listItem = new SelectListItem();
                listItem.Value = category.CategoryId.ToString();
                listItem.Text = category.CategoryName.ToString();

                selectList.Add(listItem);
            }*/

            // returnam lista de categorii
            return selectList;
        }
        
        private void SetAccessRights()
        {
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Colaborator") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
        }
        
    }
}
