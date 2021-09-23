using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace TestCodeMindX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public Login()
        {
            this.InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Register), null);
        }
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var messageDialog = new MessageDialog("User name or password is not correct.");
                if (string.IsNullOrEmpty(userName.Text))
                {
                    messageDialog.Content = "User name is not null";
                    // Show the message dialog
                    await messageDialog.ShowAsync();
                    return;
                }   
                if (string.IsNullOrEmpty(password.Password))
                {
                    messageDialog.Content = "password is not null";
                    // Show the message dialog
                    await messageDialog.ShowAsync();
                    return;
                }    
                if (CheckLogins((App.Current as App).ConnectionString, userName.Text, password.Password))
                {
                    HomeScreen.username = userName.Text;
                    this.Frame.Navigate(typeof(HomeScreen), null);
                }    
                else
                {
                    // Show the message dialog
                    await messageDialog.ShowAsync();
                    userName.Text = string.Empty;
                    password.Password = string.Empty;
                }
            }catch(Exception ex)
            {
                Debugger.Break();
            }
        }
        public bool CheckLogins(string connectionString, string userName, string password)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("select    user_name AS userName ");
                stringBuilder.AppendLine("          ,full_name AS fullName ");
                stringBuilder.AppendLine("          ,email AS email ");
                stringBuilder.AppendLine(" from Users ");
                stringBuilder.AppendLine(" where  user_name = '" + userName + "'");
                stringBuilder.AppendLine(" AND  password = '" + password + "'");
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
                                if (reader.HasRows)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception eSql)
            {
                Debugger.Break();
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return false;
        }
    }
}
