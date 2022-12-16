using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyKho.ViewModel
{
    public class ThongKe : BaseViewModel
    {
        private Object _Object;
        public Object Object { get => _Object; set { _Object = value; OnPropertyChanged(); } }

        private int _STT;
        public int STT { get => _STT; set { _STT = value; OnPropertyChanged(); } }

        private int _LuongNhap;
        public int LuongNhap { get => _LuongNhap; set { _LuongNhap = value; OnPropertyChanged(); } }

        private int _LuongXuat;
        public int LuongXuat { get => _LuongXuat; set { _LuongXuat = value; OnPropertyChanged(); } }

        private int _LuongTon;
        public int LuongTon { get => _LuongTon; set { _LuongTon = value; OnPropertyChanged(); } }

        private double _GiaNhap;
        public double GiaNhap { get => _GiaNhap; set { _GiaNhap = value; OnPropertyChanged(); } }

        private double _GiaXuat;
        public double GiaXuat { get => _GiaXuat; set { _GiaXuat = value; OnPropertyChanged(); } }

        private double _GiaTon;
        public double GiaTon { get => _GiaTon; set { _GiaTon = value; OnPropertyChanged(); } }

        private double _GiaLai;
        public double GiaLai { get => _GiaLai; set { _GiaLai = value; OnPropertyChanged(); } }

    }
}
