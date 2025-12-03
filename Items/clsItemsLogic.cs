using GroupProject.Common;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Items
{
    /// <summary>
    /// Contains business logic for Items Window
    /// </summary>
    internal class clsItemsLogic
    {
        /// <summary>
        /// SQL class
        /// </summary>
        private clsItemsSQL sqlItems;

        /// <summary>
        /// Database access class
        /// </summary>
        private clsDataAccess dataAccess;

        /// <summary>
        /// Constructor initializes the sql and data access
        /// </summary>
        public clsItemsLogic()
        {
            try
            {
                sqlItems = new clsItemsSQL();
                dataAccess = new clsDataAccess();
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructing items logic: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all items from database
        /// </summary>
        /// <returns>List of all items</returns>
        public List<clsItem> GetAllItems()
        {
            List<clsItem> items = new List<clsItem>();

            try
            {
                // Get SQL and execute query
                string sSQL = sqlItems.SelectAllItems();
                int iRows = 0;
                DataSet ds = dataAccess.ExecuteSQLStatement(sSQL, ref iRows);

                // Loop through results and create item objects
                for (int i = 0; i < iRows; i++)
                {
                    string code = ds.Tables[0].Rows[i]["ItemCode"].ToString();
                    string description = ds.Tables[0].Rows[i]["ItemDesc"].ToString();
                    double cost = Convert.ToDouble(ds.Tables[0].Rows[i]["Cost"]);

                    clsItem item = new clsItem(code, description, cost);
                    items.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all items: " + ex.Message);
            }

            return items;
        }

        /// <summary>
        /// Adds new item to database
        /// </summary>
        /// <param name="newItem">Item</param>
        public void AddItem(clsItem newItem)
        {
            try
            {
                // Get SQL and execute
                string sSQL = sqlItems.InsertItem(newItem.Code, newItem.Description,
                                                  newItem.Cost.ToString());
                dataAccess.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding item: " + ex.Message);
            }
        }

        /// <summary>
        /// Edits existing item in database
        /// </summary>
        /// <param name="oldItem">Original item</param>
        /// <param name="newItem">Updated item</param>
        public void EditItem(clsItem oldItem, clsItem newItem)
        {
            try
            {
                // Use item's code to identify which item to update
                string sSQL = sqlItems.UpdateItem(oldItem.Code, newItem.Description,
                                                  newItem.Cost.ToString());
                dataAccess.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error editing item: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes item from database
        /// </summary>
        /// <param name="itemToDelete">Item</param>
        public void DeleteItem(clsItem itemToDelete)
        {
            try
            {
                // Check if item is on any invoices
                if (IsItemOnInvoice(itemToDelete))
                {
                    throw new Exception("Cannot delete item that is on an existing invoice.");
                }

                // Get SQL and execute
                string sSQL = sqlItems.DeleteItem(itemToDelete.Code);
                dataAccess.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting item: " + ex.Message);
            }
        }

        /// <summary>
        /// Checks if item exists on any invoices
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>True if item is on invoice, false otherwise</returns>
        public bool IsItemOnInvoice(clsItem item)
        {
            try
            {
                // Get SQL and execute query
                string sSQL = sqlItems.SelectInvoicesWithItem(item.Code);
                int iRows = 0;
                DataSet ds = dataAccess.ExecuteSQLStatement(sSQL, ref iRows);

                // If any rows returned, item is on invoice
                return iRows > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking if item is on invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Get list of invoice numbers containing specified item
        /// </summary>
        /// <param name="item">Item</param>
        /// <returns>List of invoice numbers</returns>
        public List<int> GetInvoiceNumbersWithItem(clsItem item)
        {
            List<int> invoiceNumbers = new List<int>();

            try
            {
                string sSQL = sqlItems.SelectInvoicesWithItem(item.Code);
                int iRows = 0;
                DataSet ds = dataAccess.ExecuteSQLStatement(sSQL, ref iRows);

                for (int i = 0; i < iRows; i++)
                {
                    invoiceNumbers.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["InvoiceNum"]));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting invoice numbers: " + ex.Message);
            }

            return invoiceNumbers;
        }

        /// <summary>
        /// Validates edit function to prevent invoice totals from exceeding currency limit
        /// </summary>
        /// <param name="itemCode">Item code</param>
        /// <param name="newCost">New cost</param>
        /// <param name="oldCost">Old cost</param>
        /// <returns>Error message if validation fails, empty string if OK</returns>
        public string ValidateEditWontBreakInvoices(string itemCode, double newCost, double oldCost)
        {
            try
            {
                // If cost isn't changing much, skip validation
                if (Math.Abs(newCost - oldCost) < 0.01)
                    return "";

                const double MAX_CURRENCY = 922337203685477.5807;

                // Get invoices with item
                string sSQL = sqlItems.SelectInvoicesWithItem(itemCode);
                int iRows = 0;
                DataSet ds = dataAccess.ExecuteSQLStatement(sSQL, ref iRows);

                // For each invoice, calculate new total
                for (int i = 0; i < iRows; i++)
                {
                    int invoiceNum = Convert.ToInt32(ds.Tables[0].Rows[i]["InvoiceNum"]);

                    // Get line items for invoice
                    string sSQL2 = sqlItems.SelectLineItemsForInvoice(invoiceNum);
                    int iRows2 = 0;
                    DataSet ds2 = dataAccess.ExecuteSQLStatement(sSQL2, ref iRows2);

                    double total = 0;
                    for (int j = 0; j < iRows2; j++)
                    {
                        string code = ds2.Tables[0].Rows[j]["ItemCode"].ToString();
                        double cost = Convert.ToDouble(ds2.Tables[0].Rows[j]["Cost"]);

                        // Use new cost if current item is being edited
                        total += (code == itemCode) ? newCost : cost;
                    }

                    if (total > MAX_CURRENCY)
                    {
                        return $"Cannot update: Invoice #{invoiceNum} would exceed maximum total.";
                    }
                }

                return "";
            }
            catch (Exception ex)
            {
                throw new Exception("Error validating edit: " + ex.Message);
            }
        }
    }
}