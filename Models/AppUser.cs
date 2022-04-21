using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace Ramirez_Mackenzie_HW5.Models
{
    public class AppUser : IdentityUser
    {

        // FIRST NAME
        [Required]
        [Display(Name = "First Name:")]
        public String FirstName { get; set; }

        // LAST NAME
        [Required]
        [Display(Name = "Last Name:")]
        public String LastName { get; set; }

        // A CUSTOMER WILL HAVE MANY ORDERS
        // 13D
        public List<Order> Orders { get; set; }


    }
}
