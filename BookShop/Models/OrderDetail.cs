using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookShop.Models
{
    public class OrderDetail
    {
        public int OrderId { get; set; }
        public string BookIsbn { get; set; }
        public int Quantity { get; set; }
        [ValidateNever]

        public Order Order { get; set; }
        [ValidateNever]

        public Book Book { get; set; }
    }

}