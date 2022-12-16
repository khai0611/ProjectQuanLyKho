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
using System.Windows;
using System.Windows.Input;

namespace QuanLyKho.ViewModel
{
    class OutputInfoViewModel : BaseViewModel
    {
        private ObservableCollection<Model.OutputInfo> _ListOutputInfo;
        public ObservableCollection<Model.OutputInfo> ListOutputInfo { get => _ListOutputInfo; set { _ListOutputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Object> _Object;
        public ObservableCollection<Model.Object> Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Input> _Input;
        public ObservableCollection<Model.Input> Input { get => _Input; set { _Input = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.InputInfo> _InputInfo;
        public ObservableCollection<Model.InputInfo> InputInfo { get => _InputInfo; set { _InputInfo = value; OnPropertyChanged(); } }

        private ObservableCollection<Model.Output> _Output;
        public ObservableCollection<Model.Output> Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }

        //private ObservableCollection<Model.Customer> _Customer;
        //public ObservableCollection<Model.Customer> Customer { get => _Customer; set { _Customer = value; OnPropertyChanged(); } }

        private Model.OutputInfo _SelectedItem;
        public Model.OutputInfo SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    SelectedObject = SelectedItem.Object;
                    SelectedOutput = SelectedItem.Output;
                    Count = SelectedItem.Count;
                    SelectedInputInfo = SelectedItem.InputInfo;
                    //SelectedCustomer = SelectedItem.Customer;
                    Status = SelectedItem.Status;
                }
            }
        }

        private Model.Object _SelectedObject;
        public Model.Object SelectedObject { get => _SelectedObject; set { _SelectedObject = value; OnPropertyChanged(); } }

        private Model.Output _SelectedOutput;
        public Model.Output SelectedOutput { get => _SelectedOutput; set { _SelectedOutput = value; OnPropertyChanged(); } }

        private Model.InputInfo _SelectedInputInfo;
        public Model.InputInfo SelectedInputInfo { get => _SelectedInputInfo; set { _SelectedInputInfo = value; OnPropertyChanged(); } }

        private Model.Customer _SelectedCustomer;
        public Model.Customer SelectedCustomer { get => _SelectedCustomer; set { _SelectedCustomer = value; OnPropertyChanged(); } }

        private int? _Count;
        public int? Count { get => _Count; set { _Count = value; OnPropertyChanged(); } }

        private string _Status;
        public string Status { get => _Status; set { _Status = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }

        public OutputInfoViewModel()
        {
            ListOutputInfo = new ObservableCollection<Model.OutputInfo>(DataProvider.Ins.DB.OutputInfo);
            Object = new ObservableCollection<Model.Object>(DataProvider.Ins.DB.Object);
            Output = new ObservableCollection<Model.Output>(DataProvider.Ins.DB.Output);
            InputInfo = new ObservableCollection<Model.InputInfo>(DataProvider.Ins.DB.InputInfo);
            //Customer = new ObservableCollection<Model.Customer>(DataProvider.Ins.DB.Customers);

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedObject == null || SelectedOutput == null || SelectedInputInfo == null)
                    return false;
                return true;

            }, (p) =>

            {
                var NewOutput = new Model.Output() { Id = Guid.NewGuid().ToString(), DateOutput = SelectedOutput.DateOutput };
                var OutputInfo = new Model.OutputInfo() { IdObject = SelectedObject.Id, IdOutput = NewOutput.Id, Count = Count, IdInputInfo = SelectedInputInfo.Id, Status = Status, Id = Guid.NewGuid().ToString() };

                DataProvider.Ins.DB.Output.Add(NewOutput);
                DataProvider.Ins.DB.OutputInfo.Add(OutputInfo);
                DataProvider.Ins.DB.SaveChanges();

                ListOutputInfo.Add(OutputInfo);
            });

            EditCommand = new RelayCommand<OutputInfo>((p) =>
            {
                if (SelectedObject == null || SelectedOutput == null || SelectedInputInfo == null)
                    return false;

                var displayList = DataProvider.Ins.DB.OutputInfo.Where(x => x.Id == SelectedItem.Id);
                if (displayList != null && displayList.Count() != 0)
                    return true;
                return false;

            }, (p) =>
            {
                var OutputInfo = DataProvider.Ins.DB.OutputInfo.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                OutputInfo.IdObject = SelectedObject.Id;
                OutputInfo.IdOutput = SelectedOutput.Id;
                //OutputInfo.Count = Count;
                OutputInfo.IdInputInfo = SelectedInputInfo.Id;
                //OutputInfo.IdCustomer = SelectedCustomer.Id;
                OutputInfo.Status = Status;
                DataProvider.Ins.DB.SaveChanges();
            });
        }
    }
}