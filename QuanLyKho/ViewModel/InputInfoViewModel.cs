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
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    class InputInfoViewModel : BaseViewModel
    {
        private ObservableCollection<Model.InputInfo> _List;
        public ObservableCollection<Model.InputInfo> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Input> _Input;
        public ObservableCollection<Model.Input> Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }

        private Model.InputInfo _SelectedItem;
        public Model.InputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.Object;
                    SelectedInput = SelectedItem.Input;
                    Count = SelectedItem.Count;
                    Status = SelectedItem.Status;
                    InputPrice = SelectedItem.InputPrice;
                    OutputPrice = SelectedItem.OutputPrice;
                }
            }
        }

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
                // Chưa thêm được ngày tùy chọn - Chỉ thêm được ngày hiện tại
            }
        }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private double? _InputPrice;
        public double? InputPrice { get => _InputPrice; set { _InputPrice = value; OnPropertyChanged(); } }

        private double? _OutputPrice;
        public double? OutputPrice { get => _OutputPrice; set { _OutputPrice = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }
        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public InputInfoViewModel()
        {
            List = new ObservableCollection<Model.InputInfo>(DataProvider.Ins.DB.InputInfo);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);
            Input = new ObservableCollection<Model.Input>(DataProvider.Ins.DB.Input);

            ICollectionView view = CollectionViewSource.GetDefaultView(List);

            AddCommand = new RelayCommand<object>((p) =>
            {
                return true;

            }, (p) =>

            {
                var NewInput = new Model.Input() { Id = Guid.NewGuid().ToString(), DateInput = DateTime.Now };
                var InputInfo = new Model.InputInfo() { IdObject = SelectedObject.Id, IdInput = NewInput.Id, Count = Count, InputPrice = InputPrice, OutputPrice = OutputPrice, Status = Status, Id = Guid.NewGuid().ToString() };

                DataProvider.Ins.DB.Input.Add(NewInput);
                DataProvider.Ins.DB.InputInfo.Add(InputInfo);
                DataProvider.Ins.DB.SaveChanges();

                List.Add(InputInfo);
            });

            EditCommand = new RelayCommand<InputInfo>((p) =>
            {
                if (SelectedItem == null || SelectedObject == null || SelectedInput == null)
                    return false;

                var displayList = DataProvider.Ins.DB.InputInfo.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var InputInfo = DataProvider.Ins.DB.InputInfo.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                InputInfo.IdObject = SelectedObject.Id;
                InputInfo.IdInput = SelectedInput.Id;

                //InputInfo.IdInput = Input.Id;
                InputInfo.Count = Count;
                InputInfo.InputPrice = InputPrice;
                InputInfo.OutputPrice = OutputPrice;
                InputInfo.Status = Status;

                view.Refresh();
                DataProvider.Ins.DB.SaveChanges();
            });

            DeleteCommand = new RelayCommand<InputInfo>((p) =>
            {
                if (SelectedItem == null || SelectedObject == null || SelectedInput == null)
                    return false;

                var displayList = DataProvider.Ins.DB.InputInfo.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var InputInfo = DataProvider.Ins.DB.InputInfo.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                var collection = DataProvider.Ins.DB.OutputInfo.Where(x => x.IdInputInfo == SelectedItem.Id).ToList();

                foreach (var item in collection)
                {
                    DataProvider.Ins.DB.OutputInfo.Remove(item);
                    DataProvider.Ins.DB.SaveChanges();
                    //ListOutputInfo.Remove(item);
                }
                collection.Clear();

                DataProvider.Ins.DB.InputInfo.Remove(InputInfo);
                List.Remove(InputInfo);
                DataProvider.Ins.DB.SaveChanges();

                view.Refresh();
            });
        }
    }
}
