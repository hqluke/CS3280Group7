using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Items
{
    /// <summary>
    /// Contains all SQL statements for Items Window
    /// </summary>
    class clsItemsSQL
    {
        /// <summary>
        /// Gets all items from ItemDesc table
        /// </summary>
        /// <returns>SQL statement</returns>
        public string SelectAllItems()
        {
            string sSQL = "SELECT ItemCode, ItemDesc, Cost FROM ItemDesc ORDER BY ItemCode";
            return sSQL;
        }

        /// <summary>
        /// Gets all invoices containing specific item
        /// </summary>
        /// <param name="sItemCode">Item code</param>
        /// <returns>SQL statement</returns>
        public string SelectInvoicesWithItem(string sItemCode)
        {
            string sSQL = "SELECT DISTINCT(InvoiceNum) FROM LineItems WHERE ItemCode = '" + sItemCode + "'";
            return sSQL;
        }

        /// <summary>
        /// Updates existing item in ItemDesc table
        /// </summary>
        /// <param name="sItemCode">Item code</param>
        /// <param name="sItemDesc">Item description</param>
        /// <param name="sCost">New cost</param>
        /// <returns>SQL statement</returns>
        public string UpdateItem(string sItemCode, string sItemDesc, string sCost)
        {
            string sSQL = "UPDATE ItemDesc SET ItemDesc = '" + sItemDesc + "', Cost = " + sCost +
                         " WHERE ItemCode = '" + sItemCode + "'";
            return sSQL;
        }

        /// <summary>
        /// Inserts new item into ItemDesc table
        /// </summary>
        /// <param name="sItemCode">Item code</param>
        /// <param name="sItemDesc">Item description</param>
        /// <param name="sCost">Item cost</param>
        /// <returns>SQL statement</returns>
        public string InsertItem(string sItemCode, string sItemDesc, string sCost)
        {
            string sSQL = "INSERT INTO ItemDesc (ItemCode, ItemDesc, Cost) VALUES ('" +
                         sItemCode + "', '" + sItemDesc + "', " + sCost + ")";
            return sSQL;
        }

        /// <summary>
        /// Deletes item from ItemDesc table
        /// </summary>
        /// <param name="sItemCode">Item code</param>
        /// <returns>SQL statement</returns>
        public string DeleteItem(string sItemCode)
        {
            string sSQL = "DELETE FROM ItemDesc WHERE ItemCode = '" + sItemCode + "'";
            return sSQL;
        }

        /// <summary>
        /// Gets all line items with costs for specific invoice
        /// </summary>
        /// <param name="invoiceNum">Invoice number</param>
        /// <returns>SQL statement</returns>
        public string SelectLineItemsForInvoice(int invoiceNum)
        {
            string sSQL = "SELECT ItemCode, Cost " +
                          "FROM LineItems L " +
                          "INNER JOIN ItemDesc I ON L.ItemCode = I.ItemCode " +
                          "WHERE InvoiceNum = " + invoiceNum;
            return sSQL;
        }
    }
}
