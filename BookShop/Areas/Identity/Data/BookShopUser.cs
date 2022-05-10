using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BookShopUser class
public class BookShopUser : IdentityUser
{
    [PersonalData]
    public DateTime? DoB { get; set; }
    [PersonalData]
    public string? Address { get; set; }
    public Store? Store { get; set; }
        
    public virtual ICollection<Cart>? Carts { get; set; }

    public virtual ICollection<Order>? Orders { get; set; }

}

