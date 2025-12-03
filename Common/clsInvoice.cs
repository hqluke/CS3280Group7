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
            try
            {
                this.invoiceNumber = invoiceNumber;
                this.invoiceDate = invoiceDate;
                this.totalCost = totalCost;
            }
            catch (Exception ex)
            {
                throw new Exception("Error constructing invoice: " + ex.Message);
            }

        }
        /// <summary>
        /// Gets or sets the invoice number
        /// </summary>
        public int InvoiceNumber {
            get {
                return invoiceNumber; 
            }  
            set {
                try
                {
                    invoiceNumber = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("InvoiceNumber"));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error setting invoice number: " + ex.Message);
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
                try
                {
                    invoiceDate = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("InvoiceDate"));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error setting invoice date: " + ex.Message);
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
                try
                {
                    totalCost = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TotalCost"));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error setting invoice total cost: " + ex.Message);
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
            try
            {
                itemsList.Add(item);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding items to invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Used to remove an item from the invoice's item list.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItemFromInvoice(clsItem item)
        {
            try
            {
                itemsList.Remove(item);
            }
            catch (Exception ex)
            {
                throw new Exception("Error removing item from invoice: " + ex.Message);
            }
        }

        /// <summary>
        /// Shows changes to the UI when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

    }
}
