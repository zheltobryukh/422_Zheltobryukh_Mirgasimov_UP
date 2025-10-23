using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _422_Zheltobryukh.Pages
{
    public partial class UsersTabPage : Page
    {
        private Zheltobryukh_DB_PaymentsEntities1 _context = new Zheltobryukh_DB_PaymentsEntities1();

        public UsersTabPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = _context.Users.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridUser.ItemsSource = _context.Users.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService?.Navigate(new AddUserPage(null, _context));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            //var usersForRemoving = DataGridUser.SelectedItems.Cast<Users>().ToList();
            //if (MessageBox.Show($"Вы точно хотите удалить {usersForRemoving.Count()} элементов?",
            //    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //{
            //    try
            //    {
            //        _context.Users.RemoveRange(usersForRemoving);
            //        _context.SaveChanges();
            //        DataGridUser.ItemsSource = _context.Users.ToList();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService?.Navigate(new AddUserPage((sender as Button).DataContext as Users, _context));
        }
    }
}