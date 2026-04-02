using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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

namespace LanguangeSattarov
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentClient = new Client();
        public AddEditPage(Client selectedClient)
        {
            InitializeComponent();
            if (selectedClient != null)
            {
                _currentClient = selectedClient;
                IDPanel.Visibility = Visibility.Visible;
            }
            else
            {
                _currentClient = new Client();
                IDPanel.Visibility = Visibility.Collapsed;

            }
            if (_currentClient.Gender == null)
            {
                _currentClient.Gender = new Gender();
            }
            DataContext = _currentClient;
            this.Loaded += AddEditPage_Loaded;
        }

        private void AddEditPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentClient.Gender != null)
            {
                if (_currentClient.Gender.Name == "Мужской")
                    RButtonm.IsChecked = true;
                else
                    RButtonj.IsChecked = true;
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RButtonj_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RButtonm_Checked(object sender, RoutedEventArgs e)
        {
            var context = SattarovLanguageEntities2.GetContext();
            _currentClient.Gender = context.Gender.FirstOrDefault(g => g.Name == "Мужской");
        }

        private void RButtonj_Checked_1(object sender, RoutedEventArgs e)
        {
            var context = SattarovLanguageEntities2.GetContext();
            _currentClient.Gender = context.Gender.FirstOrDefault(g => g.Name == "Женский");

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            var context = SattarovLanguageEntities2.GetContext();
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentClient.FirstName) || string.IsNullOrWhiteSpace(_currentClient.LastName) || string.IsNullOrWhiteSpace(_currentClient.Patronymic))
            {
                errors.AppendLine("Укажите ФИО клиента");
            }
            if (!string.IsNullOrWhiteSpace(_currentClient.FirstName) &&
                  _currentClient.FirstName.Length >= 50)
            {
                errors.AppendLine("Фамилия должно быть меньше 50 символов");
            }

            if (!string.IsNullOrWhiteSpace(_currentClient.LastName) &&
                _currentClient.LastName.Length >= 50)
            {
                errors.AppendLine("Имя должно быть меньше 50 символов");
            }

            if (!string.IsNullOrWhiteSpace(_currentClient.Patronymic) &&
                _currentClient.Patronymic.Length >= 50)
            {
                errors.AppendLine("Отчество должно быть меньше 50 символов");
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Email))
            {
                errors.AppendLine("Укажите email клиента");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    _currentClient.Email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    errors.AppendLine("Укажите правильно email клиента");
                }
            }
            if (string.IsNullOrWhiteSpace(_currentClient.Phone))
                errors.AppendLine("Укажите телефон агента");
            else
            {
                string ph = _currentClient.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace("+", "").Replace(" ", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11) || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон агента");
                if (ph.Length < 11)
                {
                    errors.AppendLine("Укажите правильно телефон агента");
                }
            }

            

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (!IsValidName(_currentClient.FirstName) ||
                 !IsValidName(_currentClient.LastName) ||
                 !IsValidName(_currentClient.Patronymic))
            {
                MessageBox.Show("ФИО может содержать только буквы, пробел и дефис!");
                return;
            }
         
            if (_currentClient.ID == 0)
            {
                _currentClient.RegistrationDate = DateTime.Now;
              SattarovLanguageEntities2.GetContext().Client.Add(_currentClient); 
            }
            try
            {
                SattarovLanguageEntities2.GetContext().SaveChanges();
                MessageBox.Show("Сохранено!");
                Manager.MainFrame.Navigate(new ClientPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

           

          
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                {
                    string fileName = System.IO.Path.GetFileName(myOpenFileDialog.FileName);
                    _currentClient.PhotoPath = "/Клиенты/" + fileName;
                    LogoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));

                }
            }
        }
        private bool IsValidName(string text)
        {
            return Regex.IsMatch(text, @"^[А-Яа-яA-Za-z\s-]+$");
        }

    }
}
