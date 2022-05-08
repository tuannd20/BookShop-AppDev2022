using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]

        [Required(ErrorMessage = "Please enter Isbn")]
        public string Isbn { get; set; }
        [Required(ErrorMessage = "Please enter Store")]

        public int StoreId { get; set; }
        [Required(ErrorMessage = "Please enter Title")]

        public string Title { get; set; }
        [Required(ErrorMessage = "Please enterPages")]

        public int Pages { get; set; }
        [Required(ErrorMessage = "Please enter Author")]

        public string Author { get; set; }
        [Required(ErrorMessage = "Please enter Category")]

        public string Category { get; set; }
        [Required(ErrorMessage = "Please enter Price")]

        public int Price { get; set; }
        [Required(ErrorMessage = "Please enter Description")]

        public string Desc { get; set; }
        [Required(ErrorMessage = "Please enter Image url")]

        public string? ImgUrl { get; set; }

        [ValidateNever]
        public Store Store { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

        public virtual ICollection<Cart>? Carts { get; set; }

    }
}