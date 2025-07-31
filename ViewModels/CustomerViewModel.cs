using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfMvvmCustomerApp.ViewModels
{
    public class CustomerViewModel : INotifyPropertyChanged
    {
        private string name;
        private string age;
        private Customer selectedCustomer;

        public ObservableCollection<Customer> Customers { get; set; } = new();

        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged(); }
        }

        public string Age
        {
            get => age;
            set { age = value; OnPropertyChanged(); }
        }

        public Customer SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                selectedCustomer = value;
                if (selectedCustomer != null)
                {
                    Name = selectedCustomer.Name;
                    Age = selectedCustomer.Age.ToString();
                }
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }

        public CustomerViewModel()
        {
            AddCommand = new RelayCommand(AddCustomer);
            UpdateCommand = new RelayCommand(UpdateCustomer);
            DeleteCommand = new RelayCommand(DeleteCustomer);
            SearchCommand = new RelayCommand(SearchCustomer);

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            Customers.Clear();
            foreach (var c in db.Customers)
                Customers.Add(c);
        }

        private void AddCustomer(object obj)
        {
            if (!string.IsNullOrWhiteSpace(Name) && int.TryParse(Age, out int ageValue))
            {
                using var db = new AppDbContext();
                var customer = new Customer { Name = Name, Age = ageValue };
                db.Customers.Add(customer);
                db.SaveChanges();
                Customers.Add(customer);
                Name = string.Empty;
                Age = string.Empty;
            }
        }

        private void UpdateCustomer(object obj)
        {
            if (SelectedCustomer != null && int.TryParse(Age, out int ageValue))
            {
                using var db = new AppDbContext();
                var customer = db.Customers.Find(SelectedCustomer.Id);
                if (customer != null)
                {
                    customer.Name = Name;
                    customer.Age = ageValue;
                    db.SaveChanges();

                    SelectedCustomer.Name = Name;
                    SelectedCustomer.Age = ageValue;
                }
            }
        }

        private void DeleteCustomer(object obj)
        {
            if (SelectedCustomer != null)
            {
                using var db = new AppDbContext();
                var customer = db.Customers.Find(SelectedCustomer.Id);
                if (customer != null)
                {
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    Customers.Remove(SelectedCustomer);
                }
            }
        }

        private void SearchCustomer(object obj)
        {
            using var db = new AppDbContext();
            Customers.Clear();
            var results = db.Customers
                .Where(c => string.IsNullOrWhiteSpace(Name) || c.Name.Contains(Name))
                .ToList();
            foreach (var c in results)
                Customers.Add(c);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
