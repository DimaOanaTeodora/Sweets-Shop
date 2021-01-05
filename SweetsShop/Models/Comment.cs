using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
// trebuie adaugat si user Id ul
namespace SweetsShop.Models
{
    public class Comment
    {
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [Key]
        public int CommentId { get; set;}

        [Required(ErrorMessage = "Continutul comentariului dumneavoastra este obligatoriu!")]
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public float? Rating{ get; set; }
        public virtual Product Product { get; set; }

    }
}