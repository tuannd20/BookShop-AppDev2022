using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Areas.Identity.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BookShop.Models
{

    public class Cart
    {
        public string UserId { get; set; }
        public string BookIsbn { get; set; }

        [ValidateNever]
        public BookShopUser User { get; set; }

        [ValidateNever]

        public Book Book { get; set; }
    }

}