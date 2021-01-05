using Microsoft.AspNet.Identity;
using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = from category in db.Categories
                             orderby category.CategoryName
                             select category;
            ViewBag.Categories = categories;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            SetAccessRights();

            return View();
          
        }

        // GET: Show
        public ActionResult Show(int id)
        {
            Category category = db.Categories.Find(id);
            return View(category);
        }

        // GET: New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            
            return View();
        }

        // POST: New
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Category category)
        {
            try
            {
                if (ModelState.IsValid) 
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost asaugata";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }
            catch (Exception e)
            {
                return View(category);
            }
        }

        // GET: Edit
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
           
            return View(category);
        }

        // PUT: Edit
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                Category category = db.Categories.Find(id);
                if (TryUpdateModel(category))
                {
                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata!";
                    return RedirectToAction("Index");
                }
                return View(requestCategory);
            }
            catch (Exception e)
            {
                return View(requestCategory);
            }
        }

        // DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost stearsa!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        private void SetAccessRights()
        {
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

        }
    }
}
   