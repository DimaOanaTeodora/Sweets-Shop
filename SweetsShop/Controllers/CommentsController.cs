using Microsoft.AspNet.Identity;
using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Products/Show/" + comm.ProductId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }

        }
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
           // var ratings = db.Comments.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
            if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                return RedirectToAction("Index", "Products");
            }

        }

        [HttpPut]
        [Authorize(Roles = "User,Colaborator,Admin")]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comm = db.Comments.Find(id);

                if (comm.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(comm))
                    {
                        comm.Content = requestComment.Content;
                        comm.Rating = requestComment.Rating;
                        db.SaveChanges();
                    }
                    return Redirect("/Products/Show/" + comm.ProductId);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari";
                    return RedirectToAction("Index", "Products");
                }
            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }
    }
}