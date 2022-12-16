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
    public class SupplierViewModel : BaseViewModel
    {
        private ObservableCollection<Supplier> _List;
        public ObservableCollection<Supplier> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private Supplier _SelectedItem;
        public Supplier SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    Phone = SelectedItem.Phone;
                    Email = SelectedItem.Email;
                    Address = SelectedItem.Address;
                    MoreInfo = SelectedItem.MoreInfo;
                    ContractDate = SelectedItem.ContractDate;
                }
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _Phone;
        public string Phone { get => _Phone; set { _Phone = value; OnPropertyChanged(); } }

        private string _Address;
        public string Address { get => _Address; set { _Address = value; OnPropertyChanged(); } }

        private string _Email;
        public string Email { get => _Email; set { _Email = value; OnPropertyChanged(); } }

        private string _MoreInfo;
        public string MoreInfo { get => _MoreInfo; set { _MoreInfo = value; OnPropertyChanged(); } }

        private DateTime? _ContractDate;
        public DateTime? ContractDate { get => _ContractDate; set { _ContractDate = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Output> _ListOutput;
        public ObservableCollection<Model.Output> ListOutput { get => _ListOutput; set { _ListOutput = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.InputInfo> _ListInputInfo;
        public ObservableCollection<Model.InputInfo> ListInputInfo { get => _ListInputInfo; set { _ListInputInfo = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Object> _ListObject;
        public ObservableCollection<Model.Object> ListObject { get => _ListObject; set { _ListObject = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public SupplierViewModel()
        {
            List = new ObservableCollection<Supplier>(DataProvider.Ins.DB.Supplier);

            ListOutput = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            ListOutputInfo = new ObservableCollection<Model.OutputInfo>(DataProvider.Ins.DB.OutputInfo);
            ListInputInfo = new ObservableCollection<Model.InputInfo>(DataProvider.Ins.DB.InputInfo);   

            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;

            }, (p) =>

            {
                var Supplier = new Supplier() { DisplayName = DisplayName, Phone = Phone, Address = Address, Email = Email, ContractDate = ContractDate, MoreInfo = MoreInfo };

                DataProvider.Ins.DB.Supplier.Add(Supplier);
                DataProvider.Ins.DB.SaveChanges();

                List.Add(Supplier);
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Supplier.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>

            {
                var Supplier = DataProvider.Ins.DB.Supplier.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Supplier.DisplayName = DisplayName;
                Supplier.Phone = Phone; 
                Supplier.Address = Address; 
                Supplier.Email = Email; 
                Supplier.ContractDate = ContractDate; 
                Supplier.MoreInfo = MoreInfo;
                DataProvider.Ins.DB.SaveChanges();

                SelectedItem.DisplayName = DisplayName;
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Supplier.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>

            {
                var Supplier = DataProvider.Ins.DB.Supplier.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var Object = DataProvider.Ins.DB.Object.Where(x => x.IdSupplier == SelectedItem.Id).ToList();
                foreach (var item in Object)
                {
                    if (item != null)
                    {
                        var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdObject == item.Id).ToList();
                        if (collection != null)
                        {
                            foreach (var i in collection)
                            {
                                if (i != null)
                                {
                                    DataProvider.Ins.DB.OutputInfo.Remove(i);
                                    ListOutputInfo.Remove(i);

                                }

                                ICollectionView view1 = CollectionViewSource.GetDefaultView(ListOutputInfo);
                                view1.Refresh();
                            }

                        }
                        var col = DataProvider.Ins.DB.InputInfo.Where(x => x.IdObject == item.Id).ToList();
                        foreach (var j in col)
                        {
                            if (j != null)
                            {
                                DataProvider.Ins.DB.InputInfo.Remove(j);
                                ListInputInfo.Remove(j);
                            }

                            ICollectionView view1 = CollectionViewSource.GetDefaultView(ListInputInfo);
                            view1.Refresh();
                        }

                        DataProvider.Ins.DB.Object.Remove(item);
                        // ListObject.Remove(item);
                    }
                }

                DataProvider.Ins.DB.Supplier.Remove(Supplier);
                List.Remove(Supplier);
                DataProvider.Ins.DB.SaveChanges();
                ICollectionView view = CollectionViewSource.GetDefaultView(List);
                view.Refresh();
            });
        }
    }
}
