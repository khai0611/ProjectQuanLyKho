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
    public class ObjectViewModel : BaseViewModel
    {
        private ObservableCollection<Model.Object> _List;
        public ObservableCollection<Model.Object> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Unit> _Unit;
        public ObservableCollection<Model.Unit> Unit { get => _Unit; set { _Unit = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Supplier> _Supplier;
        public ObservableCollection<Model.Supplier> Supplier { get => _Supplier; set { _Supplier = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.Output> _ListOutput;
        public ObservableCollection<Model.Output> ListOutput { get => _ListOutput; set { _ListOutput = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.InputInfo> _ListInputInfo;
        public ObservableCollection<Model.InputInfo> ListInputInfo { get => _ListInputInfo; set { _ListInputInfo = value; OnPropertyChanged(); } }

        private Model.Object _SelectedItem;
        public Model.Object SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    QRCode = SelectedItem.QRCode;
                    BarCode = SelectedItem.BarCode;
                    SelectedUnit = SelectedItem.Unit;
                    SelectedSupplier = SelectedItem.Supplier;
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

        private Model.Supplier _SelectedSupplier;
        public Model.Supplier SelectedSupplier
        {
            get => _SelectedSupplier;
            set
            {
                _SelectedSupplier = value;
                OnPropertyChanged();
            }
        }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _QRCode;
        public string QRCode { get => _QRCode; set { _QRCode = value; OnPropertyChanged(); } }

        private string _BarCode;
        public string BarCode { get => _BarCode; set { _BarCode = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private string CreateAbbreviationName(string name)
        {
            string[] words = name.Split(' ');
            string abbr = "";

            foreach (var word in words)
            {
                abbr += Char.ToUpper(word.First());
            }
            return abbr;
        }

        public ObjectViewModel()
        {
            List = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);
            Unit = new ObservableCollection<Model.Unit>(DataProvider.Ins.DB.Unit);
            Supplier = new ObservableCollection<Model.Supplier>(DataProvider.Ins.DB.Supplier);

            ListOutput = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            ListOutputInfo = new ObservableCollection<Model.OutputInfo>(DataProvider.Ins.DB.OutputInfo);
            ListInputInfo = new ObservableCollection<Model.InputInfo>(DataProvider.Ins.DB.InputInfo);

            ICollectionView view = CollectionViewSource.GetDefaultView(List); //lớp dùng để hiển thị dữ liệu và cung cấp các chức năng duyệt, sắp xếp, lọc


            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;

            }, (p) =>
            {
                var Object = new Model.Object() { DisplayName = DisplayName, BarCode = BarCode, QRCode = QRCode, IdSupplier = SelectedSupplier.Id, IdUnit = SelectedUnit.Id, Id = this.CreateAbbreviationName(DisplayName) };

                DataProvider.Ins.DB.Object.Add(Object);
                DataProvider.Ins.DB.SaveChanges();

                List.Add(Object);
            });

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null || SelectedSupplier == null || SelectedUnit == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Object.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var Object = DataProvider.Ins.DB.Object.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Object.DisplayName = DisplayName;
                Object.BarCode = BarCode;
                Object.QRCode = QRCode;
                Object.IdSupplier = SelectedSupplier.Id;
                Object.IdUnit = SelectedUnit.Id;
                DataProvider.Ins.DB.SaveChanges();

                SelectedItem.DisplayName = DisplayName;
                view.Refresh();
            });

            DeleteCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedItem == null || SelectedSupplier == null || SelectedUnit == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Object.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;

                return false;

            }, (p) =>
            {
                var Object = DataProvider.Ins.DB.Object.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdObject == SelectedItem.Id).ToList();

                foreach (var item in collection)
                {
                    DataProvider.Ins.DB.OutputInfo.Remove(item);
                    ListOutputInfo.Remove(item);
                    ; ListOutputInfo.Clear();
                    DataProvider.Ins.DB.SaveChanges();
                }

                var collection1 = DataProvider.Ins.DB.InputInfo.Where(x => x.IdObject == SelectedItem.Id).ToList();
                foreach (var item in collection1)
                {
                    DataProvider.Ins.DB.InputInfo.Remove(item);
                    ListInputInfo.Remove(item);
                    ListInputInfo.Clear();
                }

                DataProvider.Ins.DB.Object.Remove(Object);
                List.Remove(Object);
                DataProvider.Ins.DB.SaveChanges();
            });
        }
    }
}
