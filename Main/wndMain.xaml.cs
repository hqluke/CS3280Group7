using GroupProject.Common;
using GroupProject.Items;
using GroupProject.Search;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace GroupProject.Main
    
    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class wndMain : Window, ICreateInvoiceWindow
    {

        /// <summary>
        /// Current invoice item
        /// </summary>
        clsInvoice currentInvoice;
        /// <summary>
        /// Lists all available items in the database
        /// </summary>
        List<clsItem> allAvailableItems;
        /// <summary>
        /// Current item selected from combo box/clicked in data grid
        /// </summary>
        clsItem currentItem;
        /// <summary>
        /// Reference to the business logic class for items
        /// </summary>
        clsItemsLogic itemsLogic;
        /// <summary>
        /// Reference to the business logic class for main window
        /// </summary>
        clsMainLogic mainLogic;
        /// <summary>
        /// Boolean to track if invoice is in edit mode
        /// </summary>
        private bool canEditInvoice = false;
        /// <summary>
        /// Boolean to track if invoice has been created in database
        /// </summary>
        private bool invoiceCreated = true;
        /// <summary>
        /// Stores the previously selected item before opening the items edit window.
        /// Allows re-selection of the same item if it still exists after editing.
        /// </summary>
        clsItem previouslySelectedItem;
        /// <summary>
        /// Represents the maximum currency value allowed by the database.
        /// </summary>
        private const double MAX_CURRENCY = 922337203685477.5807;

        public wndMain()
        {
            InitializeComponent();
            itemsLogic = new clsItemsLogic();
            mainLogic = new clsMainLogic();
            start();
        }

        /// <summary>
        /// Binds the Item's comboBox to all the currently available items.
        /// </summary>
        private void start()
        {
            try
            {
                allAvailableItems = itemsLogic.GetAllItems();
                comboBoxItems.ItemsSource = allAvailableItems;
                comboBoxItems.DisplayMemberPath = "Display";
            }
            catch (Exception ex)
            {
                throw new Exception("Error starting program " + ex.Message);
            }

        }

        /// <summary>
        /// Creates a new wndItems window that handles all the Items in the data base.
        /// Main window data grid will update if item's in the collection are deleted.
        /// Main window Item ComboBox will update if a new item is added.
        /// </summary>
        private void menuEditItemsClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                // Store the currently selected item before opening the items window
                previouslySelectedItem = (clsItem)comboBoxItems.SelectedItem;
                wndItems itemsWindow = new wndItems();
                itemsWindow.ShowDialog();
                bool hasBeenChanged = itemsWindow.HasItemsBeenChanged;
                // Refresh the list if items have been changed:
                if (hasBeenChanged)
                {
                    //Get updated list of items
                    List<clsItem> newItems = itemsLogic.GetAllItems();
                    allAvailableItems = newItems;
                    comboBoxItems.ItemsSource = allAvailableItems;
                    // Reselect the previously selected item if it still exists
                    if (previouslySelectedItem != null)
                    {
                        var itemToReselect = allAvailableItems.FirstOrDefault(item =>
                            item.Code == previouslySelectedItem.Code);

                        if (itemToReselect != null)
                        {
                            comboBoxItems.SelectedItem = itemToReselect;
                        }
                        else
                        {
                            // Item was deleted, clear selection
                            comboBoxItems.SelectedItem = null;
                        }
                    }
                    // Update all invoice totals if something changed.
                    if (currentInvoice == null)
                    {
                        return;
                    }
                    mainLogic.UpdateAllInvoiceTotals();
                    // Check if we need to update the currently selected invoice's items
                    mainLogic.UpdateCurrentItems();
                    dgInvoice.ItemsSource = null;
                    displayInvoice();
                    return;

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error opening search window or gathering all items " + ex.Message);
            }

        }

        ///<summary>
        ///Opens a search window that allows a user to select an invoice.
        ///Gets the selected invoice from the search window. If it's a new invoice object,
        ///it binds the main window's data grid to the selected invoice 
        /// </summary>
        private void menuSearchClicked(object sender, RoutedEventArgs e)
        {
            try {
                // Open the search window and store the selected invoice 
                wndSearch searchWindow = new wndSearch();
                searchWindow.ShowDialog();
                clsInvoice newInvoice = searchWindow.getSelectedInvoice();
                // If the selected invoice is the same as the current one or null, do nothing
                if (currentInvoice == newInvoice || newInvoice == null)
                {
                    return;
                }
                // Set up the main window for the selected invoice
                buttonEditInvoice.IsEnabled = true;
                buttonSaveInvoice.IsEnabled = true;
                labelEditMode.Visibility = Visibility.Hidden;
                invoiceCreated = true;
                // Reset the main logic's item list
                mainLogic.ClearItemsList();
                dgInvoice.IsReadOnly = true;
                canEditInvoice = false;
                HideAllErrors();
                currentInvoice = newInvoice;
                // Load the invoice items from the database
                mainLogic.GetInvoiceItems(currentInvoice.InvoiceNumber);
                // Display the invoice in the main window
                displayInvoice();
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening search window " + ex.Message);
            }
        }

        /// <summary>
        /// Displays the current invoice in the main window's data grid
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void displayInvoice()
        {
            try
            {
                // Load the line items for this invoice
                List<clsItem> invoiceItems = mainLogic.GetCurrentItems();

                // Clear existing items and add the loaded ones
                currentInvoice.ItemsList.Clear();
                currentInvoice.TotalCost = 0;
                foreach (clsItem item in invoiceItems)
                {
                    currentInvoice.AddItemToInvoice(item);
                    currentInvoice.TotalCost += item.Cost;
                }
                dgInvoice.ItemsSource = currentInvoice.ItemsList;

                // If invoice is not yet created in DB, show TBD
                if (!invoiceCreated)
                {
                    labelInvoiceNumberDisplay.Content = "TBD";
                }
                else
                {
                    labelInvoiceNumberDisplay.Content = currentInvoice.InvoiceNumber.ToString();

                }

                labelInvoiceDateDisplay.Content = currentInvoice.InvoiceDate.ToString();
                labelInvoiceTotalCostDisplay.Content = currentInvoice.TotalCost.ToString("C");

            }
            catch (Exception ex)
            {
                throw new Exception("Error displaying invoice " + ex.Message);
            }

        }
        /// <summary>
        /// Handles adding an item to the current invoice but not to the db until saved.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonAddItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentInvoice == null)
                {
                    ShowError("Search or create an invoice to add items");
                    return;
                }
                // If the selected item is a clsItem object, reference the object as itemToAdd
                if (comboBoxItems.SelectedItem is clsItem itemToAdd && canEditInvoice == true)
                {
                    double newTotal = currentInvoice.TotalCost + itemToAdd.Cost;

                    // Prevents adding item if it would exceed max currency value allowed by db
                    if (newTotal > MAX_CURRENCY)
                    {
                        ShowError($"Cannot add item: Total would exceed maximum allowed value of ${MAX_CURRENCY:N2}");
                        return;
                    }
                    HideAllErrors();
                    // Adds the item to the current invoice but not to the db until saved
                    mainLogic.AddItemToInvoice(itemToAdd);
                    // Gets updated list of items in the invoice
                    List<clsItem> invoiceItems = mainLogic.GetCurrentItems();
                    // Clear and re-add all items to the current invoice
                    currentInvoice.ItemsList.Clear();
                    foreach (clsItem item in invoiceItems)
                    {
                        currentInvoice.AddItemToInvoice(item);
                    }
                    // Update the total cost
                    currentInvoice.TotalCost += itemToAdd.Cost;
                    labelInvoiceTotalCostDisplay.Content = currentInvoice.TotalCost.ToString("C");

                }
                // If they are not in edit mode, show error
                else if (canEditInvoice == false)
                {
                    ShowError("Your are not currently editing an invoice");
                }
                // If no item is selected, show error
                else
                {
                    ShowError("Select an item to add");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding item " + ex.Message);
            }

        }
        /// <summary>
        /// Displays an error message in the error label
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        private void ShowError(string message)
        {
            try
            {
                errorLabel.Content = message;
                errorLabel.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                throw new Exception("Error showing error message " + ex.Message);
            }

        }
        /// <summary>
        /// Hides the error label
        /// </summary>
        private void HideAllErrors()
        {
            errorLabel.Visibility = Visibility.Hidden;

        }
        /// <summary>
        /// Removes an item from the current invoice but not from the db until saved
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If there is no current invoice, show error
                if (currentInvoice == null) { ShowError("Search or create an invoice to remove items"); return; }
                // If the selected item is a clsItem object, reference the object as itemToAdd
                if (comboBoxItems.SelectedItem is clsItem itemToAdd && canEditInvoice == true)
                {
                    HideAllErrors();
                    // Removes the item from the current invoice but not from the db until saved
                    // See if the removal worked
                    bool worked = mainLogic.RemoveItemFromInvoice(itemToAdd);
                    // Gets updated list of items in the invoice
                    List<clsItem> invoiceItems = mainLogic.GetCurrentItems();
                    currentInvoice.ItemsList.Clear();
                    // Re-add all items to the current invoice
                    foreach (clsItem item in invoiceItems)
                    {
                        currentInvoice.AddItemToInvoice(item);
                    }
                    // Update the total cost if removal worked
                    if (worked)
                    {
                        currentInvoice.TotalCost -= itemToAdd.Cost;
                    }

                    labelInvoiceTotalCostDisplay.Content = currentInvoice.TotalCost.ToString("C"); ;

                }
                // If they are not in edit mode, show error
                else if (canEditInvoice == false)
                {
                    ShowError("Your are not currently editing an invoice");
                }
                // If no item is selected, show error
                else
                {
                    ShowError("Select an item to remove");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item " + ex.Message);
            }
        }

        /// <summary>
        /// Sets currentItem to the selected item from the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void comboBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                currentItem = (clsItem)comboBoxItems.SelectedItem;
                if (currentItem != null)
                {
                    labelItemCostDisplay.Content = $"${currentItem.Cost.ToString("F2")}";
                }
                else
                {
                    // Clears if the item was selected, then it got deleted/edited in price menu.
                    labelItemCostDisplay.Content = "$0.00";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error changing item seleciton from combo box " + ex.Message);
            }
        }

        /// <summary>
        /// Sets the main window to edit mode, allowing items to be added/removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonEditInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentInvoice != null)
                {
                    labelEditMode.Visibility = Visibility.Visible;
                    HideAllErrors();
                }
                canEditInvoice = true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error trying to enter edit mode " + ex.Message);
            }
        }

        /// <summary>
        /// Saves the current invoice to the database, creating it if it doesn't exist yet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonSaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // If there is no current invoice, do nothing
                if (currentInvoice == null)
                {
                    return;
                }
                // If the invoice hasn't been created yet, create it in the database
                if (!invoiceCreated)
                {
                    currentInvoice.InvoiceNumber = mainLogic.CreateInvoice();
                    invoiceCreated = true;
                }
                // Exit edit mode
                labelEditMode.Visibility = Visibility.Hidden;
                canEditInvoice = false;
                // Save the invoice and update the total
                mainLogic.SaveInvoice(currentInvoice.InvoiceNumber);
                mainLogic.UpdateCurrentInvoiceTotal(currentInvoice.InvoiceNumber);
                // Reload the invoice from the database to ensure all data is up to date
                currentInvoice = mainLogic.SelectInvoice(currentInvoice.InvoiceNumber);
                // Display the updated invoice
                displayInvoice();
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving invoice " + ex.Message);
            }
        }

        /// <summary>
        /// Opens the create invoice window as a modal dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void buttonCreateNewInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create the invoice window, center it on the main window, and set the interface
                var createInvoiceWin = new wndCreateInvoice();
                createInvoiceWin.CreateInvoiceInterface = this;
                createInvoiceWin.Owner = this;
                createInvoiceWin.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                // Show the dialog
                createInvoiceWin.ShowDialog();
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening create invoice window " + ex.Message);
            }
        }

        // Callback from the create invoice window when confirmed. User selected date is passed in.
        public void onConfirmCreateInvoice(DateOnly date)
        {
            try
            {
                // Set up the main window for the new invoice
                HideAllErrors();
                buttonEditInvoice.IsEnabled = true;
                buttonSaveInvoice.IsEnabled = true;
                canEditInvoice = false;
                labelEditMode.Visibility = Visibility.Hidden;
                mainLogic.ClearItemsList();
                invoiceCreated = false;
                // Store the new invoice(not yet in db) and display 
                clsInvoice tempInvoice = mainLogic.StoreInvoice(date);
                currentInvoice = tempInvoice;
                Debug.WriteLine("Called interface");
                displayInvoice();
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating new invoice " + ex.Message);

            }
        }
        /// <summary>
        /// Updates the ComboBox selection when an item is selected in the data grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="Exception"></exception>
        private void dgInvoice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgInvoice.SelectedItem is clsItem selectedItem)
                {
                    // Find the matching item in the ComboBox
                    var matchingItem = allAvailableItems.FirstOrDefault(item =>
                        item.Code == selectedItem.Code);

                    if (matchingItem != null)
                    {
                        comboBoxItems.SelectedItem = matchingItem;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error changing item seleciton from data grid " + ex.Message);
            }
        }
    }
}