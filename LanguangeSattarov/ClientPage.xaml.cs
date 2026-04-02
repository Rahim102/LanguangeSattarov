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

namespace LanguangeSattarov
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>

    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
            var currentclient = SattarovLanguageEntities2.GetContext().Client.ToList();

            ClientListView.ItemsSource = currentclient;
            ComboPageCount.SelectedIndex = 0;
            UpdateServices();

        }

        private void go_Click(object sender, RoutedEventArgs e)
        {
       
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void ComboPageCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();

        }
        private void UpdateServices()
        {
            var currentClient = SattarovLanguageEntities2.GetContext().Client.ToList();

            if(ComboGender.SelectedIndex==1)
            {
                currentClient=currentClient.Where(a=>a.GenderCode=="м").ToList();

            }
            if (ComboGender.SelectedIndex == 2)
            {
                currentClient = currentClient.Where(a => a.GenderCode == "ж").ToList();

            }
            currentClient = currentClient.Where(p => p.LastName.ToLower().Contains(TBoxSearch.Text.ToLower()) ||
                p.Phone.Replace("+7", "8").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Contains(TBoxSearch.Text.Replace("+7", "8").Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", ""))
               ||p.FirstName.ToLower().Contains(TBoxSearch.Text.ToLower())|| p.Patronymic.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
           
            if ( ComboSort.SelectedIndex==1)
            {
                currentClient = currentClient.OrderBy(p => p.FirstName).ToList();

            }
            if (ComboSort.SelectedIndex == 2)
            {
                currentClient = currentClient.OrderByDescending(p => p.LastVisitDate == "нет").OrderByDescending(p => p.LastVisitDateTime).ToList();

            }
            if (ComboSort.SelectedIndex == 3)
            {
                currentClient = currentClient.OrderByDescending(p => p.VisitCount).ToList();

            }
            ClientListView.ItemsSource = currentClient.ToList();
            TableList = currentClient;

            ChangePage(0, 0);

        }
        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        private void ChangePage(int direction, int? selectedPage)
        {
            CountRecords = TableList.Count;
            int pageSize = 10;
            if (ComboPageCount.SelectedIndex == 0)
            {
                pageSize = 10;
            }
            if (ComboPageCount.SelectedIndex == 1)
            {

                pageSize = 50;
            }
            if (ComboPageCount.SelectedIndex == 2)
            {

                pageSize = 200;
            }
            if (ComboPageCount.SelectedIndex==3)
            {
                pageSize = CountRecords;
            }
                CountPage = CountRecords / pageSize;
            if (CountRecords % pageSize > 0)
                CountPage++;

            if (selectedPage.HasValue)
                CurrentPage = selectedPage.Value;
            else if (direction == 1 && CurrentPage > 0)
                CurrentPage--;
            else if (direction == 2 && CurrentPage < CountPage - 1)
                CurrentPage++;
            else
                return;

            CurrentPageList = TableList.Skip(CurrentPage * pageSize).Take(pageSize).ToList();

            PageListBox.ItemsSource = Enumerable.Range(1, CountPage);
            PageListBox.SelectedIndex = CurrentPage;

            int shown = CurrentPage * pageSize + CurrentPageList.Count;
            TBAllRecords.Text = shown.ToString();
            TBRecap.Text = " из " + CountRecords;

            ClientListView.ItemsSource = CurrentPageList;
    
        }
    
           private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;
            var currentClientServices = SattarovLanguageEntities2.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ClientID == currentClient.ID).ToList();
            if (currentClientServices.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаление, так как существуют записи на этого клиента");

            }
            else {
                if (MessageBox.Show("Вы точно хотите выпольнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SattarovLanguageEntities2.GetContext().Client.Remove(currentClient);
                        SattarovLanguageEntities2.GetContext().SaveChanges();
                        ClientListView.ItemsSource = SattarovLanguageEntities2.GetContext().Client.ToList();
                        UpdateServices();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        private void BtnEdit_Click_1(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Client));
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null)); 
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateServices();
        }
    }
}
