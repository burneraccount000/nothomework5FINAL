using Ramirez_Mackenzie_HW5.DAL;
using System;
using System.Linq;

namespace Ramirez_Mackenzie_HW5.Utilities
{
    public static class OrderNumberGenerator
    {
        public static Int32 GetNextOrderNumber(AppDbContext _context)
        {
            //Set a number where the order numbers should start
            const Int32 START_NUMBER = 70001;

            Int32 intMaxOrderNumber; //the current maximum order number
            Int32 intNextOrderNumber; //the course number for the next class

            if (_context.Order.Count() == 0) //there are no orders in the database yet
            {
                intMaxOrderNumber = START_NUMBER; //order numbers start at 70001
            }
            else
            {
                intMaxOrderNumber = _context.Order.Max(c => c.OrderNumber);
                //this is the highest number in the database right now
            }

            //You added orders before you realized that you needed this code
            //and now you have some order numbers less than 70001
            if (intMaxOrderNumber < START_NUMBER)
            {
                intMaxOrderNumber = START_NUMBER;
            }

            //add one to the current max to find the next one
            intNextOrderNumber = intMaxOrderNumber + 1;

            //return the value
            return intNextOrderNumber;
        }

    }
}
