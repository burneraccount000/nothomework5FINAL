using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ramirez_Mackenzie_HW5.DAL;
using Ramirez_Mackenzie_HW5.Models;

namespace Ramirez_Mackenzie_HW5.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET:
        // ORDERDETAILS
        public IActionResult Index(int? orderID)
        {
            if (orderID == null)
            {
                return View("Error", new String[] { "Please specify a order to view!" });
            }

            //limit the list to only the order details that belong to this registration
            List<OrderDetail> ods = _context.OrderDetail
                                          .Include(od => od.Product)
                                          .Where(od => od.Order.OrderID == orderID)
                                          .ToList();

            return View(ods);
        }


        // GET:
        // ORDERDETAILS/DETAILS/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetail = await _context.OrderDetail
        //        .FirstOrDefaultAsync(m => m.OrderDetailID == id);
        //    if (orderDetail == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderDetail);
        //}


        // GET:
        // ORDERDETAILS/CREATE
        public IActionResult Create(int orderID)
        {
            //create a new instance of the OrderDetail class
            OrderDetail od = new OrderDetail();

            //find the order that should be associated with this registration
            Order dbOrder = _context.Order.Find(orderID);

            //set the new order detail's registration equal to the registration you just found
            od.Order = dbOrder;

            //populate the ViewBag with a list of existing products
            ViewBag.AllProducts = GetAllProducts();

            //pass the newly created order detail to the view
            return View(od);
        }

        // POST:
        // ORDERDETAILS/CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Order, OrderDetailID, Quantity")] OrderDetail orderDetail, int SelectedProduct)
        {
            // IF FALSE - DO THIS
            if (ModelState.IsValid == false)
            {
                ViewBag.AllProducts = GetAllProducts();
                return View(orderDetail);
            }

            // FIND PRODUCT ASSOCIATED W THIS ORDER
            Product dbProduct = _context.Products.Find(SelectedProduct);

            //set the order detail's course to be equal to the one we just found
            orderDetail.Product = dbProduct;

            // FIND ORDER IN DATABASE
            Order dbOrder = _context.Order.Find(orderDetail.Order.OrderID);

            // SET ORDER ON ORDER DETAIL OF ORDER WE JUST FOUND
            orderDetail.Order = dbOrder;

            // SET ORDER DETAILS PRICE TO PRODUCT PRICE
            //this will allow us to to store the price that the user paid
            orderDetail.ProductPrice = dbProduct.Price;

            // CALCULATE EXTENDED PRICE FOR ORDER DETAIL
            orderDetail.ExtendedPrice = orderDetail.Quantity * orderDetail.ProductPrice;

            // ADD ORDER DETAIL TO DATBASE
            _context.Add(orderDetail);
            await _context.SaveChangesAsync();

            // SEND USER TO DETAILS PAGE
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }


        // GET: OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetail.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return View(orderDetail);
        }


        // POST: OrderDetails/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailID, Quantity, ProductPrice")] OrderDetail orderDetail)
        {
            //this is a security check to make sure they are editing the correct record
            if (id != orderDetail.OrderDetailID)
            {
                return View("Error", new String[] { "There was a problem editing this record. Try again!" });
            }

            //information is not valid, try again
            if (ModelState.IsValid == false)
            {
                return View(orderDetail);
            }

            //create a new registration detail
            OrderDetail dbOD;

            //if code gets this far, update the record
            try
            {
                //find the existing registration detail in the database
                //include both registration and course
                dbOD = _context.OrderDetail
                      .Include(od => od.Product)
                      .Include(od => od.Order)
                      .FirstOrDefault(od => od.OrderDetailID == orderDetail.OrderDetailID);

                //update the scalar properties
                dbOD.Quantity = orderDetail.Quantity;
                dbOD.ProductPrice = dbOD.ProductPrice;
                dbOD.ExtendedPrice = dbOD.Quantity * dbOD.ProductPrice;

                //save changes
                _context.Update(dbOD);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem editing this record", ex.Message });
            }

            //if code gets this far, go back to the registration details index page
            return RedirectToAction("Details", "Orders", new { id = dbOD.Order.OrderID });
        }


        // GET: OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order detail to delete!" });
            }

            var orderDetail = await _context.OrderDetail
                .FirstOrDefaultAsync(m => m.OrderDetailID == id);

            if (orderDetail == null)
            {
                return View("Error", new String[] { "This order detail was not in the database!" });
            }

            return View(orderDetail);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //find the order detail to delete
            OrderDetail orderDetail = await _context.OrderDetail
                                                   .Include(r => r.Order)
                                                   .FirstOrDefaultAsync(r => r.OrderDetailID == id);

            //delete the order detail
            _context.OrderDetail.Remove(orderDetail);
            await _context.SaveChangesAsync();

            //return the user to the registration/details page
            return RedirectToAction("Details", "Orders", new { id = orderDetail.Order.OrderID });
        }


        // PRIVATE
        // ORDERDETAIL EXISTS
        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetail.Any(e => e.OrderDetailID == id);
        }

        // PRIVATE
        // GET ALL PRODUCTS
        private SelectList GetAllProducts()
        {

            //create a list for all the courses
            List<Product> allProducts = _context.Products.ToList();

            //use the constructor on select list to create a new select list with the options
            SelectList slAllProducts = new SelectList(allProducts, nameof(Product.ProductID), nameof(Product.Name));

            return slAllProducts;

        }

    }
}
