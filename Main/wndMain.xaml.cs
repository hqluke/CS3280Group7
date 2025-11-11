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
        ObservableCollection<clsItem> allAvailableItems;
        clsItem currentItem;

        public wndMain()
        {
            InitializeComponent();
            //start();
        }

        /// <summary>
        /// Binds the Item's comboBox to all the currently available items.
        /// </summary>
        //private void start()
        //{

        //    allAvailableItems = Items.clsItemsLogic.getAllItems();
        //    comboBoxItems.ItemsSource = allAvailableItems;
        //    comboBoxItems.DisplayMemberPath = "Description";
        //}

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
        // allAvailableItems = clsItemsLogic.getAllItems();
        // comboBoxItems.ItemsSource = allAvailableItems;

        //bool hasBeenChanged = itemsWindow.getHasBeenChanged();

        //MOVE LOGIC TO MainLogic.cs\
        //update the currentInvoice.ItemList if an item is removed from the collection
        //if (hasbeenChanged == true) {
        //for (int i = currentInvoice.ItemsList.Count - 1; i >= 0; i--)
        //{
        //    var item = currentInvoice.ItemsList[i];
        //    if (!allAvailableItems.Contains(item))
        //    {
        //        currentInvoice.ItemsList.RemoveAt(i);
        //    }
        //}
        //}
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

        //    clsInvoice newInvoice = searchWindow.getSelectedInvoice();
        //    if (currentInvoice == searchWindow.getSelectedInvoice() || newInvoice == null)
        //    {
        //        return;
        //    }
        //    currentInvoice = newInvoice;
        //    dgInvoice.ItemsSource = currentInvoice.ItemsList;
        //    dgInvoice.IsReadOnly = false;
        //    labelInvoiceNumberDisplay.Content = currentInvoice.InvoiceNumber.ToString();
        //    labelInvoiceDateDisplay.Content = currentInvoice.InvoiceDate.ToString();
        //    //example display: $70.00
        //    labelInvoiceTotalCostDisplay.Content = "$" + currentInvoice.TotalCost.ToString("F2");

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
    }
}