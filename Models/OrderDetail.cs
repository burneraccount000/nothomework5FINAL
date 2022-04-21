using System;
using System.ComponentModel.DataAnnotations;


namespace Ramirez_Mackenzie_HW5.Models
{
    public class OrderDetail
    {

        public Int32 OrderDetailID { get; set; }

        // QUANTITY
        [Required]
        [Display(Name = "Quantity:")]
        [Range(1, 1000, ErrorMessage = "Quantity must be between 1 and 1000")]
        public Int32 Quantity { get; set; }

        // PRODUCT PRICE
        [Display(Name = "Product Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ProductPrice { get; set; }

        //// EXTENDED PRICE
        [Display(Name = "Extended Price:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ExtendedPrice { get; set; }



        // ONE TO ONE
        // 13B
        // A SINGLE ORDER DETAIL WILL BE ASSOCIATED WITH A SINGLE ORDER
        public Order Order { get; set; }

        // A SINGLE 
        public Product Product { get; set; }




    }
}
