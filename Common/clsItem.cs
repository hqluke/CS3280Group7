using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupProject.Common
{
    public class clsItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Item code. Max length of 4 characters.
        /// </summary>
        private string code;

        /// <summary>
        /// Item description. Max length of 50 characters.
        /// </summary>
        private string description;

        /// <summary>
        /// Item cost 
        /// </summary>
        private double cost;

        /// <summary>
        /// Creates an instance of clsItem.
        /// <para>The code has a max length of 4 characters.</para>
        /// <para>The description has a max length of 50 characters.</para>
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="cost"></param>
        public clsItem(string code, string description, double cost)
        {
            if(code.Length > 4)
            {
                throw new ArgumentException("Item code cannot be longer than 4 characters.");
            }
            if(description.Length > 50)
            {
                throw new ArgumentException("Item description cannot be longer than 50 characters.");
            }

            this.code = code;
            this.description = description;
            this.cost = cost;
        }

        /// <summary>
        /// Gets or sets the item code.
        /// </summary>
        public string Code {
            get {
                return code; 
            } 
            set {
                code = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Code"));
                }
            } 
        }
        /// <summary>
        /// Gets or sets the item description.
        /// </summary>
        public string Description {
            get {
                return description; 
            } 
            set {
                description = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Description"));
                }
            } 
        }
        /// <summary>
        /// Gets or sets the item cost.
        /// </summary>
        public double Cost {
            get {
                return cost; 
            } 
            set {
                cost = value; 
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Cost"));
                }
            } 
        }

        public string Display { get { return Code + " - " + Description; } }



        /// <summary>
        /// Shows changes to the UI when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
