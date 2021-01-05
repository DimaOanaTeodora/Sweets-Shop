using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SweetsShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Numele categoriei este obligatoriu!")]
        [StringLength(100, ErrorMessage = "Numele categoriei are maxim 100 de caractere !")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}