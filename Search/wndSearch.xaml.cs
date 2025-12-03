using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GroupProject.Items;
using GroupProject.Search;
using GroupProject.Common;
using System.Collections.ObjectModel;
using System.Reflection;
namespace GroupProject.Search
{
    /// <summary>
    /// Interaction logic for windowSearch.xaml
    /// </summary>
    public partial class wndSearch : Window
    {
        /// <summary>
        /// Reference to the business logic class, handling database
        /// retrieval
        /// </summary>
        public clsSearchLogic SearchLogic { get; private set; }
        /// <summary>
        /// The selected invoice is accessed by wndMain via getSelectedInvoice()
        /// </summary>
        private clsInvoice? selectedInvoice;

        /// <summary>
        /// Constructs a new search window, binding data to combo boxes and the
        /// data grid from the database
        /// </summary>
        /// <exception cref="Exception"></exception>
        public wndSearch()
        {
            try
            {
                InitializeComponent();
                SearchLogic = new clsSearchLogic();
                cbxInvoiceNumber.ItemsSource =
                    SearchLogic.GetDistinctInvoiceNums();

                cbxInvoiceDate.ItemsSource =
                    SearchLogic.GetDistinctInvoiceDates();

                cbxInvoiceTotalCost.ItemsSource =
                    SearchLogic.GetDistinctTotalCost();

                dgdInvoices.ItemsSource = SearchLogic.GetInvoices();
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
        /// The current selected invoice, this is accessed by wndMain once
        /// the dialog has closed.
        /// </summary>
        public clsInvoice? getSelectedInvoice()
        {
            try
            {
                return selectedInvoice;
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
        /// Closes the window. At this point there is a valid
        /// selection as the select button will only be enabled
        /// if a selection has been made. wndMain will
        /// obtain the selected invoice via getSelectedInvoice() after
        /// the dialog has closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }

        /// <summary>
        /// Updates the selected invoice and enables the select button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgdInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                selectedInvoice = (clsInvoice?)dgdInvoices.SelectedItem;
                btnSelect.IsEnabled = selectedInvoice != null;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles the invoice number selection change by updating the
        /// list of invoices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxInvoiceNumber_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            try
            {
                updateInvoices();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }

        /// <summary>
        /// Calls the business logic function to get an updated list of
        /// invoices based on the form's arguments
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateInvoices()
        {
            try
            {
                dgdInvoices.ItemsSource = SearchLogic.GetInvoices(
                    (int?)cbxInvoiceNumber.SelectedItem,
                    (DateOnly?)cbxInvoiceDate.SelectedItem,
                    (double?)cbxInvoiceTotalCost.SelectedItem);
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
        /// Handles top level exception errors by showing a message box or
        /// writing to a file
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="sMessage"></param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt",
                    Environment.NewLine + "HandleError Exception: " +
                    ex.Message);
            }
        }

        /// <summary>
        /// Handles the invoice date selection change by updating the
        /// list of invoices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxInvoiceDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                updateInvoices();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles the invoice total cost selection change by updating the
        /// list of invoices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxInvoiceTotalCost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                updateInvoices();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }

        /// <summary>
        /// Handles the clear button click event which clears the selected
        /// items in each combo box and updates the invoice list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cbxInvoiceNumber.SelectedItem = null;
                cbxInvoiceDate.SelectedItem = null;
                cbxInvoiceTotalCost.SelectedItem = null;
                dgdInvoices.ItemsSource = SearchLogic.GetInvoices();
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod()!.DeclaringType!.Name,
                    MethodInfo.GetCurrentMethod()!.Name, ex.Message);
            }
        }
    }
}
