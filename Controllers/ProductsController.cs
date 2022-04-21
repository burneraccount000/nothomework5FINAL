using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ramirez_Mackenzie_HW5.DAL;
using Ramirez_Mackenzie_HW5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;



namespace Ramirez_Mackenzie_HW5.Controllers
{

    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Products
        public async Task<IActionResult> Index()
        {
            

            return View(await _context.Products.ToListAsync());
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify a product to view!" });
            }

            Product product = await _context.Products
                .Include(c => c.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return View("Error", new String[] { "That product was not found in the database." });
            }

            return View(product);
        }


        /////////// PRODUCTS/CREATE
        /// GET
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // POPULATING VIEW BAG FOR SUPPLIERS
            ViewBag.AllSuppliers = GetAllSuppliers();

            return View();
        }

        /////////// PRODUCTS/CREATE
        /// POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ProductID,Name,Description,Price,ProductType")] Product product, int[] SelectedSuppliers)
        {

            if (ModelState.IsValid == false)
            {
                ViewBag.AllSuppliers = GetAllSuppliers();
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();

            foreach (int supplierID in SelectedSuppliers)
            {
                //find the Supplier associated with that id
                Supplier dbSupplier = _context.Supplier.Find(supplierID);

                //add the Supplier to the product's list of suppliers and save changes
                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }

            // IF NOT ADMIN - NOT AUTHORIZED
            if (User.IsInRole("Admin") == false)
            {
                return View("Error", new string[] { "You are not authorized to create!" });
            }

            return RedirectToAction(nameof(Index));
        }


        /////////// PRODUCTS/EDIT/5
        /// GET
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            // id check
            if (id == null)
            {
                return View("Error", new string[] { "Please specify a product to edit!" });
            }

            // find product in db and change data type
            Product product = await _context.Products.Include(c => c.Suppliers)
                                           .FirstOrDefaultAsync(c => c.ProductID == id);

            //var product = await _context.Products.FindAsync(id);

            // product check
            if (product == null)
            {
                return View("Error", new string[] { "This product was not found!" });
            }

            // populate view bag with existing suppliers
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }


        /////////// PRODUCTS/EDIT/5
        /// POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, [Bind("ProductID,Name,Description,Price,ProductType")] Product product, int[] SelectedSuppliers)
        {
            // IF PRODUCT ID DOES NOT MATCH
            if (id != product.ProductID)
            {
                return View("Error", new string[] { "Please try again!" });
            }

            // IF NOT VALID
            if (ModelState.IsValid == false) 
            {
                ViewBag.AllSuppliers = GetAllSuppliers(product);
                return View(product);
            }

            // IF VALID
            //if code gets this far, attempt to edit the product
            try
            {
                //Find the course to edit in the database and include relevant 
                //navigational properties
                Product dbProduct = _context.Products
                    .Include(c => c.Suppliers)
                    .FirstOrDefault(c => c.ProductID == product.ProductID);

                //create a list of departments that need to be removed
                List<Supplier> SuppliersToRemove = new List<Supplier>();

                //find the departments that should no longer be selected because the
                //user removed them
                //remember, SelectedDepartments = the list from the HTTP request (listbox)
                foreach (Supplier supplier in dbProduct.Suppliers)
                {
                    //see if the new list contains the department id from the old list
                    if (SelectedSuppliers.Contains(supplier.SupplierID) == false)//this department is not on the new list
                    {
                        SuppliersToRemove.Add(supplier);
                    }
                }

                //remove the departments you found in the list above
                //this has to be 2 separate steps because you can't iterate (loop)
                //over a list that you are removing things from
                foreach (Supplier supplier in SuppliersToRemove)
                {
                    //remove this course department from the course's list of departments
                    dbProduct.Suppliers.Remove(supplier);
                    _context.SaveChanges();
                }

                //add the departments that aren't already there
                foreach (int supplierID in SelectedSuppliers)
                {
                    if (dbProduct.Suppliers.Any(d => d.SupplierID == supplierID) == false)
                        //this department is NOT already associated with this course
                    {
                        //Find the associated department in the database
                        Supplier dbSupplier = _context.Supplier.Find(supplierID);

                        //Add the department to the course's list of departments
                        dbProduct.Suppliers.Add(dbSupplier);
                        _context.SaveChanges();
                    }
                }

                //update the product's scalar properties
                dbProduct.Price = product.Price;
                dbProduct.Name = product.Name;
                dbProduct.Description = product.Description;

                //save the changes
                _context.Products.Update(dbProduct);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an error editing this product.", ex.Message });
            }

            //if code gets this far, everything is okay
            //send the user back to the page with all the courses
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .FirstOrDefaultAsync(m => m.ProductID == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        // POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var product = await _context.Products.FindAsync(id);
        //    _context.Products.Remove(product);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }

        // GET ALL SUPPLIERS
        private MultiSelectList GetAllSuppliers()
        {
            // get the list of suppliers from the database
            List<Supplier> SupplierList = _context.Supplier.ToList();

            // multiselect constructor method to get new multiselectlist
            MultiSelectList supplierSelectList = new MultiSelectList(SupplierList.OrderBy(m => m.SupplierID), "SupplierID", "SupplierName");

            //return the SelectList
            return supplierSelectList;
        }

        // GET ALL SUPPLIERS
        private MultiSelectList GetAllSuppliers(Product product)
        {
            //Create a new list of suppliers and get the list of the suppliers
            //from the database
            List<Supplier> allDepartments = _context.Supplier.ToList();

            //loop through the list of suppliers to find a list of supplier ids
            //create a list to store the supplier ids
            List<Int32> selectedSupplierIDs = new List<Int32>();

            //Loop through the list to find the SupplierIDs
            foreach (Supplier associatedSupplier in product.Suppliers)
            {
                selectedSupplierIDs.Add(associatedSupplier.SupplierID);
            }

            //use the MultiSelectList constructor method to get a new MultiSelectList
            MultiSelectList mslAllSuppliers = new MultiSelectList(allDepartments.OrderBy(d => d.SupplierName), "SupplierID", "SupplierName", selectedSupplierIDs);

            //return the MultiSelectList
            return mslAllSuppliers;
        }

    }
}
