using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Ramirez_Mackenzie_HW5.Models
{
    // PRODUCT TYPE
    public enum ProductType { Hat, Pants, Sweatshirt, Tank, TShirt, Other }

    public class Product
    {
        // PRODUCT ID
        [Key]
        [Required]
        [Display(Name = "Product ID:")]
        public Int32 ProductID { get; set; }

        // PRODUCT NAME
        [Required(ErrorMessage = "Product name is required!")]
        [Display(Name = "Product Name:")]
        public String Name { get; set; }

        // DESCRIPTION
        [Display(Name = "Product Description:")]
        public String Description { get; set; }

        // PRICE
        [Display(Name = "Product Price:")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal Price { get; set; }

        [Display(Name = "Product Type:")]
        public ProductType ProductType { get; set; }

        // NAVIGATIONAL
        // 13C
        // PRODUCT WILL BE PART OF MANY ORDER DETAILS
        public List<OrderDetail> OrderDetails { get; set; }

        // 13A
        // MANY TO MANY
        // PRODUCTS ARE PROVIDED BY MULTIPLE SUPPLIERS
        public List<Supplier> Suppliers { get; set; }

        public Product()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }

            if (Suppliers == null)
            {
                Suppliers = new List<Supplier>();
            }
        }


    }
}
