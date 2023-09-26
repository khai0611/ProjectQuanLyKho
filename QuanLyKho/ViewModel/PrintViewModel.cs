using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    class PrintViewModel : BaseViewModel
    {
        private ObservableCollection<Model.Output> _ListOutput;
        public ObservableCollection<Model.Output> ListOutput { get => _ListOutput; set { _ListOutput = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.InputInfo> _ListInputInfo;
        public ObservableCollection<Model.InputInfo> ListInputInfo { get => _ListInputInfo; set { _ListInputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Customer> _Customer;
        public ObservableCollection<Model.Customer> Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Users> _Users;
        public ObservableCollection<Model.Users> Users { get => _Users; set { _Users = value; OnPropertyChanged(); } }


        private Model.Output _SelectedItem;
        public Model.Output SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    //SelectedCustomer = SelectedItem.Customer;
                    DateOutput = SelectedItem.DateOutput;
                    SelectedUsers = SelectedItem.Users;
                    //Count = SelectedItem.Count;
                    //SelectedInputInfo = SelectedItem.InputInfo;
                    Id = SelectedItem.Id;
                    Status = SelectedItem.Status;
                }
            }
        }

        //private Model.OutputInfo _SelectedOutputInfo;
        //public Model.OutputInfo SelectedOutputInfo
        //{
        //    get => _SelectedOutputInfo; set
        //    {
        //        _SelectedOutputInfo = value;
        //        OnPropertyChanged();
        //        if (SelectedOutputInfo != null)
        //        {
        //            SelectedObject = SelectedOutputInfo.Object;
        //            Count = SelectedOutputInfo.Count;
        //            Status = SelectedOutputInfo.Status;
        //            //SelectedCustomer = SelectedItem.Customer;

        //        }
        //    }
        //}

        private Model.Customer _SelectedCustomer;
        public Model.Customer SelectedCustomer
        {
            get => _SelectedCustomer;
            set
            {
                _SelectedCustomer = value;
                OnPropertyChanged();
                if (SelectedCustomer != null && SelectedItem != null)
                {
                    SelectedItem.IdCustomer = SelectedCustomer.Id;
                }

            }
        }

        private Model.Object _SelectedObject;
        public Model.Object SelectedObject { get => _SelectedObject; set { _SelectedObject = value; OnPropertyChanged(); } }

        #region List

        private Model.InputInfo _SelectedInputInfo;
        public Model.InputInfo SelectedInputInfo { get => _SelectedInputInfo; set { _SelectedInputInfo = value; OnPropertyChanged(); } }

        private Model.Users _SelectedUsers;
        public Model.Users SelectedUsers { get => _SelectedUsers; set { _SelectedUsers = value; OnPropertyChanged(); } }

        private DateTime? _DateOutput;
        public DateTime? DateOutput { get => _DateOutput; set { _DateOutput = value; OnPropertyChanged(); } }

        private string _Id;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private double? _Sum;
        public double? Sum { get => _Sum; set { _Sum = value; OnPropertyChanged(); } }

        private double? _TotalPrice;
        public double? TotalPrice { get => _TotalPrice; set { _TotalPrice = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        public ICommand SelectedItemListViewChangedCommand { get; set; }
        public ICommand SelectedOutputInfoListViewChangedCommand { get; set; }
        public ICommand PrintCommand { get; set; }


        #endregion

        public PrintViewModel()
        {
            ListOutput = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            ListOutputInfo = new ObservableCollection<OutputInfo>(DataProvider.Ins.DB.OutputInfo);
            ListInputInfo = new ObservableCollection<InputInfo>(DataProvider.Ins.DB.InputInfo);
            Users = new ObservableCollection<Model.Users>(DataProvider.Ins.DB.Users);
            Customer = new ObservableCollection<Model.Customer>(DataProvider.Ins.DB.Customer);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);

            ICollectionView viewOutputInfo = CollectionViewSource.GetDefaultView(ListOutputInfo);

            ICollectionView viewOutput = CollectionViewSource.GetDefaultView(ListOutput);
        
            //LoadTotalPrice();

            PrintCommand = new RelayCommand<Output>((p) =>
            {
                if (SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                viewOutput.Refresh();
            });
        }
    }
}
