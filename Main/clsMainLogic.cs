using GroupProject.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GroupProject.Main
{
    class clsMainLogic
    {
        /// <summary>
        /// SQL query class for Main window
        /// </summary>
        private clsMainSQL sqlMain;
        /// <summary>
        /// Represents the data access layer used for interacting with the database.
        /// </summary>
        private clsDataAccess dataAccess;
        /// <summary>
        /// List of all invoice numbers in the database
        /// </summary>
        private List<int> invoiceNumbersList;
        /// <summary>
        /// Stores original items for the current invoice when first loaded. originalLineItems and currentLineItems are compared when saving
        /// </summary>
        private Dictionary<int, clsItem> originalLineItems = new Dictionary<int, clsItem>();
        /// <summary>
        /// Updated items for the current invoice. originalLineItems and currentLineItems are compared when saving
        /// </summary>
        private Dictionary<int, clsItem> currentLineItems = new Dictionary<int, clsItem>();
        /// <summary>
        /// Used to assign temporary large line item numbers for new items added to the invoice
        /// </summary>
        private int tempLineItemCounter;
        /// <summary>
        /// Represents the newly created invoice's date
        /// </summary>
        public InvoiceKey currentData;
        

        /// <summary>
        /// Constructs new main logic, initializing sql and data fields
        /// </summary>
        public clsMainLogic()
        {
            try
            {
                sqlMain = new clsMainSQL();
                dataAccess = new clsDataAccess();
            }
            catch (Exception ex)
            {
                throw new Exception("Error construct main logic: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all line items for a specific invoice
        /// </summary>
        public List<clsItem> GetInvoiceItems(int invoiceNum)
        {
            try
            {
                List<clsItem> items = new List<clsItem>();
                originalLineItems.Clear();
                currentLineItems.Clear();
                tempLineItemCounter = 0;

                // Get invoice items from database for the given invoice number
                DataSet ds = sqlMain.GetLineItemsForInvoice(invoiceNum);

                // Check if there are any rows
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string code = row["ItemCode"].ToString();
                        string description = row["ItemDesc"].ToString();
                        double cost = Convert.ToDouble(row["Cost"]);
                        int lineItemNum = Convert.ToInt32(row["LineItemNum"]);
                        tempLineItemCounter++;
                        clsItem item = new clsItem(code, description, cost);
                        items.Add(item);

                        originalLineItems[lineItemNum] = item;
                        currentLineItems[lineItemNum] = item;
                    }
                    tempLineItemCounter++;
                }
                return items;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting invoice items: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets a specific invoice by its number
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public clsInvoice SelectInvoice(int invoiceNum)
        {
            try
            {
                return sqlMain.GetInvoiceByNumber(invoiceNum);
            }
            catch (Exception ex)
            {
                throw new Exception("Error selecting invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Stores all invoice numbers from the database into a list so the update totals method can use it
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void GetAllInvoiceNumbers()
        {
            try
            {
                DataSet ds = sqlMain.GetAllInvoiceNumbers();
                invoiceNumbersList = new List<int>();
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        int num = Convert.ToInt32(row["InvoiceNum"]);
                        invoiceNumbersList.Add(num);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all invoice numbers: " + ex.Message);
            }

        }

        /// <summary>
        /// Updates the total cost for all invoices in the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateAllInvoiceTotals()
        {
            try
            {
                GetAllInvoiceNumbers();
                foreach (int invoiceNum in invoiceNumbersList)
                {
                    double num = 0;
                    DataSet ds = sqlMain.GetLineItemsForInvoice(invoiceNum);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            double cost = Convert.ToDouble(row["Cost"]);
                            num += cost;

                        }
                        sqlMain.UpdateInvoiceTotalCost(invoiceNum, num);

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating all invoice totals: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates the total cost for a specific invoice
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <exception cref="Exception"></exception>
        public void UpdateCurrentInvoiceTotal(int invoiceNum)
        {
            try
            {
                double num = 0;
                DataSet ds = sqlMain.GetLineItemsForInvoice(invoiceNum);
                // Check if there are any rows
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        double cost = Convert.ToDouble(row["Cost"]);
                        num += cost;

                    }
                    sqlMain.UpdateInvoiceTotalCost(invoiceNum, num);

                }
                // If there are no line items, set total cost to 0
                else
                {
                    sqlMain.UpdateInvoiceTotalCost(invoiceNum, num);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating current invoice total: " + ex.Message);
            }
        }

        /// <summary>
        /// Adds an item to the current line items dictionary
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="Exception"></exception>
        public void AddItemToInvoice(clsItem item)
        {
            try
            {
                // Use large numbers keys for new items
                tempLineItemCounter++;
                currentLineItems[10000 + tempLineItemCounter] = item;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding item to invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Removes an item from the current line items dictionary
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool RemoveItemFromInvoice(clsItem item)
        {
            try
            {
                // Find the last matching item by code
                int lastMatchKey = currentLineItems
                    .Where(kvp => kvp.Value.Code == item.Code)
                    .OrderByDescending(kvp => kvp.Key)
                    .Select(kvp => kvp.Key)
                    .FirstOrDefault();

                //Remove it if found
                if (lastMatchKey != 0)
                {
                    currentLineItems.Remove(lastMatchKey);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item from invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Clears all items from both the original and current line item collections.
        /// </summary>
        /// <exception cref="Exception">
        public void ClearItemsList()
        {
            try
            {
                originalLineItems.Clear();
                currentLineItems.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Error clearing items list: " + ex.Message);
            }
        }
        /// <summary>
        /// Gets the current list of items in the invoice
        /// </summary>
        /// <returns>currentLineItems.Values.ToList();</returns>
        /// <exception cref="Exception"></exception>
        public List<clsItem> GetCurrentItems()
        {
            try
            {
                return currentLineItems
                    .OrderBy(kvp => kvp.Key)
                    .Select(kvp => kvp.Value)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting current items: " + ex.Message);
            }
        }

        /// <summary>
        /// Saves changes made to the invoice's line items back to the database
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <exception cref="Exception"></exception>
        public void SaveInvoice(int invoiceNum)
        {
            try
            {
                //Get keys that are in original but not in current and delete them
                var lineItemsToDelete = originalLineItems.Keys
                    .Except(currentLineItems.Keys)
                    .ToList();

                foreach (int lineItemNum in lineItemsToDelete)
                {
                    sqlMain.DeleteLineItem(invoiceNum, lineItemNum);
                }

                // Insert new items (those with negative keys are new)
                var lineItemsToInsert = currentLineItems
                    .Where(kvp => kvp.Key >= 10000)
                    .OrderBy(kvp => kvp.Key)
                    .ToList();

                foreach (var kvp in lineItemsToInsert)
                {
                    // Get next line item number
                    int newLineItemNum = sqlMain.GetNextLineItemNumber(invoiceNum);
                    // Insert line item
                    sqlMain.InsertLineItem(invoiceNum, newLineItemNum, kvp.Value.Code);
                }

                // Update invoice total
                UpdateCurrentInvoiceTotal(invoiceNum);

                // Refresh from database to get correct state
                GetInvoiceItems(invoiceNum);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Holds the date for the newly created invoice
        /// </summary>
        public readonly struct InvoiceKey
        {
            public DateOnly Date { get; }
            public InvoiceKey(DateOnly date)
            {
                Date = date;
            }

        }
        /// <summary>
        /// Returns a new invoice object with the specified date. Total cost and invoice number are set to zero.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public clsInvoice StoreInvoice(DateOnly date)
        {
            try
            {
                currentData = new InvoiceKey(date);
                clsInvoice temp = new clsInvoice(0, currentData.Date, 0);
                return temp;
            }
            catch (Exception ex)
            {
                throw new Exception("Error storing invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserts a new invoice into the database and returns its invoice number.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public int CreateInvoice()
        {
            try
            {
                return sqlMain.InsertInvoice(currentData.Date, 0);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Refreshes the prices of all current line items from the database.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void UpdateCurrentItems()
        {
            try
            {
                // Update prices for all current line items
                foreach (var kvp in currentLineItems)
                {
                    double updatedPrice = sqlMain.GetNewItemPrice(kvp.Value.Code);
                    if (updatedPrice == -1.0)
                    {
                        RemoveAllItemsWithCode(kvp.Value);
                        return;
                    }

                    kvp.Value.Cost = updatedPrice;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating current items: " + ex.Message);
            }
        }

        /// <summary>
        /// Removes all items with the same item code as item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool RemoveAllItemsWithCode(clsItem item)
        {
            try
            {
                // Find all matching items by code
                var keysToRemove = currentLineItems
                    .Where(kvp => kvp.Value.Code == item.Code)
                    .Select(kvp => kvp.Key)
                    .ToList(); // ToList() to avoid modifying collection during enumeration

                // Remove all matching items
                if (keysToRemove.Any())
                {
                    foreach (var key in keysToRemove)
                    {
                        currentLineItems.Remove(key);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item from invoice: " + ex.Message);
            }
        }
    }
}

