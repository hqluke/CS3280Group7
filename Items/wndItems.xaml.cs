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
using GroupProject.Main;
using GroupProject.Search;
using GroupProject.Common;

namespace GroupProject.Items
{
    /// <summary>
    /// Interaction logic for wndItems.xaml
    /// </summary>
    public partial class wndItems : Window
    {
        /// <summary>
        /// Business logic for Items
        /// </summary>
        private clsItemsLogic itemsLogic;

        /// <summary>
        /// Tracks if items have been added, edited, or deleted
        /// Used by Main Window to refresh items combo box
        /// </summary>
        private bool bHasItemsBeenChanged;

        /// <summary>
        /// Tracks if items have been changed
        /// Main Window will check this property after window closes
        /// </summary>
        public bool HasItemsBeenChanged
        {
            get { return bHasItemsBeenChanged; }
        }

        /// <summary>
        /// Currently selected item in DataGrid
        /// </summary>
        private clsItem selectedItem;

        /// <summary>
        /// Constructor
        /// </summary>
        public wndItems()
        {
            InitializeComponent();
            itemsLogic = new clsItemsLogic();
            bHasItemsBeenChanged = false;
            LoadItems();
        }

        /// <summary>
        /// Displays success message in status label
        /// </summary>
        private void ShowSuccess(string message)
        {
            lblStatus.Text = message;
            lblStatus.Foreground = System.Windows.Media.Brushes.Green;
        }

        /// <summary>
        /// Displays error message in status label
        /// </summary>
        private void ShowError(string message)
        {
            lblStatus.Text = message;
            lblStatus.Foreground = System.Windows.Media.Brushes.Red;
        }

        /// <summary>
        /// Displays warning message in status label
        /// </summary>
        private void ShowWarning(string message)
        {
            lblStatus.Text = message;
            lblStatus.Foreground = System.Windows.Media.Brushes.Orange;
        }

        /// <summary>
        /// Clears status message
        /// </summary>
        private void ClearStatus()
        {
            lblStatus.Foreground = System.Windows.Media.Brushes.Black;
        }

        /// <summary>
        /// Loads all items from database into DataGrid
        /// </summary>
        private void LoadItems()
        {
            try
            {
                dgItems.ItemsSource = itemsLogic.GetAllItems();
                ClearStatus();
            }
            catch (Exception ex)
            {
                ShowError("Error loading items: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates text boxes with item data and enables Edit/Delete buttons
        /// </summary>
        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem != null)
            {
                selectedItem = (clsItem)dgItems.SelectedItem;
                txtCode.Text = selectedItem.Code;
                txtDescription.Text = selectedItem.Description;
                txtCost.Text = selectedItem.Cost.ToString();

                // Enable Edit and Delete buttons when item is selected
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                ClearStatus();
            }
            else
            {
                ClearFields();
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        /// <summary>
        /// Validates input and adds new item to database
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCode.Text) ||
                    string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtCost.Text))
                {
                    ShowWarning("Please fill in all fields.");
                    return;
                }

                // Validate cost is numeric
                if (!double.TryParse(txtCost.Text, out double cost))
                {
                    ShowWarning("Cost must be a valid number.");
                    return;
                }

                // Validate item code length (max 4 characters)
                if (txtCode.Text.Trim().Length > 4)
                {
                    ShowWarning("Item code cannot be longer than 4 characters.");
                    return;
                }

                // Validate description length (max 50 characters)
                if (txtDescription.Text.Trim().Length > 50)
                {
                    ShowWarning("Item description cannot be longer than 50 characters.");
                    return;
                }

                // Check if item code already exists
                var existingItems = itemsLogic.GetAllItems();
                if (existingItems.Any(item => item.Code.Equals(txtCode.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    ShowWarning($"Item code '{txtCode.Text.Trim()}' already exists. Please use a different code.");
                    return;
                }

                // Create new item
                clsItem newItem = new clsItem(txtCode.Text.Trim(),
                                             txtDescription.Text.Trim(),
                                             cost);

                // Add to database
                itemsLogic.AddItem(newItem);

                // Set flag that items have changed
                bHasItemsBeenChanged = true;

                // Refresh DataGrid
                LoadItems();
                ClearFields();

                ShowSuccess($"Item '{newItem.Code}' added successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error adding item: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates selected item in database
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedItem == null)
                {
                    ShowWarning("Please select an item to edit.");
                    return;
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(txtDescription.Text) ||
                    string.IsNullOrWhiteSpace(txtCost.Text))
                {
                    ShowWarning("Please fill in all fields.");
                    return;
                }

                // Validate cost is numeric
                if (!double.TryParse(txtCost.Text, out double cost))
                {
                    ShowWarning("Cost must be a valid number.");
                    return;
                }

                // Validate description length (max 50 characters)
                if (txtDescription.Text.Trim().Length > 50)
                {
                    ShowWarning("Item description cannot be longer than 50 characters.");
                    return;
                }

                // Validate invoices
                string validationError = itemsLogic.ValidateEditWontBreakInvoices(selectedItem.Code, cost, selectedItem.Cost);
                if (!string.IsNullOrEmpty(validationError))
                {
                    ShowError(validationError);
                    return;
                }

                // Create updated item (code cannot be changed, only description and cost)
                clsItem updatedItem = new clsItem(selectedItem.Code,
                                                 txtDescription.Text.Trim(),
                                                 cost);

                // Update in database
                itemsLogic.EditItem(selectedItem, updatedItem);

                // Set flag that items have changed
                bHasItemsBeenChanged = true;

                // Refresh DataGrid
                LoadItems();
                ClearFields();

                ShowSuccess($"Item '{updatedItem.Code}' updated successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Error editing item: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes selected item from database
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedItem == null)
                {
                    ShowWarning("Please select an item to delete.");
                    return;
                }

                // Store item code before deletion
                string itemCode = selectedItem.Code;

                // Check if item is on any invoices BEFORE showing delete confirmation
                if (itemsLogic.IsItemOnInvoice(selectedItem))
                {
                    // Get the invoice numbers
                    List<int> invoiceNumbers = itemsLogic.GetInvoiceNumbersWithItem(selectedItem);

                    // Create message with invoice numbers
                    string invoiceList = string.Join(", ", invoiceNumbers);

                    // Show error with specific invoice numbers (REQUIREMENT)
                    ShowError($"Cannot delete item '{itemCode}'. It is used on the following invoice(s): {invoiceList}");
                    return;
                }

                // Confirm deletion with MessageBox
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete item '{itemCode}'?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Delete from database
                    itemsLogic.DeleteItem(selectedItem);

                    // Set flag that items have changed
                    bHasItemsBeenChanged = true;

                    // Refresh DataGrid
                    LoadItems();
                    ClearFields();

                    ShowSuccess($"Item '{itemCode}' deleted successfully.");
                }
            }
            catch (Exception ex)
            {
                ShowError("Error deleting item: " + ex.Message);
            }
        }

        /// <summary>
        /// Closes window, Main Window will check HasItemsBeenChanged property
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Main Window will check HasItemsBeenChanged property after window closes
            // If true, Main Window should refresh items combo box
            this.Close();
        }

        /// <summary>
        /// Clears all input fields
        /// </summary>
        private void ClearFields()
        {
            txtCode.Text = "";
            txtDescription.Text = "";
            txtCost.Text = "";
            selectedItem = null;
            dgItems.SelectedItem = null;
        }
    }
}