using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Scanner
{
    public class CustomerViewModel: INotifyPropertyChanged
    {
        private ICollectionView _customerView;
        private string _filterString;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ICollectionView Customers
        {
            get { return _customerView; }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                _customerView.Refresh();
            }
        }



        public CustomerViewModel()
        {
            IList<Customer> customers = new List<Customer>();

            customers.Add(new Customer { Id = 1,Name="c1"});
            customers.Add(new Customer { Id = 2,Name="c2"});
            customers.Add(new Customer { Id = 3,Name="c3"});
            customers.Add(new Customer { Id = 4,Name="c4"});
            
            _customerView = CollectionViewSource.GetDefaultView(customers);
            _customerView.Filter = CustomerFilter;

        }

        private bool CustomerFilter(object item)
        {
            Customer customer = item as Customer;
            if (string.IsNullOrEmpty(_filterString)) 
                return true;

            return customer.Name.Contains(_filterString);
        }

    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id + " : " + Name;
        }

    }


    }
