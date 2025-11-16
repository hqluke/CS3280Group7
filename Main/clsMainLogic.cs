using GroupProject.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Main
{
    class clsMainLogic
    {
        private clsMainSQL sqlMain;
        private clsDataAccess dataAccess;

        public clsMainLogic()
        {
            sqlMain = new clsMainSQL();
            dataAccess = new clsDataAccess();
        }

        /// <summary>
        /// Retrieves all line items for a specific invoice
        /// </summary>
        public List<clsItem> GetInvoiceItems(int invoiceNum)
        {
            List<clsItem> items = new List<clsItem>();

            try
            {
                DataSet ds = sqlMain.GetLineItemsForInvoice(invoiceNum);

                // Check if there are any rows
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        string code = row["ItemCode"].ToString();
                        string description = row["ItemDesc"].ToString();
                        double cost = Convert.ToDouble(row["Cost"]);

                        clsItem item = new clsItem(code, description, cost);
                        items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting invoice items: " + ex.Message);
            }

            return items;
        }
    }
}

