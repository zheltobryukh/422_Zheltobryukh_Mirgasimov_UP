using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace _422_Zheltobryukh.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            UpdateUsers(); // сразу загружаем пользователей
        }

        // ===== Очистка фильтров =====
        private void clearFiltersButton_Click_1(object sender, RoutedEventArgs e)
        {
            fioFilterTextBox.Text = "";
            sortComboBox.SelectedIndex = 0;
            onlyAdminCheckBox.IsChecked = false;
            UpdateUsers();
        }

        // ===== Обновление списка при вводе текста =====
        private void fioFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateUsers();
        }

        // ===== Обновление при смене сортировки =====
        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUsers();
        }

        // ===== Фильтр только администраторы =====
        private void onlyAdminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        private void onlyAdminCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateUsers();
        }

        // ===== Основной метод обновления данных =====
        private void UpdateUsers()
        {
            if (!IsInitialized)
                return;

            try
            {
                using (var db = new Zheltobryukh_DB_PaymentsEntities1())
                {
                    List<User> currentUsers = db.Users.ToList();

                    // 🔹 Фильтрация по ФИО
                    if (!string.IsNullOrWhiteSpace(fioFilterTextBox.Text))
                    {
                        currentUsers = currentUsers
                            .Where(x => x.FIO != null &&
                                        x.FIO.ToLower().Contains(fioFilterTextBox.Text.ToLower()))
                            .ToList();
                    }

                    // 🔹 Фильтр по роли (только админы)
                    if (onlyAdminCheckBox.IsChecked == true)
                    {
                        currentUsers = currentUsers
                            .Where(x => x.Role == "Admin")
                            .ToList();
                    }

                    // 🔹 Сортировка (0 — по возрастанию, 1 — по убыванию)
                    if (sortComboBox.SelectedIndex == 0)
                        currentUsers = currentUsers.OrderBy(x => x.FIO).ToList();
                    else
                        currentUsers = currentUsers.OrderByDescending(x => x.FIO).ToList();

                    // 🔹 Отображение списка
                    ListUser.ItemsSource = currentUsers;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка пользователей:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
