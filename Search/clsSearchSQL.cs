using GroupProject.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GroupProject.Search
{
    /// <summary>
    /// Contains various sql statements and generator methods
    /// </summary>
    class clsSearchSQL
    {
        /// <summary>
        /// Generates a sql statement to select invoices according to the
        /// parameters. If none are set, selects all invoices
        /// </summary>
        /// <param name="invoiceNum"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetInvoices(
            int? invoiceNum = null,
            DateOnly? invoiceDate = null,
            double? totalCost = null)
        {
            try
            {
                string sql = "SELECT * FROM Invoices";

                if (invoiceNum != null)
                {
                    sql += " WHERE InvoiceNum = " + invoiceNum;

                    if (invoiceDate != null)
                    {
                        sql += " AND InvoiceDate = #" +
                            invoiceDate.ToString() + "#";
                    }

                    if (totalCost != null)
                    {
                        sql += " AND TotalCost = " + totalCost;
                    }
                }
                else if (invoiceDate != null)
                {
                    sql += " WHERE InvoiceDate = #" + invoiceDate.ToString() + "#";


                    if (totalCost != null)
                    {
                        sql += " AND TotalCost = " + totalCost;
                    }
                }
                else if (totalCost != null)
                {
                    sql += " WHERE TotalCost = " + totalCost;
                }

                return sql;
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
        /// SQL query selects the distinct invoice numbers
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetDistinctInvoiceNums()
        {
            try
            {
                string sql = "SELECT DISTINCT(InvoiceNum) From Invoices" +
                    " order by InvoiceNum";

                return sql;
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
        /// SQL query selects the distinct invoice dates
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetDistinctInvoiceDates()
        {
            try
            {
                string sql = "SELECT DISTINCT(InvoiceDate) From Invoices" +
                    " order by InvoiceDate";

                return sql;
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
        /// SQL query selects the distinct invoice total costs
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string GetDistinctTotalCost()
        {
            try
            {
                string sql = "SELECT DISTINCT(TotalCost) From Invoices" +
                    " order by TotalCost";

                return sql;
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
