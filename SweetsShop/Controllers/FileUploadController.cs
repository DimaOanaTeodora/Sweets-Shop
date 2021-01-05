using SweetsShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Controllers
{
    public class FileUploadController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: FileUpload
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase uploadedFile)
        {
            // Se preia numele fisierul
            string uploadedFileName = uploadedFile.FileName;
            string uploadedFileExtension = Path.GetExtension(uploadedFileName);

            // Se poate verifica daca extensia este intr-o lista dorita
            if (uploadedFileExtension == ".png" || uploadedFileExtension == ".jpg" || uploadedFileExtension == ".pdf")
            {
                // Se stocheaza fisierul in folderul Files (folderul trebuie creat in proiect)

                // 1. Se seteaza calea folderului de upload
                string uploadFolderPath = Server.MapPath("~//Files//");

                // 2. Se salveaza fisierul in acel folder
                uploadedFile.SaveAs(uploadFolderPath + uploadedFileName);

                // 3. Se face o instanta de model si se populeaza cu datele necesare
                UploadFile file = new UploadFile();
                file.Extension = uploadedFileExtension;
                file.FileName = uploadedFileName;
                file.FilePath = uploadFolderPath + uploadedFileName;

                // 4. Se adauga modelul in baza de date
                db.UploadFiles.Add(file);
                db.SaveChanges();


                // 5. Return catre index cu mesaj de succes - TODO
                return Redirect("Index");

            }

            // TODO: tratarea erorilor
            return View();
        }
        
        // GET: Show
        public ActionResult Show(int id)
        {
            UploadFile uploadfile = db.UploadFiles.Find(id);
            return View(uploadfile);
        }
        
    }
}