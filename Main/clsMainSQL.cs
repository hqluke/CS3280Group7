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
        clsDataAccess db = new clsDataAccess();

        /// <summary>
        /// Updates the total cost of an invoice.
        /// </summary>
        public void UpdateInvoiceTotalCost(int invoiceNum, double totalCost)
        {
            string sSQL = $"UPDATE Invoices SET TotalCost = {totalCost.ToString(System.Globalization.CultureInfo.InvariantCulture)} WHERE InvoiceNum = {invoiceNum}";
            db.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Inserts a new line item into an invoice.
        /// </summary>
        public void InsertLineItem(int invoiceNum, int lineItemNum, string itemCode)
        {
            string sSQL = $"INSERT INTO LineItems (InvoiceNum, LineItemNum, ItemCode) VALUES ({invoiceNum}, {lineItemNum}, '{itemCode}')";
            db.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Creates a new invoice in the database.
        /// Converts DateOnly to DateTime for database compatibility.
        /// </summary>
        public int InsertInvoice(DateOnly invoiceDate, double totalCost)
        {
            // Convert DateOnly to DateTime for Access database
            DateTime dateTime = invoiceDate.ToDateTime(TimeOnly.MinValue);

            string sSQL = $"INSERT INTO Invoices (InvoiceDate, TotalCost) VALUES (#{dateTime.ToShortDateString()}#, {totalCost})";
            db.ExecuteNonQuery(sSQL);

            sSQL = "SELECT MAX(InvoiceNum) FROM Invoices";
            string result = db.ExecuteScalarSQL(sSQL);
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Retrieves a specific invoice and converts DateTime to DateOnly.
        /// </summary>
        public clsInvoice GetInvoiceByNumber(int invoiceNum)
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

        /// <summary>
        /// Retrieves all items from the ItemDesc table.
        /// SQL: SELECT ItemCode, ItemDesc, Cost FROM ItemDesc
        /// </summary>
        /// <returns>DataSet containing all items</returns>
        public DataSet GetAllItems()
        {
            string sSQL = "SELECT ItemCode, ItemDesc, Cost FROM ItemDesc";
            int rowCount = 0;
            return db.ExecuteSQLStatement(sSQL, ref rowCount);
        }


        /// <summary>
        /// Retrieves all items from the ItemDesc table.
        /// SQL: SELECT ItemCode, ItemDesc, Cost FROM ItemDesc
        /// </summary>
        /// <returns>DataSet containing all items</returns>
        public DataSet GetAllInvoiceNumbers()
        {
            string sSQL = "SELECT InvoiceNum FROM Invoices";
            int rowCount = 0;
            return db.ExecuteSQLStatement(sSQL, ref rowCount);
        }

        public int GetHighestInvoiceNum()
        {
            string sSQL = "SELECT MAX(InvoiceNum) FROM Invoices";
            string result = db.ExecuteScalarSQL(sSQL);
            return Convert.ToInt32(result) + 1;
        }

        public double GetNewItemPrice(string code)
        {
            string sSQL = $"SELECT Cost FROM ItemDesc WHERE ItemCode = '{code}'";
            string result = db.ExecuteScalarSQL(sSQL);
            return Convert.ToDouble(result);
        }

        /// <summary>
        /// Retrieves all line items for a specific invoice with their details.
        /// </summary>
        /// <returns>DataSet containing the line items with descriptions and costs</returns>
        public DataSet GetLineItemsForInvoice(int invoiceNum)
        {
            string sSQL = $"SELECT LineItems.ItemCode, ItemDesc.ItemDesc, ItemDesc.Cost, LineItems.LineItemNum " +
                         $"FROM LineItems, ItemDesc " +
                         $"WHERE LineItems.ItemCode = ItemDesc.ItemCode " +
                         $"AND LineItems.InvoiceNum = {invoiceNum} " +
                         "ORDER BY LineItems.LineItemNum";
            int rowCount = 0;
            return db.ExecuteSQLStatement(sSQL, ref rowCount);
        }

        /// <summary>
        /// Deletes all line items for a specific invoice.
        /// </summary>
        public void DeleteLineItemsForInvoice(int invoiceNum)
        {
            string sSQL = $"DELETE FROM LineItems WHERE InvoiceNum = {invoiceNum}";
            db.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Deletes a specific line item from an invoice.
        /// </summary>
        public void DeleteLineItem(int invoiceNum, int lineItemNum)
        {
            string sSQL = $"DELETE FROM LineItems WHERE InvoiceNum = {invoiceNum} AND LineItemNum = {lineItemNum}";
            db.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Gets the next available line item number for an invoice.
        /// </summary>
        public int GetNextLineItemNumber(int invoiceNum)
        {
            string sSQL = $"SELECT MAX(LineItemNum) FROM LineItems WHERE InvoiceNum = {invoiceNum}";
            string result = db.ExecuteScalarSQL(sSQL);

            // If no line items exist, return 1; otherwise return max + 1
            if (string.IsNullOrEmpty(result))
                return 1;

            return Convert.ToInt32(result) + 1;
        }

        /// <summary>
        /// Deletes an entire invoice from the database.
        /// </summary>
        public void DeleteInvoice(int invoiceNum)
        {
            // First delete all line items
            DeleteLineItemsForInvoice(invoiceNum);

            // Then delete the invoice
            string sSQL = $"DELETE FROM Invoices WHERE InvoiceNum = {invoiceNum}";
            db.ExecuteNonQuery(sSQL);
        }

        /// <summary>
        /// Updates an invoice's date, converting DateOnly to DateTime.
        /// </summary>
        public void UpdateInvoiceDate(int invoiceNum, DateOnly newDate)
        {
            DateTime dateTime = newDate.ToDateTime(TimeOnly.MinValue);
            string sSQL = $"UPDATE Invoices SET InvoiceDate = #{dateTime.ToShortDateString()}# WHERE InvoiceNum = {invoiceNum}";
            db.ExecuteNonQuery(sSQL);
        }
    }
}