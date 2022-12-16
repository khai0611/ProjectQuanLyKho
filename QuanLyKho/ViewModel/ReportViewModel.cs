using QuanLyKho.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace QuanLyKho.ViewModel
{
    public class ReportViewModel : BaseViewModel
    {
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
        public ReportViewModel()
        {

        }
        //private Model.Output _Output;
        //public Model.Output Output { get => _Output; set { _Output = value; OnPropertyChanged(); } }
        public Output Id { get; set; }
        public Output Status { get; set; }
        public Output Total { get; set; }
        public OutputInfo Count { get; set; }
        public OutputInfo SumPrice { get; set; }
        public Customer DisplayName { get; set; }
        public Customer Address { get; set; }
        public Customer Phone { get; set; }
        public Customer Email { get; set; }
        public InputInfo OutputPrice { get; set; }
        public Users DisplayName1 { get; set; }
        public Model.Object DisplayName2 { get; set; }


    }
}
