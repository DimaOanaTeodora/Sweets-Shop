using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetsShop.Models
{
    public class Product
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Numele este este obligatoriu!")]
        [StringLength(30, ErrorMessage = "Numele nu poate avea mai mult de 30 de caractere!")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Descrierea este obligatorie!")]
        [DataType(DataType.MultilineText)]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Pretul este  obligatoriu!")]
        public double ProductPrice { get; set; }

        [Required(ErrorMessage = "Disponibilitatea este obligatorie!")]
        public int ProductStoc { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie!")]
        public int CategoryId { get; set; }

        //[Required(ErrorMessage = "Imaginea este este obligatorie!")]
        public string Photo { get; set; }

        public bool Request { get; set; }
        public float ProductRating { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Order>Orders { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }

       

    }
}