using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SweetsShop.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}