using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ObservableCollection<TonKho> _TonKhoList; //tập hợp tương tự như List<T>, mô tả một tập hợp dữ liệu có thể thay đổi số lượng bằng các phương thức quen thuộc
        public ObservableCollection<TonKho> TonKhoList { get => _TonKhoList; set { _TonKhoList = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ThongKe _ThongKe;
        public ThongKe ThongKe { get => _ThongKe; set { _ThongKe = value; OnPropertyChanged(); } }

        private DateTime? _DateBeginInventory;
        public DateTime? DateBeginInventory { get => _DateBeginInventory; set { _DateBeginInventory = value; OnPropertyChanged(); } }

        private DateTime? _DateEndInventory;
        public DateTime? DateEndInventory { get => _DateEndInventory; set { _DateEndInventory = value; OnPropertyChanged(); } }

        private DateTime? _DateBegin;
        public DateTime? DateBegin { get => _DateBegin; set { _DateBegin = value; OnPropertyChanged(); } }

        private DateTime? _DateEnd;
        public DateTime? DateEnd { get => _DateEnd; set { _DateEnd = value; OnPropertyChanged(); } }

        private Model.Object _SelectedObject;
        public Model.Object SelectedObject { get => _SelectedObject; set { _SelectedObject = value; OnPropertyChanged(); } }

        private Model.Input _SelectedInput;
        public Model.Input SelectedInput
        {
            get => _SelectedInput;
            set
            {
                _SelectedInput = value;
                OnPropertyChanged();

            }
        }

        public bool Isloaded = false;
        public ICommand LoadedWindowCommand { get; set; }
        public ICommand UnitCommand { get; set; }
        public ICommand SupplierCommand { get; set; }
        public ICommand CustomerCommand { get; set; }
        public ICommand ObjectCommand { get; set; }
        public ICommand UserCommand { get; set; }
        public ICommand InputInfoCommand { get; set; }
        public ICommand OutputInfoCommand { get; set; }
        public ICommand OutputCommand { get; set; }
        public ICommand TonKho { get; set; }
        public ICommand TonKhoCommand { get; set; }
        public ICommand ThongKeCommand { get; set; }

        // mọi xử lý sẽ nằm trong đây
        public MainViewModel()
        {
            LoadedWindowCommand = new RelayCommand<Window>((p) => 
            { 
                return true; 
            }, (p) => 

            {
                Isloaded= true;
                if (p == null)
                    return;
                p.Hide();
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.ShowDialog();

                if (loginWindow.DataContext == null)
                    return;

                var loginVM = loginWindow.DataContext as LoginViewModel;

                if(loginVM.IsLogin)
                {
                    p.Show();
                    LoadTonKhoData();
                }
                else
                {
                    p.Close();
                }
                
            });

            UnitCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UnitWindow wd = new UnitWindow(); wd.ShowDialog(); });
            SupplierCommand = new RelayCommand<object>((p) => { return true; }, (p) => { SupplierWindow wd = new SupplierWindow(); wd.ShowDialog(); });
            CustomerCommand = new RelayCommand<object>((p) => { return true; }, (p) => { CustomerWindow wd = new CustomerWindow(); wd.ShowDialog(); });
            ObjectCommand = new RelayCommand<object>((p) => { return true; }, (p) => { ObjectWindow wd = new ObjectWindow(); wd.ShowDialog(); });
            UserCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UserWindow wd = new UserWindow(); wd.ShowDialog(); });
            InputInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { InputInfoWindow wd = new InputInfoWindow(); wd.ShowDialog(); });
            OutputInfoCommand = new RelayCommand<object>((p) => { return true; }, (p) => { OutputInfoWindow wd = new OutputInfoWindow(); wd.ShowDialog(); });
            OutputCommand = new RelayCommand<object>((p) => { return true; }, (p) => { OutputWindow wd = new OutputWindow(); wd.ShowDialog(); });
            
            TonKhoCommand = new RelayCommand<object>((x) =>
            {
                Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);
                if (DateBeginInventory == null || DateEndInventory == null) 
                    if(SelectedObject == null)

                    return false;

                return true;

            }, (x) =>

            {
                TonKhoList.Clear();
                LoadTonKhoData2();
            });

            ThongKeCommand = new RelayCommand<object>((x) =>
            {
                return true;

            }, (x) =>

            {
                TonKhoList.Clear();
                LoadTonKhoData();
            });
        }

        void LoadTonKhoData()
        {
            TonKhoList = new ObservableCollection<TonKho>();
            ThongKe = new ThongKe();
            var objectList = DataProvider.Ins.DB.Object;

            int luongNhap = 0;
            int luongXuat = 0;
            double tongTienNhap = 0;
            double tongTienXuat = 0;
            double tongTienTon = 0;
            double tongTienLai = 0;

            int i = 1;
            foreach (var item in objectList)
            {
                var inputList = DataProvider.Ins.DB.InputInfo.Where(p => p.IdObject == item.Id);
                var outputList = DataProvider.Ins.DB.OutputInfo.Where(p => p.IdObject == item.Id);

                int sumInput = 0;
                int sumOutput = 0;
                double tienNhap = 0;
                double tienXuat = 0;

                if (inputList != null && inputList.Count() > 0)
                {
                    sumInput = (int)inputList.Sum(p => p.Count);
                    tienNhap = (double)inputList.Sum(p => p.InputPrice);
                    tienXuat = (double)inputList.Sum(p => p.OutputPrice);
                    luongNhap += sumInput;
                }

                if (outputList != null && outputList.Count() > 0)
                {
                    sumOutput = (int)outputList.Sum(p => p.Count);
                    luongXuat += sumOutput;
                }

                tongTienTon += (sumInput - sumOutput) * tienNhap;
                tongTienLai += sumOutput * (tienXuat - tienNhap);
                tongTienNhap += sumInput * tienNhap;
                tongTienXuat += sumOutput * tienXuat;

                TonKho inventory = new TonKho();

                inventory.STT = i;
                inventory.CountInput = sumInput;
                inventory.CountOutput = sumOutput;
                inventory.CountInventory = sumInput - sumOutput;
                inventory.MoneyInput = sumInput * tienNhap;
                inventory.MoneyOutput = sumOutput * tienXuat;
                inventory.MoneyInventory = (sumInput - sumOutput) * tienNhap;
                inventory.MoneyIncome = sumOutput * (tienXuat - tienNhap);
                inventory.Object = item;

                TonKhoList.Add(inventory);

                i++;
            }

            ThongKe.LuongNhap = luongNhap;
            ThongKe.LuongXuat = luongXuat;
            ThongKe.GiaNhap = tongTienNhap;
            ThongKe.GiaXuat = tongTienXuat;
            ThongKe.LuongTon = luongNhap - luongXuat;
            ThongKe.GiaTon = tongTienTon;
            ThongKe.GiaLai = tongTienLai;
        }

        void LoadTonKhoData2()
        {
            TonKhoList = new ObservableCollection<TonKho>();
            ThongKe = new ThongKe();
            var objectList = DataProvider.Ins.DB.Object;

            // Tạo danh sách trong ngày duyệt - mảng Input & mảng Output

            // Lay danh sach nhap
            var dsNhap = DataProvider.Ins.DB.Input.AsQueryable();

            // loc danh sach nhap theo ngay
            if (DateBeginInventory != null)
            {
                dsNhap = DataProvider.Ins.DB.Input.Where(p => (p.DateInput >= DateBeginInventory) && (p.DateInput <= DateEndInventory));
            } 

            // lay danh sach xuat
            var dsXuat = DataProvider.Ins.DB.Output.AsQueryable();

            // loc danh sach xuat theo ngay
            if (DateBeginInventory != null)
            {
                dsXuat = DataProvider.Ins.DB.Output.Where(p => (p.DateOutput >= DateBeginInventory) && (p.DateOutput <= DateEndInventory));
            }

            int luongNhap = 0;
            int luongXuat = 0;

            double tongTienNhap = 0;
            double tongTienXuat = 0;
            double tongTienTon = 0;
            double tongTienLai = 0;

            // Duyệt mảng Object
            int i = 1;
            foreach (var item in objectList)
            {
                int sumInput = 0;
                int sumOutput = 0;

                double tienNhap = 0;
                double tienXuat = 0;

                TonKho inventory = new TonKho();
                inventory.STT = i;
                inventory.Object = item;

                bool inputFound = false;
                bool outputFound = false;

                // Tìm mảng InputInfo nằm trong mảng Object và mảng Input (dsNhap)
                foreach (var item1 in dsNhap)
                {
                    IQueryable<InputInfo> inputList;
                    if (SelectedObject != null)
                    {
                        inputList = DataProvider.Ins.DB.InputInfo.Where(p => (p.IdInput == item1.Id) && (p.IdObject == SelectedObject.Id) && (p.IdObject == item.Id));
                    } 
                    else
                    {
                        inputList = DataProvider.Ins.DB.InputInfo.Where(p => (p.IdInput == item1.Id) && (p.IdObject == item.Id));
                    }
                    
                    if (inputList != null && inputList.Count() > 0)
                    {
                        sumInput += (int)inputList.Sum(p => p.Count);
                        tienNhap = (double)inputList.Sum(p => p.InputPrice);
                        tienXuat = (double)inputList.Sum(p => p.OutputPrice);

                        inputFound = true;
                    }
                }

                luongNhap += sumInput;

                // Tìm mảng OutputInfo nằm trong mảng Object và mảng Output (dsXuat)
                foreach (var item2 in dsXuat)
                {
                    IQueryable<OutputInfo> outputList;
                    if (SelectedObject != null)
                    {
                        outputList = DataProvider.Ins.DB.OutputInfo.Where(p => (p.IdOutput == item2.Id) && (p.IdObject == SelectedObject.Id) && (p.IdObject == item.Id));
                    }
                    else
                    {
                        outputList = DataProvider.Ins.DB.OutputInfo.Where(p => (p.IdOutput == item2.Id)  && (p.IdObject == item.Id));
                    }
                    
                    if (outputList != null && outputList.Count() > 0)
                    {
                        sumOutput += (int)outputList.Sum(p => p.Count);

                        outputFound = true;
                    }
                }

                luongXuat += sumOutput;

                if (inputFound || outputFound)
                {
                    inventory.CountInput = sumInput;
                    inventory.CountOutput = sumOutput;
                    inventory.CountInventory = sumInput - sumOutput;
                    inventory.MoneyInput = sumInput * tienNhap;
                    inventory.MoneyOutput = sumOutput * tienXuat;
                    inventory.MoneyInventory = (sumInput - sumOutput) * tienNhap;
                    inventory.MoneyIncome = sumOutput * (tienXuat - tienNhap);

                    tongTienTon += (sumInput - sumOutput) * tienNhap;
                    tongTienLai += sumOutput * (tienXuat - tienNhap);
                    tongTienNhap += sumInput * tienNhap;
                    tongTienXuat += sumOutput * tienXuat;

                    TonKhoList.Add(inventory);
                    i++;
                }
            }

            ThongKe.LuongNhap = luongNhap;
            ThongKe.LuongXuat = luongXuat;
            ThongKe.GiaNhap = tongTienNhap;
            ThongKe.GiaXuat = tongTienXuat;
            ThongKe.LuongTon = luongNhap - luongXuat;
            ThongKe.GiaTon = tongTienTon;
            ThongKe.GiaLai = tongTienLai;
        }
    }
}

