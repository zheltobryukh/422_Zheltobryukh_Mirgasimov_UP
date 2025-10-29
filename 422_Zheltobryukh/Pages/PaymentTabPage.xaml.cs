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
using System.Data.Entity;

namespace _422_Zheltobryukh.Pages
{
    /// <summary>
    /// Логика взаимодействия для PaymentTabPage.xaml
    /// </summary>
    public partial class PaymentTabPage : Page
    {
        public PaymentTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void LoadData()
        {
            using (var db = new Zheltobryukh_DB_PaymentsEntities1())
            {
                DataGridPayment.ItemsSource = db.Payments
                    .Include(p => p.User)
                    .Include(p => p.Category)
                    .ToList();
            }
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                using (var db = new Zheltobryukh_DB_PaymentsEntities1())
                {
                    db.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                    DataGridPayment.ItemsSource = db.Payments
                        .Include(p => p.User)
                        .Include(p => p.Category)
                        .ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPaymentPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var paymentForRemoving = DataGridPayment.SelectedItems.Cast<Payment>().ToList();

            if (MessageBox.Show($"Вы точно хотите удалить {paymentForRemoving.Count()} элементов?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new Zheltobryukh_DB_PaymentsEntities1())
                    {
                        foreach (var payment in paymentForRemoving)
                        {
                            db.Entry(payment).State = System.Data.Entity.EntityState.Deleted;
                        }
                        db.SaveChanges();
                        MessageBox.Show("Данные успешно удалены!");
                        DataGridPayment.ItemsSource = db.Payments.Include(p => p.User).Include(p => p.Category).ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddPaymentPage((sender as Button).DataContext as Payment));
        }
    }
}