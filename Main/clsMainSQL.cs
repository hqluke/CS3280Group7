using GroupProject.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Main
{
    /// <summary>
    /// Contains all SQL statements for the Main window.
    /// Handles database operations for invoices and line items.
    /// </summary>
    class clsMainSQL
    {
        /// <summary>
        /// Member instance for database access
        /// </summary>
        clsDataAccess db = new clsDataAccess();

        /// <summary>
        /// Updates the total cost of an invoice.
        /// </summary>
        public void UpdateInvoiceTotalCost(int invoiceNum, double totalCost)
        {
            try
            {
                string sSQL = $"UPDATE Invoices SET TotalCost = {totalCost.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE InvoiceNum = {invoiceNum}";
                db.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sql update invoice total cost: " + ex.Message);
            }
        }

        /// <summary>
        /// Inserts a new line item into an invoice.
        /// </summary>
        public void InsertLineItem(int invoiceNum, int lineItemNum, string itemCode)
        {
            try
            {
                string sSQL = $"INSERT INTO LineItems (InvoiceNum, LineItemNum, ItemCode) VALUES ({invoiceNum}, {lineItemNum}, '{itemCode}')";
                db.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sql insert line item: " + ex.Message);
            }
        }

        /// <summary>
        /// Creates a new invoice in the database.
        /// Converts DateOnly to DateTime for database compatibility.
        /// </summary>
        public int InsertInvoice(DateOnly invoiceDate, double totalCost)
        {
            try
            {
                // Convert DateOnly to DateTime for Access database
                DateTime dateTime = invoiceDate.ToDateTime(TimeOnly.MinValue);

                string sSQL = $"INSERT INTO Invoices (InvoiceDate, TotalCost) VALUES (#{dateTime.ToShortDateString()}#, {totalCost})";
                db.ExecuteNonQuery(sSQL);

                sSQL = "SELECT MAX(InvoiceNum) FROM Invoices";
                string result = db.ExecuteScalarSQL(sSQL);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sql insert invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a specific invoice and converts DateTime to DateOnly.
        /// </summary>
        public clsInvoice GetInvoiceByNumber(int invoiceNum)
        {
            try
            {
                string sSQL = $"SELECT InvoiceNum, InvoiceDate, TotalCost FROM Invoices WHERE InvoiceNum = {invoiceNum}";
                int rowCount = 0;
                DataSet ds = db.ExecuteSQLStatement(sSQL, ref rowCount);

                if (rowCount > 0)
                {
                    DataRow row = ds.Tables[0].Rows[0];
                    int invNum = Convert.ToInt32(row["InvoiceNum"]);
                    DateTime dateTime = Convert.ToDateTime(row["InvoiceDate"]);
                    DateOnly invDate = DateOnly.FromDateTime(dateTime); // Convert DateTime to DateOnly
                    double total = Convert.ToDouble(row["TotalCost"]);

                    return new clsInvoice(invNum, invDate, total);
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error sql get invoice by number: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all items from the ItemDesc table.
        /// SQL: SELECT ItemCode, ItemDesc, Cost FROM ItemDesc
        /// </summary>
        /// <returns>DataSet containing all items</returns>
        public DataSet GetAllInvoiceNumbers()
        {
            try
            {
                string sSQL = "SELECT InvoiceNum FROM Invoices";
                int rowCount = 0;
                return db.ExecuteSQLStatement(sSQL, ref rowCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sql get all invoice numbers: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the price of a new item based on its code. Returns -1.0 if item not found.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public double GetNewItemPrice(string code)
        {
            try
            {
                string sSQL = $"SELECT Cost FROM ItemDesc WHERE ItemCode = '{code}'";
                string result = db.ExecuteScalarSQL(sSQL);
                if (result == "")
                {
                    return -1.0;
                }
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error get new item price: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all line items for a specific invoice with their details.
        /// </summary>
        /// <returns>DataSet containing the line items with descriptions and costs</returns>
        public DataSet GetLineItemsForInvoice(int invoiceNum)
        {
            try
            {
                string sSQL = $"SELECT LineItems.ItemCode, ItemDesc.ItemDesc, ItemDesc.Cost, LineItems.LineItemNum " +
                             $"FROM LineItems, ItemDesc " +
                             $"WHERE LineItems.ItemCode = ItemDesc.ItemCode " +
                             $"AND LineItems.InvoiceNum = {invoiceNum} " +
                             "ORDER BY LineItems.LineItemNum";
                int rowCount = 0;
                return db.ExecuteSQLStatement(sSQL, ref rowCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Error get line items for invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes all line items for a specific invoice.
        /// </summary>
        public void DeleteLineItemsForInvoice(int invoiceNum)
        {
            try
            {
                string sSQL = $"DELETE FROM LineItems WHERE InvoiceNum = {invoiceNum}";
                db.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete line items for invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes a specific line item from an invoice.
        /// </summary>
        public void DeleteLineItem(int invoiceNum, int lineItemNum)
        {
            try
            {
                string sSQL = $"DELETE FROM LineItems WHERE InvoiceNum = {invoiceNum} AND LineItemNum = {lineItemNum}";
                db.ExecuteNonQuery(sSQL);
            }
            catch (Exception ex)
            {
                throw new Exception("Error delete line item: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets the next available line item number for an invoice.
        /// </summary>
        public int GetNextLineItemNumber(int invoiceNum)
        {
            try
            {
                string sSQL = $"SELECT MAX(LineItemNum) FROM LineItems WHERE InvoiceNum = {invoiceNum}";
                string result = db.ExecuteScalarSQL(sSQL);

                // If no line items exist, return 1; otherwise return max + 1
                if (string.IsNullOrEmpty(result))
                    return 1;

                return Convert.ToInt32(result) + 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error get next line item number: " + ex.Message);
            }
        }
    }
}