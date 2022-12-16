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
using System.Windows;

namespace QuanLyKho.ViewModel
{
    public class UsersViewModel : BaseViewModel
    {
        private ObservableCollection<Users> _List;
        public ObservableCollection<Users> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<UserRole> _UserRole;
        public ObservableCollection<UserRole> UserRole { get => _UserRole; set { _UserRole = value; OnPropertyChanged(); } }

        private int _Id;
        public int Id { get => _Id; set { _Id = value; OnPropertyChanged(); } }

        private string _DisplayName;
        public string DisplayName { get => _DisplayName; set { _DisplayName = value; OnPropertyChanged(); } }

        private string _UserName;
        public string UserName { get => _UserName; set { _UserName = value; OnPropertyChanged(); } }

        private string _Password;
        public string Password { get => _Password; set { _Password = value; OnPropertyChanged(); } }

        private int _IdRole;
        public int IdRole { get => _IdRole; set { _IdRole = value; OnPropertyChanged(); } }

        private Users _SelectedItem;
        public Users SelectedItem
        {
            get => _SelectedItem;
            set
            {
                _SelectedItem = value;
                OnPropertyChanged();
                if (SelectedItem != null)
                {
                    DisplayName = SelectedItem.DisplayName;
                    UserName = SelectedItem.UserName;
                    SelectedUserRole = SelectedItem.UserRole;
                }
            }
        }

        private UserRole _SelectedUserRole;
        public UserRole SelectedUserRole { get => _SelectedUserRole; set { _SelectedUserRole = value; OnPropertyChanged(); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ChangePasswordCommand { get; set; }

        public UsersViewModel()
        {
            List = new ObservableCollection<Model.Users>(DataProvider.Ins.DB.Users);
            UserRole = new ObservableCollection<Model.UserRole>(DataProvider.Ins.DB.UserRole);

            AddCommand = new RelayCommand<object>((p) =>
            {
                if (SelectedUserRole == null || UserName == null || DisplayName == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Users.Where(x => x.UserName == UserName);
                if (displayList == null || displayList.Count() != 0)
                    return false;
                return true;

            }, (p) =>

            {
                var Users = new Model.Users() { DisplayName = DisplayName, UserName = UserName, IdRole = SelectedUserRole.Id };

                DataProvider.Ins.DB.Users.Add(Users);
                DataProvider.Ins.DB.SaveChanges();
                List.Add(Users);
            });

            EditCommand = new RelayCommand<Users>((p) =>
            {
                if (UserName == null || SelectedUserRole == null)
                    return false;

                var displayList = DataProvider.Ins.DB.Users.Where(x => x.UserName == UserName);
                if (displayList == null || displayList.Count() != 0)
                    return false;

                return true;

            }, (p) =>
            {
                var Users = DataProvider.Ins.DB.Users.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                Users.DisplayName = DisplayName;
                Users.UserName = UserName;
                Users.IdRole = SelectedUserRole.Id;
                DataProvider.Ins.DB.SaveChanges();

                ICollectionView view = CollectionViewSource.GetDefaultView(List);
                view.Refresh();
            });

            DeleteCommand = new RelayCommand<Users>((p) =>
            {
                if (SelectedItem == null)
                    return false;
                return true;

            }, (p) =>
            {
                var Users = DataProvider.Ins.DB.Users.Where(x => x.Id == SelectedItem.Id).SingleOrDefault();
                DataProvider.Ins.DB.Users.Remove(Users);
                List.Remove(Users);
                DataProvider.Ins.DB.SaveChanges();
            });

            ChangePasswordCommand = new RelayCommand<Users>((p) =>
            {
                return true;

            }, (p) =>
            {
                MessageBox.Show("Vui lòng liên hệ người quản lý");
            });
        }
    }
}
