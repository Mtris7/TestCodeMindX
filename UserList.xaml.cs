using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TestCodeMindX.Model;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TestCodeMindX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserList : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public UserList()
        {
            this.InitializeComponent();
            InventoryList.ItemsSource = GetProducts((App.Current as App).ConnectionString);

            //show defalut 10 items
            NextButton_Click(null, null);
        }
        async public void BuildUIAsync()
        {
            var m_dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;// CoreWindow.GetForCurrentThread();
            await m_dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NextButton_Click(null, null);
                TextBox_TextChanged(null, null);
            });
        }
        public UserList(int index, string filterStr )
        {
            try
            {

                this.InitializeComponent();
                InventoryList.ItemsSource = GetProducts((App.Current as App).ConnectionString);
                pageIndex = index - 1;
                filter.Text = filterStr;
                BuildUIAsync();


            }
            catch(Exception ex)
            {
                Debugger.Break();
            }
        }
        public ObservableCollection<User> users = new ObservableCollection<User>();
        public ObservableCollection<User> GetProducts(string connectionString)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("select    user_name AS userName ");
            stringBuilder.AppendLine("          ,full_name AS fullName ");
            stringBuilder.AppendLine("          ,email AS email ");
            stringBuilder.AppendLine(" from Users ");

            users = new ObservableCollection<User>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = stringBuilder.ToString();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var user = new User();
                                    user.userName = reader.GetString(0);
                                    user.fullName = reader.GetString(1);
                                    user.email = reader.GetString(2);
                                    users.Add(user);
                                }
                            }
                        }
                    }
                }
                return users;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                localSettings.Values["filter"] = filter.Text;
                var list = users.ToList();
                InventoryList.ItemsSource = list.Where(x => x.fullName.ToUpper().Contains(filter.Text?.ToUpper()) 
                || x.userName.ToUpper().Contains(filter.Text?.ToUpper())).OrderBy(x=>x.userName);
            }
            catch(Exception ex)
            {
                Debugger.Break();
            }
        }
        //private List<User> postsList = new List<User>(); //Given List
        //private List<User> displayPostsList = new List<User>(); //List to be displayed in ListView
        int pageIndex = -1;
        int pageSize = 5; //Set the size of the page

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex++;
            localSettings.Values["pageIndex"] = pageIndex;
            InventoryList.ItemsSource = users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            pageIndex--;
            localSettings.Values["pageIndex"] = pageIndex;
            InventoryList.ItemsSource = users.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["pageIndex"] = -1;
            localSettings.Values["ID"] = null;
            localSettings.Values["password"] = null;
            Frame.Navigate(typeof(Login), null);
        }
    }
}
