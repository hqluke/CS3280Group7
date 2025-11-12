using GroupProject.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Search
{
    /// <summary>
    /// Business logic for the search window, handles database retrieval
    /// and formatting
    /// </summary>
    public class clsSearchLogic
    {
        /// <summary>
        /// Reference to the data access class, executes sql queries
        /// </summary>
        private clsDataAccess db = new clsDataAccess();
        /// <summary>
        /// Reference to the sql class, contains and generates sql queries
        /// </summary>
        private clsSearchSQL searchSQL = new clsSearchSQL();

        /// <summary>
        /// Gets the invoices according the parameters, if none are set,
        /// returns every invoice
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<clsInvoice> GetInvoices(
            int? invoiceNum = null,
            DateOnly? invoiceDate = null,
            double? totalCost = null
        )
        {
            try
            {
                int rows = 0;

                DataSet ds = db.ExecuteSQLStatement(
                    searchSQL.GetInvoices(invoiceNum, invoiceDate, totalCost),
                    ref rows
                );

                var result = new List<clsInvoice>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(
                        new clsInvoice(
                            Convert.ToInt32(row["InvoiceNum"]),

                            DateOnly.FromDateTime(
                                Convert.ToDateTime(row["InvoiceDate"])
                            ),

                            Convert.ToDouble(row["TotalCost"])
                        )
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    MethodInfo.GetCurrentMethod()!.DeclaringType!.Name + "." +
                    MethodInfo.GetCurrentMethod()!.Name + " -> " +
                    ex
                );
            }
        }

        /// <summary>
        /// Gets the distinct list of invoice numbers
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<int> GetDistinctInvoiceNums()
        {
            try
            {
                int rows = 0;
                DataSet ds = db.ExecuteSQLStatement(
                    searchSQL.GetDistinctInvoiceNums(), ref rows);

                var result = new List<int>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(Convert.ToInt32(row["InvoiceNum"]));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    MethodInfo.GetCurrentMethod()!.DeclaringType!.Name + "." +
                    MethodInfo.GetCurrentMethod()!.Name + " -> " +
                    ex
                );
            }
        }

        /// <summary>
        /// Gets the list of distinct invoice dates
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<DateOnly> GetDistinctInvoiceDates()
        {
            try
            {
                int rows = 0;
                DataSet ds = db.ExecuteSQLStatement(
                    searchSQL.GetDistinctInvoiceDates(), ref rows);

                var result = new List<DateOnly>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(
                        DateOnly.FromDateTime(
                            Convert.ToDateTime(row["InvoiceDate"])
                        )
                    );
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    MethodInfo.GetCurrentMethod()!.DeclaringType!.Name + "." +
                    MethodInfo.GetCurrentMethod()!.Name + " -> " +
                    ex
                );
            }
        }

        /// <summary>
        /// Gets the list of distinct invoice total costs
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<double> GetDistinctTotalCost()
        {
            try
            {
                int rows = 0;
                DataSet ds = db.ExecuteSQLStatement(
                    searchSQL.GetDistinctTotalCost(), ref rows);

                var result = new List<double>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    result.Add(Convert.ToDouble(row["TotalCost"]));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    MethodInfo.GetCurrentMethod()!.DeclaringType!.Name + "." +
                    MethodInfo.GetCurrentMethod()!.Name + " -> " +
                    ex
                );
            }
        }
    }
}
