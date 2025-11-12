using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Common
{
    public class clsInvoice : INotifyPropertyChanged
    {
        /// <summary>
        /// Holds the invoice number
        /// </summary>
        private int invoiceNumber;

        /// <summary>
        /// Holds the invoice date
        /// </summary>
        private DateOnly invoiceDate;

        /// <summary>
        /// Holds the total cost of the invoice
        /// </summary>
        private double totalCost;

        /// <summary>
        /// Holds the list of items in the invoice. Add the items to ItemsList during invoice creation
        /// </summary>
        private ObservableCollection<clsItem> itemsList = new ObservableCollection<clsItem>();

        /// <summary>
        /// Creates an instance of clsInvoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="totalCost"></param>
        /// <param name="Items"></param>
        public clsInvoice(int invoiceNumber, DateOnly invoiceDate, double totalCost)
        {
            this.invoiceNumber = invoiceNumber;
            this.invoiceDate = invoiceDate;
            this.totalCost = totalCost;
        }
        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        public int InvoiceNumber {
            get {
                return invoiceNumber; 
            }  
            set {
                invoiceNumber = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InvoiceNumber"));
                }
            } 
        }

        /// <summary>
        /// Gets or sets the invoice date
        /// </summary>
        public DateOnly InvoiceDate { 
            get {
                return invoiceDate; 
            } 
            set {
                invoiceDate = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InvoiceDate"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the total cost
        /// </summary>
        public double TotalCost { 
            get { 
                return totalCost; 
            } 
            set { 
                totalCost = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TotalCost"));
                }
            } 
        }

        /// <summary>
        /// Returns the list of items in the invoice.
        /// </summary>
        public ObservableCollection<clsItem> ItemsList { get { return itemsList; } }

        /// <summary>
        /// Used to add an item to the invoice's item list.
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToInvoice(clsItem item)
        {
            itemsList.Add(item);
        }

        /// <summary>
        /// Used to remove an item from the invoice's item list.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItemFromInvoice(clsItem item)
        {
            itemsList.Remove(item);
        }

        /// <summary>
        /// Shows changes to the UI when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
