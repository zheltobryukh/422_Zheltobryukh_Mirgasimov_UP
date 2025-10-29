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

namespace _422_Zheltobryukh.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPaymentPage.xaml
    /// </summary>
    public partial class AddPaymentPage : Page
    {
        private Payment _currentPayment = new Payment();

        public AddPaymentPage(Payment selectedPayment)
        {
            InitializeComponent();

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                CBCategory.ItemsSource = db.Categories.ToList();
                CBUser.ItemsSource = db.Users.ToList();
            }

            CBCategory.DisplayMemberPath = "Name";
            CBUser.DisplayMemberPath = "FIO";

            if (selectedPayment != null)
            {
                _currentPayment = selectedPayment;
            }

            DataContext = _currentPayment;

            if (selectedPayment != null)
            {
                CBUser.SelectedValue = selectedPayment.UserID;
                CBCategory.SelectedValue = selectedPayment.CategoryID;
            }
            else
            {
                // Устанавливаем текущую дату для новых записей
                TBDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(TBPaymentName.Text))
                errors.AppendLine("Укажите название платежа!");

            if (!DateTime.TryParse(TBDate.Text, out DateTime paymentDate))
            {
                errors.AppendLine("Введите корректную дату (например, dd.MM.yyyy)!");
            }
            else
            {
                _currentPayment.Date = paymentDate;
            }

            if (!decimal.TryParse(TBCount.Text, out decimal num) || num <= 0)
                errors.AppendLine("Укажите корректное количество!");
            else
                _currentPayment.Num = num;

            if (!decimal.TryParse(TBAmount.Text, out decimal price) || price <= 0)
                errors.AppendLine("Укажите корректную цену!");
            else
                _currentPayment.Price = price;

            if (CBUser.SelectedItem == null)
                errors.AppendLine("Укажите клиента!");
            else
                _currentPayment.UserID = (CBUser.SelectedItem as User).ID;

            if (CBCategory.SelectedItem == null)
                errors.AppendLine("Укажите категорию!");
            else
                _currentPayment.CategoryID = (CBCategory.SelectedItem as Category).ID;


            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                if (_currentPayment.ID == 0)
                {
                    db.Payments.Add(_currentPayment);
                }
                else
                {
                    var existingPayment = db.Payments.Find(_currentPayment.ID);
                    if (existingPayment != null)
                    {
                        db.Entry(existingPayment).CurrentValues.SetValues(_currentPayment);
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
            TBPaymentName.Text = "";
            TBAmount.Text = "";
            TBCount.Text = "";
            TBDate.Text = DateTime.Now.ToString("dd.MM.yyyy");
            CBCategory.SelectedIndex = -1;
            CBUser.SelectedIndex = -1;
        }
    }
}