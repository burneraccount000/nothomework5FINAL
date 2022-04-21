using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ramirez_Mackenzie_HW5.Models
{
    public class Order
    {
        // TAX
        private const Decimal TAX_RATE = 0.0825m;

        // ORDER ID
        [Key]
        [Required]
        [Display(Name = "Order ID:")]
        public Int32 OrderID { get; set; }

        // ORDER NUMBER
        [Required]
        [Display(Name = "Order Number:")]
        public Int32 OrderNumber { get; set; }

        // ORDER DATE
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Order Date:")]
        public DateTime OrderDate { get; set; }

        // ORDER NOTES
        [Display(Name = "Order Notes:")]
        public String OrderNotes { get; set; }

        // ORDER SUBTOTAL
        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(od => od.ExtendedPrice); }
        }

        // SALES TAX
        [Display(Name = "Sales Tax (8.5%)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal SalesTax
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        // ORDER TOTAL
        [Display(Name = "Order Total:")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + SalesTax; }
        }

        // NAVIGATIONAL
        // 13B
        // ORDER WILL CONSIST OF MANY ORDER DETAILS
        public List<OrderDetail> OrderDetails { get; set; }

        // EVERY ORDER HAS ONE CUSTOMER
        // 13D
        public AppUser User { get; set; }

        // FIGURE OUT LATER
        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }


    }
}
