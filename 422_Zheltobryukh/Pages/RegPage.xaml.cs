using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _422_Zheltobryukh
{
    /// <summary>
    /// Interaction logic for RegPage.xaml
    /// </summary>
    /// 
    public partial class RegPage : Page
    {   

        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;

        }

        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs
e)
        {
            txtbxLog.Focus();
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }


        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLogHitn.Visibility = Visibility.Visible;
            if (txtbxLog.Text.Length > 0)
            {
                lblLogHitn.Visibility = Visibility.Hidden;

            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) ||
                string.IsNullOrEmpty(txtbxFIO.Text) || string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            bool hasEnglish = false;
            bool hasNumber = false;

            for (int i = 0; i < passBxFrst.Password.Length; i++)
            {
                char c = passBxFrst.Password[i];
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                {
                    hasEnglish = true;
                }
                else if (c >= '0' && c <= '9')
                {
                    hasNumber = true;
                }
            }

            if (!hasEnglish)
            {
                MessageBox.Show("Используйте только английскую раскладку!");
                return;
            }

            if (!hasNumber)
            {
                MessageBox.Show("Добавьте хотя бы одну цифру!");
                return;
            }

            User userObject = new User
            {
                FIO = txtbxFIO.Text,
                LOGIN = txtbxLog.Text,
                Password = passBxFrst.Password,
                Role = comboBxRole.Text
            };

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                db.Users.Add(userObject);
                db.SaveChanges();
            }

            MessageBox.Show("Пользователь успешно зарегистрирован!");
            txtbxLog.Clear();
            passBxFrst.Clear();
            passBxScnd.Clear();
            comboBxRole.SelectedIndex = 1;
            txtbxFIO.Clear();
        }
    }
}