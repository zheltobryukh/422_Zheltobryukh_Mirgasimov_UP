using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _422_Zheltobryukh.Pages
{
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;
        private User currentUser;

        public AuthPage()
        {
            InitializeComponent();
            CaptchaChange(); // Инициализация капчи при загрузке страницы
        }

        // --- Хеширование SHA256 (совпадает с RegPage) ---
        public static string GetHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToUpper();
            }
        }

        // --- События текстовых полей и подсказок ---
        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = string.IsNullOrEmpty(TextBoxLogin.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtHintPass.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            // обработка смены пароля (если нужно)
        }

        // --- Авторизация пользователя ---
        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) ||
                string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            string hashedPassword = GetHash(PasswordBox.Password);

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                var user = db.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.LOGIN == TextBoxLogin.Text && u.Password == hashedPassword);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или пароль!");
                    failedAttempts++;

                    if (failedAttempts >= 3 && captcha.Visibility != Visibility.Visible)
                    {
                        CaptchaSwitch();
                    }

                    return;
                }

                MessageBox.Show("Вход выполнен успешно!");

                switch (user.Role)
                {
                    case "User":
                        NavigationService?.Navigate(new Pages.UserPage());
                        break;
                    case "Admin":
                        NavigationService?.Navigate(new Pages.AdminPage());
                        break;
                    default:
                        MessageBox.Show("Неизвестная роль пользователя!");
                        break;
                }
            }
        }

        // --- Переключение режима капчи ---
        public void CaptchaSwitch()
        {
            switch (captcha.Visibility)
            {
                case Visibility.Visible:
                    TextBoxLogin.Clear();
                    PasswordBox.Clear();
                    captcha.Visibility = Visibility.Hidden;
                    captchaInput.Visibility = Visibility.Hidden;
                    labelCaptcha.Visibility = Visibility.Hidden;
                    submitCaptcha.Visibility = Visibility.Hidden;
                    labelLogin.Visibility = Visibility.Visible;
                    labelPass.Visibility = Visibility.Visible;
                    TextBoxLogin.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Visible;
                    ButtonChangePassword.Visibility = Visibility.Visible;
                    ButtonEnter.Visibility = Visibility.Visible;
                    ButtonReg.Visibility = Visibility.Visible;
                    return;

                case Visibility.Hidden:
                    CaptchaChange();
                    captcha.Visibility = Visibility.Visible;
                    captchaInput.Visibility = Visibility.Visible;
                    labelCaptcha.Visibility = Visibility.Visible;
                    submitCaptcha.Visibility = Visibility.Visible;
                    labelLogin.Visibility = Visibility.Hidden;
                    labelPass.Visibility = Visibility.Hidden;
                    TextBoxLogin.Visibility = Visibility.Hidden;
                    PasswordBox.Visibility = Visibility.Hidden;
                    ButtonChangePassword.Visibility = Visibility.Hidden;
                    ButtonEnter.Visibility = Visibility.Hidden;
                    ButtonReg.Visibility = Visibility.Hidden;
                    return;
            }
        }

        // --- Генерация новой капчи ---
        public void CaptchaChange()
        {
            string allowchar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            StringBuilder captchaBuilder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 6; i++)
            {
                captchaBuilder.Append(allowchar[random.Next(0, allowchar.Length)]);
            }

            captcha.Text = captchaBuilder.ToString();
        }

        // --- Проверка капчи ---
        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча!", "Ошибка");
                CaptchaChange();
            }
            else
            {
                MessageBox.Show("Капча введена успешно..", "Успех");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        // --- Блокировка вставки / копирования ---
        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }

        // --- Переход на страницу регистрации ---
        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegPage());
        }
    }
}
