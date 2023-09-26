using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class CustomerViewModel : BaseViewModel
    {
        private ObservableCollection<Model.Customer> _List;
        public ObservableCollection<Model.Customer> List { get => _List; set { _List = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Output> _ListOutput;
        public ObservableCollection<Model.Output> ListOutput { get => _ListOutput; set { _ListOutput = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private Model.Customer _SelectedItem;
        public Model.Customer SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    Address = SelectedItem.Address;
                    Phone = SelectedItem.Phone;
                    Email = SelectedItem.Email;
                    MoreInfo = SelectedItem.MoreInfo;
                    ContractDate = SelectedItem.ContractDate;
                }
            }
        }

        private Model.Unit _SelectedUnit;
        public Model.Unit SelectedUnit
        {
            get => _SelectedUnit;
            set
            {
                _SelectedUnit = value;
                OnPropertyChanged();
            }
        }

        private Model.Supplier _SelectedSuplier;
        public Model.Supplier SelectedSuplier
        {
            get => _SelectedSuplier;
            set
            {
                _SelectedSuplier = value;
                OnPropertyChanged();
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _Address;
        public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }

        private string _Phone;
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }

        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }

        private string _MoreInfo;
        public string MoreInfo { get => _MoreInfo; set { _MoreInfo = value; OnPropertyChanged(); } }

        private DateTime? _ContractDate;
        public DateTime? ContractDate { get => _ContractDate; set { _ContractDate = value; OnPropertyChanged(); } }
        private Model.Customer _SelectedCustomer;
        public Model.Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                _SelectedCustomer = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public CustomerViewModel()
        {
            List = new ObservableCollection<Model.Customer>(DataProvider.Ins.DB.Customer);
            ListOutput = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            ListOutputInfo = new ObservableCollection<Model.OutputInfo>(DataProvider.Ins.DB.OutputInfo);

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (DisplayName == null || Address == null || Phone == null)
                    return false;
                
                return true;

            }, (p) =>
            {
                var Customer = new Model.Customer() { DisplayName = DisplayName, Phone = Phone, Address = Address,  Email = Email, MoreInfo = MoreInfo, ContractDate = ContractDate };

                DataProvider.Ins.DB.Customer.Add(Customer);
                DataProvider.Ins.DB.SaveChanges();

                ICollectionView view = CollectionViewSource.GetDefaultView(ListOutput);
                view.Refresh();

                List.Add(Customer);
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Customer.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var Customer = DataProvider.Ins.DB.Customer.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Customer.DisplayName = DisplayName;
                Customer.Address = Address;
                Customer.Phone = Phone;
                Customer.Email = Email;
                Customer.MoreInfo = MoreInfo;
                Customer.ContractDate = ContractDate;
                DataProvider.Ins.DB.SaveChanges();

                SelectedItem.DisplayName = DisplayName;
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Customer.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var Customer = DataProvider.Ins.DB.Customer.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var Output = DataProvider.Ins.DB.Output.Where(x => x.IdCustomer == SelectedItem.Id).ToList();
                foreach (var item in Output)
                {
                    var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdOutput == item.Id).ToList();
                    if (collection != null)
                    {
                        foreach (var i in collection)
                        {
                            if (i != null)
                            {
                                DataProvider.Ins.DB.OutputInfo.Remove(i);
                                ListOutputInfo.Remove(i);
                            }
                        }
                    }
                    DataProvider.Ins.DB.Output.Remove(item);
                    ListOutput.Remove(item);
                }

                DataProvider.Ins.DB.Customer.Remove(Customer);
                List.Remove(Customer);
                ICollectionView view = CollectionViewSource.GetDefaultView(List);
                view.Refresh();

                DataProvider.Ins.DB.SaveChanges();
            });
        }
    }
}
