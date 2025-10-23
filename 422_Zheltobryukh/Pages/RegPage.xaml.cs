using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Text;

namespace _422_Zheltobryukh
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
            comboBxRole.SelectedIndex = 0;
        }

        // --- Обработчики для подсказок (placeholder'ов) ---
        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLog.Focus();
        }

        private void lblPassHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxFrst.Focus();
        }

        private void lblPassSecHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxScnd.Focus();
        }

        private void lblFioHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxFIO.Focus();
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLogHitn.Visibility = string.IsNullOrEmpty(txtbxLog.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFioHitn.Visibility = string.IsNullOrEmpty(txtbxFIO.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassHitn.Visibility = string.IsNullOrEmpty(passBxFrst.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassSecHitn.Visibility = string.IsNullOrEmpty(passBxScnd.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        // --- Основная логика регистрации ---
        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            // 1️⃣ Проверка заполнения всех полей
            if (string.IsNullOrEmpty(txtbxLog.Text) ||
                string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            // 2️⃣ Проверка совпадения паролей
            if (passBxFrst.Password != passBxScnd.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            // 3️⃣ Проверка длины пароля
            if (passBxFrst.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return;
            }

            // 4️⃣ Проверка символов в пароле
            bool hasEnglish = false;
            bool hasNumber = false;

            foreach (char c in passBxFrst.Password)
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                    hasEnglish = true;
                else if (c >= '0' && c <= '9')
                    hasNumber = true;
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

            // 5️⃣ Проверка существующего логина
            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                var user = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.LOGIN == txtbxLog.Text);

                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                // 6️⃣ Создание нового пользователя с хешированием SHA256
                User newUser = new User
                {
                    FIO = txtbxFIO.Text,
                    LOGIN = txtbxLog.Text,
                    Password = GetHash(passBxFrst.Password), // ✅ хешируем перед сохранением
                    Role = comboBxRole.Text
                };

                db.Users.Add(newUser);
                db.SaveChanges();
            }

            // 7️⃣ Очистка полей после регистрации
            MessageBox.Show("Пользователь успешно зарегистрирован!");

            txtbxLog.Clear();
            passBxFrst.Clear();
            passBxScnd.Clear();
            txtbxFIO.Clear();
            comboBxRole.SelectedIndex = 0;
        }

        // --- Метод хеширования SHA256 ---
        public static string GetHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }
    }
}
