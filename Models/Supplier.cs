using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;



namespace Ramirez_Mackenzie_HW5.Models
{
    public class Supplier 
    {
        // SUPPLIER ID
        [Key]
        [Required]
        [Display(Name = "Supplier ID:")]
        public Int32 SupplierID { get; set; }

        // SUPPLIER NAME
        [Required]
        [Display(Name = "Supplier Name:")]
        public String SupplierName { get; set; }

        // EMAIL
        [Required]
        [Display(Name = "Supplier Email:")]
        public String Email { get; set; }

        // PHONE NUMBER
        [Required]
        [Display(Name = "Phone number:")]
        public String PhoneNumber { get; set; }

        // NAVIGATIONAL
        // SUPPLIERS PROVIDE MULTIPLE PRODUCTS
        // 13A
        public List<Product> Products { get; set; }

        // FIGURE OUT LATER
        public Supplier()
        {
            if (Products == null)
            {
                Products = new List<Product>();
            }
        }

    }
}
