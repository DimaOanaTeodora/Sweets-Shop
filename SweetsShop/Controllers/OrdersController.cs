using Microsoft.AspNet.Identity;
using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Orders
        public ActionResult Index(int ?id)
        {   
            Dictionary<int,int> count=new Dictionary<int, int>();
            Order order = db.Orders.Find(id);
            ICollection<ProductCount> OPs = db.ProductCounts.OrderBy(i=>i.Product_Id).Where(c => c.Order_Id == id).ToList();
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            ViewBag.Orders = order;
            var ord = db.Orders.Where(o => o.UserId == User.Identity.GetUserId());
            ViewBag.Products =order.Products.OrderBy(i=>i.ProductId);
            
            foreach(var OP in OPs)
            {
                if (count.ContainsKey(OP.Product_Id))
                    count[OP.Product_Id] += 1;
                else
                {
                    count.Add(OP.Product_Id, 1);
                }
            }
            ViewBag.Count = count;
            ViewBag.Total = order.Products.Sum(i => i.ProductPrice);
            return View();
        }

        public ActionResult MyCart(int? id)
        {
            Order order = db.Orders.Find(id);
            var UserId = User.Identity.GetUserId();
            var ord = db.Orders.Where(o => o.UserId == UserId);
            if(ord.Count()>0)
                return RedirectToAction("Index/" + ord.FirstOrDefault().OrderId.ToString());
            else     
                return View("~/Views/Orders/Empty.cshtml");
        }

        [Authorize (Roles= "User")]
        [HttpPost]
        public ActionResult Add (Product product,int id)
        {
            ProductCount OP;
            OP = new ProductCount();
            Order order;
            int ok=0;
            var UserId = User.Identity.GetUserId();
            var ord = db.Orders.Where(o => o.UserId == UserId);
            if (ord.Count() == 0)
            {
                order = new Order();
                ok = 1;
            }
            else
            {
                order = ord.FirstOrDefault();
            }
            product = db.Products.Find(id);
            order.UserId = User.Identity.GetUserId();
  
            if (order.Products == null)
            {
                order.Products = new List<Product>();
            }
            db.ProductCounts.Add(OP);
            order.Products.Add(product);

            if (ok == 1)
            {
                
                db.Orders.Add(order);
            }
            TempData["message"] = "Ati adaugat un produs in cos";
            db.SaveChanges();
            OP.Product_Id = product.ProductId;
            OP.Order_Id = order.OrderId;
            db.SaveChanges();
            return Redirect("/Products");
        }
        // DELETE: Delete
        [HttpDelete]
        [Authorize(Roles = "User")]
        public ActionResult Delete(int id)
        {
            Order order = db.Orders.Find(id);
            ICollection<ProductCount> OPs = db.ProductCounts.Where(c=> c.Order_Id==id).ToList();
            if (order.UserId == User.Identity.GetUserId() ||
                User.IsInRole("User"))
            {
                foreach (var OP in OPs)
                {
                    db.ProductCounts.Remove(OP);
                }
                db.Orders.Remove(order);
                db.SaveChanges();
                TempData["message"] = "Comanda a fost plasata!";
                return Redirect("/Products/Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un produs care nu va apartine!";
                return Redirect("/Products/Index");
            }

        }

    }
}