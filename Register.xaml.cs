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
    public sealed partial class Register : Page
    {
        public Register()
        {
            this.InitializeComponent();
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(userName.Text))
            {
                var messageDialog = new MessageDialog("Vui long nhap user name.");
                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }  
            if (string.IsNullOrEmpty(password.Password))
            {
                var messageDialog = new MessageDialog("Vui long nhap password.");
                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }  
            if (password.Password != confirmPassword.Password)
            {
                var messageDialog = new MessageDialog("Confirm password is incorrect.");
                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }
            var message = new MessageDialog("Register Successfully");
            var connectionString = (App.Current as App).ConnectionString;
            if (!CheckUser(connectionString))
            {
                message.Content = "Registration failed , UserName already exist";
                // Show the message dialog
                await message.ShowAsync();
                return;
            }    
            if (InsertUser(connectionString))
            {
                message.Content = "Successful registration";
                // Show the message dialog
                await message.ShowAsync();
                this.Frame.Navigate(typeof(Login), null);
            }
            else
            {
                message.Content = "Registration failed";
                // Show the message dialog
                await message.ShowAsync();
            }

        }
        private bool CheckUser(string connectionString)
        {
            try
            {

                StringBuilder checkUser = new StringBuilder();
                checkUser.AppendLine("select user_name from Users");
                checkUser.AppendLine("           where user_name = '" + userName.Text + "'");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = checkUser.ToString();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows) return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                Debugger.Break();
                return false;
            }
        }
        public bool InsertUser(string connectionString)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("INSERT INTO Users(user_name,full_name,email,password) ");
                stringBuilder.AppendLine("          VALUES('" +userName.Text +"', ");
                stringBuilder.AppendLine("          '" +fullName.Text +"', ");
                stringBuilder.AppendLine("          '" +email.Text +"', ");
                stringBuilder.AppendLine("          '" +password.Password + "') ");
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
                                return true;
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
