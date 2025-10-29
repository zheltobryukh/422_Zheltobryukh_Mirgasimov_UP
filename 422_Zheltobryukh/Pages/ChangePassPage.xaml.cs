using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace _422_Zheltobryukh.Pages
{
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbLogin.Text) ||
                string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новые пароли не совпадают!");
                return;
            }

            if (NewPasswordBox.Password.Length < 6)
            {
                MessageBox.Show("Новый пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            bool hasEnglish = false;
            bool hasNumber = false;
            foreach (char c in NewPasswordBox.Password)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    hasEnglish = true;
                else if (c >= '0' && c <= '9')
                    hasNumber = true;
            }

            if (!hasEnglish)
            {
                MessageBox.Show("Новый пароль должен содержать только английскую раскладку!");
                return;
            }
            if (!hasNumber)
            {
                MessageBox.Show("Новый пароль должен содержать хотя бы одну цифру!");
                return;
            }

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                string currentPasswordHash = GetHash(CurrentPasswordBox.Password);
                var user = db.Users.FirstOrDefault(u => u.LOGIN == TbLogin.Text && u.Password == currentPasswordHash);

                if (user == null)
                {
                    MessageBox.Show("Текущий логин или пароль неверный!");
                    return;
                }

                user.Password = GetHash(NewPasswordBox.Password);
                db.SaveChanges();

                MessageBox.Show("Пароль успешно изменен!");

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                else
                {
                    NavigationService?.Navigate(new AuthPage());
                }
            }
        }
    }
}