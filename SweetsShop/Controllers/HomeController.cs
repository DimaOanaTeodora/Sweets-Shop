using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            /*
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Products");
            }
            */

            var products = from product in db.Products
                           select product;

            
            ViewBag.Products = products;
           

            //nu avem data adaugarii unui produs
            // ViewBag.Products = products.OrderBy(o => o.Date).Skip(1).Take(2);

            return View();
        }
       
        /*
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        */
    }
}