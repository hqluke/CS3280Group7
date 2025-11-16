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
using GroupProject.Items;
using GroupProject.Search;
using GroupProject.Common;
using System.Collections.ObjectModel;
namespace GroupProject.Main
    
    
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class wndMain : Window
    {


        clsInvoice currentInvoice;
        List<clsItem> allAvailableItems;
        clsItem currentItem;
        clsItemsLogic itemsLogic;
        clsMainLogic mainLogic;

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

            allAvailableItems = itemsLogic.GetAllItems();
            comboBoxItems.ItemsSource = allAvailableItems;
            comboBoxItems.DisplayMemberPath = "Description";
        }

        /// <summary>
        /// Creates a new wndItems window that handles all the Items in the data base.
        /// Main window data grid will update if item's in the collection are deleted.
        /// Main window Item ComboBox will update if a new item is added.
        /// </summary>
        private void menuEditItemsClicked(object sender, RoutedEventArgs e)
        {
            wndItems itemsWindow = new wndItems();
            itemsWindow.ShowDialog();

            //if clsItemsLogic returns an ObservableCollection, shouldn't have to update allAvailableItems
            //otherwise refresh the list after the window closes:
            //allAvailableItems = itemsLogic.GetAllItems();
            //comboBoxItems.ItemsSource = allAvailableItems;

            //bool hasBeenChanged = itemsWindow.HasItemsBeenChanged;

        //    MOVE LOGIC TO MainLogic.cs\
        //update the currentInvoice.ItemList if an item is removed from the collection
            //if (hasBeenChanged == true)
            //    {
            //        for (int i = currentInvoice.ItemsList.Count - 1; i >= 0; i--)
            //        {
            //            var item = currentInvoice.ItemsList[i];
            //            if (!allAvailableItems.Contains(item))
            //            {
            //                currentInvoice.ItemsList.RemoveAt(i);
            //            }
            //        }
            //    }
        }

        ///<summary>
        ///Opens a search window that allows a user to select an invoice.
        ///Gets the selected invoice from the search window. If it's a new invoice object,
        ///it binds the main window's data grid to the selected invoice 
        /// </summary>
        private void menuSearchClicked(object sender, RoutedEventArgs e)
        {
            wndSearch searchWindow = new wndSearch();
            searchWindow.ShowDialog();

            clsInvoice newInvoice = searchWindow.getSelectedInvoice();
            if (currentInvoice == newInvoice || newInvoice == null)
            {

                return;
            }

            currentInvoice = newInvoice;

            // Load the line items for this invoice
            try
            {
                List<clsItem> invoiceItems = mainLogic.GetInvoiceItems(currentInvoice.InvoiceNumber);

                // Clear existing items and add the loaded ones
                currentInvoice.ItemsList.Clear();
                foreach (clsItem item in invoiceItems)
                {
                    currentInvoice.AddItemToInvoice(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoice items: " + ex.Message, "Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

            dgInvoice.ItemsSource = currentInvoice.ItemsList;
            dgInvoice.IsReadOnly = true;
            labelInvoiceNumberDisplay.Content = currentInvoice.InvoiceNumber.ToString();
            labelInvoiceDateDisplay.Content = currentInvoice.InvoiceDate.ToString();
            labelInvoiceTotalCostDisplay.Content = "$" + currentInvoice.TotalCost.ToString("F2");
        }

        //MOVE LOGIC TO MainLogic.cs\
        private void buttonAddItem_Click(object sender, RoutedEventArgs e)
        {
        //    // if the selected item is a clsItem object, reference the object as itemToAdd
        //    if (comboBoxItems.SelectedItem is clsItem itemToAdd)
        //    {
        //        //add to db
        //        //add the object
        //        currentInvoice.AddItemToInvoice(itemToAdd);

        //    }

        }

        private void buttonRemoveItem_Click(object sender, RoutedEventArgs e)
        {

        //    // if the selected item is a clsItem object, reference the object as itemToAdd
        //    if (comboBoxItems.SelectedItem is clsItem itemToAdd)
        //    {
        //        //remove from db
        //        //remove object
        //        currentInvoice.RemoveItemFromInvoice(itemToAdd);

        //    }
        }

        private void comboBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsItem selectedItem = (clsItem)comboBoxItems.SelectedItem;
            labelItemCostDisplay.Content = $"${selectedItem.Cost.ToString()}";
        }

        private void buttonEditInvoice_Click(object sender, RoutedEventArgs e)
        {
            dgInvoice.IsReadOnly = false;
        }
    }
}