using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace _422_Zheltobryukh.Pages
{
    public partial class AddUserPage : Page
    {
        private User _currentUser = new User();

        public AddUserPage(User selectedUser)
        {
            InitializeComponent();

            if (selectedUser != null)
            {
                _currentUser = selectedUser;
            }

            DataContext = _currentUser;

            cmbRole.ItemsSource = new string[] { "Admin", "User" };
            if (selectedUser != null)
            {
                cmbRole.SelectedItem = selectedUser.Role;
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentUser.LOGIN))
                errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(_currentUser.Password))
                errors.AppendLine("Укажите пароль!");
            if (cmbRole.SelectedItem == null)
                errors.AppendLine("Выберите роль!");
            else
                _currentUser.Role = cmbRole.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(_currentUser.FIO))
                errors.AppendLine("Укажите ФИО");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                if (_currentUser.ID == 0)
                {
                    db.Users.Add(_currentUser);
                }
                else
                {
                    var existingUser = db.Users.Find(_currentUser.ID);
                    if (existingUser != null)
                    {
                        db.Entry(existingUser).CurrentValues.SetValues(_currentUser);
                    }
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = -1;
            TBFio.Text = "";
            TBPhoto.Text = "";
        }
    }
}