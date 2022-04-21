using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Ramirez_Mackenzie_HW5.DAL;
using Ramirez_Mackenzie_HW5.Utilities;
using Ramirez_Mackenzie_HW5.Models;

namespace Ramirez_Mackenzie_HW5.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET:
        // ORDERS INDEX
        public IActionResult Index()
        {

            // IF ADMIN - VIEW ALL
            // todo
            List<Order> orders;

            if (User.IsInRole("Admin"))
            {
                orders = _context.Order.Include(r => r.OrderDetails).ToList();
            }

            // CUSTOMER
            else
            {
                orders = _context.Order
                                .Include(r => r.OrderDetails)
                                .Where(r => r.User.UserName == User.Identity.Name)
                                .ToList();
            }

            return View(orders);
        }

        // GET:
        // ORDER/DETAILS/5
        public async Task<IActionResult> Details(int? id)
        {
            // USER DID NOT SPECIFY AN ORDER
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order to view!" });
            }

            // FIND ORDER IN DB
            Order order = await _context.Order
                                              .Include(r => r.OrderDetails)
                                              .ThenInclude(r => r.Product)
                                              .Include(r => r.User)
                                              .FirstOrDefaultAsync(m => m.OrderID == id);

            // NO ORDER FOUND
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found!" });
            }

            // CUSTOMER
            if (User.IsInRole("Customer") && order.User.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "This is not your order!  Don't be such a snoop!" });
            }

            // SEND USER TO DETAILS PAGE
            return View(order);
        }


        // GET
        // ORDERS/CREATE
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            return View();
        }


        // POST:
        // ORDERS/CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([Bind("OrderNotes")] Order order)
        {
            //Find the next order number from the utilities class
            order.OrderNumber = Utilities.OrderNumberGenerator.GetNextOrderNumber(_context);

            //Set the date of this order
            order.OrderDate = DateTime.Now;

            //Associate the order with the logged-in customer
            order.User = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            //make sure all properties are valid
            if (ModelState.IsValid == false)
            {
                return View(order);
            }

            //if code gets this far, add the registration to the database
            _context.Add(order);
            await _context.SaveChangesAsync();

            //send the user on to the action that will allow them to 
            //create a order detail.  Be sure to pass along the OrderID
            //that you created when you added the order to the database above
            return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
        }


        // GET:
        // ORDER/EDIT/5
        public IActionResult Edit(int? id)
        {

            // user did not specify an order to edit
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a order to edit" });
            }

            // find the order in the database, and be sure to include details
            Order order = _context.Order
                                       .Include(r => r.OrderDetails)
                                       .ThenInclude(r => r.Product)
                                       .Include(r => r.User)
                                       .FirstOrDefault(r => r.OrderID == id);

            // order was not found in the database
            if (order == null)
            {
                return View("Error", new String[] { "This order was not found in the database!" });
            }

            // order does not belong to this user
            if (User.IsInRole("Customer") && order.User.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "You are not authorized to edit this order!" });
            }

            return View(order);

        }


        // POST:
        // ORDER/EDIT/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderNotes")] Order order)

        {
            if (id != order.OrderID)
            {
                return View("Error", new String[] { "There was a problem editing this order. Try again!" });
            }

            //if there is something wrong with this order, try again
            if (ModelState.IsValid == false)
            {
                return View(order);
            }

            //if code gets this far, update the record
            try
            {
                //find the record in the database
                Order dbOrder = _context.Order.Find(order.OrderID);

                //update the notes
                dbOrder.OrderNotes = order.OrderNotes;

                _context.Update(dbOrder);
                await _context.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                return View("Error", new String[] { "There was an error updating this order!", ex.Message });
            }


            return RedirectToAction(nameof(Index));
        }

        // GET: Orders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var order = await _context.Order
        //        .FirstOrDefaultAsync(m => m.OrderID == id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(order);
        //}


        // POST: Orders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var order = await _context.Order.FindAsync(id);
        //    _context.Order.Remove(order);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderID == id);
        }
    }
}
