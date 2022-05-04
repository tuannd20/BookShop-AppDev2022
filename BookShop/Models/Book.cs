using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Book
    {
        [Key]
        public string Isbn { get; set; }
        public int StoreId { get; set; }
        public string Title { get; set; }
        public int Pages { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public int Price { get; set; }
        public string Desc { get; set; }
        public string? ImgUrl { get; set; }
        [ValidateNever]

        public Store Store { get; set; }
        public virtual ICollection<OrderDetail>? OrderDetails { get; set; }

        public virtual ICollection<Cart>? Carts { get; set; }

    }
}