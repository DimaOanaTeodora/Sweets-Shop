using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SweetsShop.Models
{
    public class ProductCount
    {
        [Key]
        public int OrderProductId { get; set; }

        public int Product_Id { get; set; }


        public int Order_Id { get; set; }


    }
}