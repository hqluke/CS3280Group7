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

namespace GroupProject.Main
{
    /// <summary>
    /// Interaction logic for wndCreateInvoice.xaml
    /// </summary>
    public partial class wndCreateInvoice : Window
    {
        /// <summary>
        /// Interface to communicate with wndMain
        /// </summary>
        public ICreateInvoiceWindow CreateInvoiceInterface { get; set; }
        /// <summary>
        /// Checks if DatePicker has validation error
        /// </summary>
        private bool hasValidationError = false;
        public wndCreateInvoice()
        {
            InitializeComponent();
            // Set DatePicker to today's date by default and attach validation error handler
            dP.DateValidationError += DatePicker_DateValidationError;
            dP.SelectedDate = DateTime.Today;
        }
        /// <summary>
        /// Checks for validation errors in DatePicker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            hasValidationError = true;
            errorLabelEmpty.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handler for Confirm button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dP.SelectedDate == null || hasValidationError)
                {
                    // Show error, reset flag, and exit
                    errorLabelEmpty.Visibility = Visibility.Visible;
                    hasValidationError = false;
                    return;
                }
                // Get the DateTime value from the DatePicker
                DateTime selectedDateTime = dP.SelectedDate.Value;
                // Convert the DateTime to a DateOnly
                DateOnly date = DateOnly.FromDateTime(selectedDateTime);

                errorLabelEmpty.Visibility = Visibility.Hidden;
                // Call interface method to notify main window
                CreateInvoiceInterface?.onConfirmCreateInvoice(date);
                // Reset DatePicker to today's date
                dP.SelectedDate = DateTime.Today;
                Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error changing item seleciton from combo box " + ex.Message);
            }
        }

        /// <summary>
        /// Cancel button click handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Close the window without taking action
                Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error cancelling create invoice window " + ex.Message);
            }
        }
    }

    /// <summary>
    /// Interface for Create Invoice Window to communicate with Main Window
    /// </summary>

    public interface ICreateInvoiceWindow
    {
        /// <summary>
        /// Called when user confirms creation of new invoice. Passes selected date.
        /// </summary>
        /// <param name="date"></param>
        void onConfirmCreateInvoice(DateOnly date);
    }
}
