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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDarkTheme = false;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Source = new Uri("Pages/AuthPage.xaml", UriKind.Relative);
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Обновляем видимость кнопки назад
            UpdateBackButtonVisibility();
        }

        private void UpdateBackButtonVisibility()
        {
            // Показываем кнопку назад только если есть куда возвращаться
            BackButton.Visibility = MainFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        // Публичный метод для очистки истории навигации
        public void ClearNavigationHistory()
        {
            // Очищаем журнал навигации
            while (MainFrame.CanGoBack)
            {
                MainFrame.RemoveBackEntry();
            }
            UpdateBackButtonVisibility();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => {
                DateTimeNow.Text = DateTime.Now.ToString();
            };
            timer.Start();

            // Инициализируем видимость кнопки назад
            UpdateBackButtonVisibility();
        }

        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Message",
            MessageBoxButton.YesNo) == MessageBoxResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }

        // Обработчик кнопки "Сменить стиль"
        private void ChangeStyleButton_Click(object sender, RoutedEventArgs e)
        {
            // Переключаем тему при каждом нажатии
            _isDarkTheme = !_isDarkTheme;

            string themeFile = _isDarkTheme ? "DictionaryDark.xaml" : "DictionaryLight.xaml";
            var uri = new Uri(themeFile, UriKind.Relative);

            // Загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            // Очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();

            // Добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        // Обработчик кнопки "Назад"
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
    }
}