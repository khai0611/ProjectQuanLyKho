using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data;
using System.Xml;

namespace QuanLyKho.ViewModel
{
    class OutputViewModel : BaseViewModel
    {

        private ObservableCollection<TonKho> _InventoryList;
        public ObservableCollection<TonKho> InventoryList { get => _InventoryList; set { _InventoryList = value; OnPropertyChanged(); } }

        private ThongKe _ThongKe;
        public ThongKe ThongKe { get => _ThongKe; set { _ThongKe = value; OnPropertyChanged(); } }

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

        private ReportViewModel _Report;
        public ReportViewModel Report { get => _Report; set { _Report = value; OnPropertyChanged(); } }

        private IDocumentPaginatorSource _fixedDocumentSequence;

        public IDocumentPaginatorSource FixedDocumentSequence
        {
            get { return _fixedDocumentSequence; }
            set
            {
                if (_fixedDocumentSequence == value) return;

                _fixedDocumentSequence = value;
                OnPropertyChanged("FixedDocumentSequence");
            }
        }

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
                    SelectedCustomer = SelectedItem.Customer;
                    DateOutput = SelectedItem.DateOutput;
                    SelectedUsers = SelectedItem.Users;
                    //Count = SelectedItem.Count;
                    //SelectedInputInfo = SelectedItem.InputInfo;
                    Id = SelectedItem.Id;
                    Status = SelectedItem.Status;
                }
            }
        }

        private Model.OutputInfo _SelectedOutputInfo;
        public Model.OutputInfo SelectedOutputInfo
        {
            get => _SelectedOutputInfo; set
            {
                _SelectedOutputInfo = value;
                OnPropertyChanged();
                if (SelectedOutputInfo != null)
                {
                    SelectedObject = SelectedOutputInfo.Object;
                    Count = SelectedOutputInfo.Count;
                    Status = SelectedOutputInfo.Status;
                    SelectedCustomer = SelectedItem.Customer;
                }
            }
        }

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

        private Model.Promotion _SelectedPromotion;
        public Model.Promotion SelectedPromotion { get => _SelectedPromotion; set { _SelectedPromotion = value; OnPropertyChanged(); } }

        private DateTime? _DateOutput;
        public DateTime? DateOutput { get => _DateOutput; set { _DateOutput = value; OnPropertyChanged(); } }

        private string _Id;
        public string Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private double? _Sum;
        public double? Sum { get => _Sum; set { _Sum = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        public ICommand SelectedItemListViewChangedCommand { get; set; }
        public ICommand SelectedOutputInfoListViewChangedCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand AddOuputInfoCommand { get; set; }
        public ICommand EditOuputInfoCommand { get; set; }
        public ICommand DeleteOuputInfoCommand { get; set; }
        public ICommand PrintCommand { get; set; }


        #endregion

        public OutputViewModel()
        {
            ListOutput = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            ListOutputInfo = new ObservableCollection<OutputInfo>(DataProvider.Ins.DB.OutputInfo);
            ListInputInfo = new ObservableCollection<InputInfo>(DataProvider.Ins.DB.InputInfo);
            Users = new ObservableCollection<Model.Users>(DataProvider.Ins.DB.Users);
            Customer = new ObservableCollection<Model.Customer>(DataProvider.Ins.DB.Customer);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);

            LoadTotalPrice();

            // Nhấn vào danh sách hóa đơn => chi tiết hóa đơn thay đổi theo
            SelectedItemListViewChangedCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                ListOutputInfo.Clear();

                if (SelectedItem != null)
                {
                    var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdOutput == SelectedItem.Id).ToList();
                    foreach (var item in collection)
                    {
                        ListOutputInfo.Add(item);
                    }
                }

                var Output = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();

                if (SelectedItem != null)
                {
                    // Tính giá tiền nhập tương ứng với danh sách từng sản phẩm
                    var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdOutput == SelectedItem.Id).ToList();
                    foreach (var item in collection)
                    {

                        var InputInfo = DataProvider.Ins.DB.InputInfo.Where(x => x.IdInput == item.Id).SingleOrDefault();
                        if (InputInfo != null)
                        {
                            Output.Total += item.Count * InputInfo.OutputPrice;
                        }
                        ListOutputInfo.Add(item);
                        DataProvider.Ins.DB.SaveChanges();
                    }
                }

                //DataProvider.Ins.DB.Output.Add(ListOutputInfo);
                DataProvider.Ins.DB.SaveChanges();

                ICollectionView view = CollectionViewSource.GetDefaultView(ListOutput);
                view.Refresh();
            });

            //SelectedOutputInfoListViewChangedCommand = new RelayCommand<object>((p) => true, (p) =>
            //{
            //    //SelectedObject = SelectedOutputInfo.Object;
            //    Count = SelectedOutputInfo.Count;
            //    Status = SelectedOutputInfo.Status;
            //});

            #region OutputCommand
            AddCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedUsers == null)
                    return false;
                return true;

            }, (p) =>

            {
                if (SelectedCustomer != null)
                {
                    var Output = new Model.Output() { IdCustomer = SelectedCustomer.Id, IdUser = SelectedUsers.Id, DateOutput = DateOutput, Id = Guid.NewGuid().ToString() };

                    DataProvider.Ins.DB.Output.Add(Output);
                    DataProvider.Ins.DB.SaveChanges();
                    ListOutput.Add(Output);
                }
                else
                {
                    var Customer = new Model.Customer() { DisplayName = SelectedCustomer.DisplayName.ToString(), Address = SelectedCustomer.Address, Phone = SelectedCustomer.Phone };
                    var Output = new Model.Output() { IdCustomer = Customer.Id, IdUser = SelectedUsers.Id, Id = Guid.NewGuid().ToString() };

                    DataProvider.Ins.DB.Output.Add(Output);
                    DataProvider.Ins.DB.SaveChanges();
                    ListOutput.Add(Output);
                }

                //--//

            });

            EditCommand = new RelayCommand<Output>((p) =>
            {
                if (SelectedUsers == null || SelectedCustomer == null || SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var Output = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Output.IdCustomer = SelectedCustomer.Id;
                Output.IdUser = SelectedUsers.Id;
                //Output.IdPromotion = SelectedPromotion.Id;
                DateOutput = DateOutput;
                Status = Status;
                DataProvider.Ins.DB.SaveChanges();

                LoadTotalPrice();

                //ICollectionView view2 = CollectionViewSource.GetDefaultView(ListOutput);
                //view2.Refresh();
            });

            DeleteCommand = new RelayCommand<Output>((p) =>
            {
                if (SelectedUsers == null || SelectedCustomer == null || SelectedItem == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var Output = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdOutput == SelectedItem.Id).ToList();

                foreach (var item in collection)
                {
                    DataProvider.Ins.DB.OutputInfo.Remove(item);

                }
                ListOutputInfo.Clear();

                DataProvider.Ins.DB.Output.Remove(Output);
                ListOutput.Remove(Output);
                DataProvider.Ins.DB.SaveChanges();

                ICollectionView view2 = CollectionViewSource.GetDefaultView(ListOutputInfo);
                view2.Refresh();
            });
            #endregion

            #region OutputInfoCommand
            AddOuputInfoCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedObject == null || Count == null)
                    return false;
                return true;

            }, (p) => {

                InventoryList = new ObservableCollection<TonKho>();
                ThongKe = new ThongKe();
                var objectList = DataProvider.Ins.DB.Object.Where(x => x.DisplayName == SelectedObject.DisplayName).ToList();

                int luongNhap = 0;
                int luongXuat = 0;

                int i = 1;
                foreach (var item in objectList)
                {
                    var inputList = DataProvider.Ins.DB.InputInfo.Where(x => x.IdObject == item.Id);
                    var outputList = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdObject == item.Id);

                    int sumInput = 0;
                    int sumOutput = 0;


                    if (inputList != null && inputList.Count() > 0)
                    {
                        sumInput = (int)inputList.Sum(x => x.Count);

                        luongNhap += sumInput;
                    }

                    if (outputList != null && outputList.Count() > 0)
                    {
                        sumOutput = (int)outputList.Sum(x => x.Count);
                        luongXuat += sumOutput;
                    }
                }
                ThongKe.LuongTon = luongNhap - luongXuat;
                //var Output= DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var Object = DataProvider.Ins.DB.Object.Where(x => x.Id == SelectedObject.Id).SingleOrDefault();
                var InputInfolist = DataProvider.Ins.DB.InputInfo.Where(x => x.IdObject == Object.Id).First();

                if (ThongKe.LuongTon < Count)
                {
                    MessageBox.Show("Hàng trong kho đã hết");
                }
                else
                {

                    var OutputInfo = new Model.OutputInfo()
                    {
                        IdOutput = SelectedItem.Id,
                        IdObject = SelectedObject.Id,
                        IdInputInfo = InputInfolist.Id,
                        Count = Count,
                        Status = Status,
                        //Status = SelectedOutputInfo.Status,
                        SumPrice = Count * InputInfolist.OutputPrice,
                        Id = Guid.NewGuid().ToString()
                    };

                    DataProvider.Ins.DB.OutputInfo.Add(OutputInfo);
                    DataProvider.Ins.DB.SaveChanges();

                    ListOutputInfo.Add(OutputInfo);
                    //ICollectionView view2 = CollectionViewSource.GetDefaultView(ListOutput);
                    //view2.Refresh();
                }
            });

            EditOuputInfoCommand = new RelayCommand<Output>((p) =>
            {
                if (SelectedObject == null || SelectedCustomer == null || SelectedOutputInfo == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var OutputInfo = DataProvider.Ins.DB.OutputInfo.Where(x => x.Id == SelectedOutputInfo.Id).SingleOrDefault();

                //MessageBox.Show(OutputInfo.Id);
                //var Output = DataProvider.Ins.DB.Output.Where(x => x.Id == OutputInfo.IdOutput).SingleOrDefault();


                var InputInfo = DataProvider.Ins.DB.InputInfo.Where(x => x.Id == SelectedObject.Id).SingleOrDefault();
                if (InputInfo == null)
                {
                    MessageBox.Show("Hàng trong kho đã hết");
                }
                else
                {
                    OutputInfo.IdObject = SelectedObject.Id;
                    //OutputInfo.Count = Count;
                    OutputInfo.Status = Status;
                    OutputInfo.SumPrice = Count * InputInfo.OutputPrice;
                    //Output.IdPromotion = SelectedPromotion.Id;

                    LoadTotalPrice();
                    DataProvider.Ins.DB.SaveChanges();

                    ICollectionView view3 = CollectionViewSource.GetDefaultView(ListOutputInfo);
                    view3.Refresh();
                }

            });


            DeleteOuputInfoCommand = new RelayCommand<Output>((p) =>
            {
                if (SelectedObject == null || SelectedCustomer == null || SelectedOutputInfo == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Output.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var OutputInfo = DataProvider.Ins.DB.OutputInfo.Where(x => x.Id == SelectedOutputInfo.Id).SingleOrDefault();

                DataProvider.Ins.DB.OutputInfo.Remove(OutputInfo);
                ListOutputInfo.Remove(OutputInfo);
                LoadTotalPrice();
                DataProvider.Ins.DB.SaveChanges();
                //ICollectionView view1 = CollectionViewSource.GetDefaultView(ListOutputInfo);
                //view1.Refresh();

            });
            #endregion

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
                // rpw.DocViewer.Document = rpt;
                //ICollectionView view = CollectionViewSource.GetDefaultView(ListOutputInfo);
                //view.Refresh();

            });
        }

        // Hàm tính tổng hóa đơn
        void LoadTotalPrice()
        {
            var outputList = DataProvider.Ins.DB.Output;

            int i = 1;

            foreach (var item in outputList)
            {
                var outputInfoList = DataProvider.Ins.DB.OutputInfo.Where(p => p.IdOutput == item.Id);
                double tongtien = 0;

                if (outputInfoList != null && outputInfoList.Count() > 0)
                {
                    tongtien = (double)outputInfoList.Sum(p => p.SumPrice);
                }
                item.Total = tongtien;

                i++;
            }

            DataProvider.Ins.DB.SaveChanges();
        }

    }
}