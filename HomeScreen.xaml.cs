using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using TestCodeMindX.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class HomeScreen : Page
    {
        public static string username = string.Empty;
        private string connectionString = (App.Current as App).ConnectionString;
        public HomeScreen()
        {
            this.InitializeComponent();
            GetListPost(connectionString);
        }
        public List<Post> posts = new List<Post>();
        void GetListPost (string connectionString)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("select    user_name AS userName ");
                stringBuilder.AppendLine("          ,post ");
                stringBuilder.AppendLine("          ,id");
                stringBuilder.AppendLine(" from Post ");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = stringBuilder.ToString();
                            posts.Clear();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var post = new Post();
                                    post.userName = reader.GetString(0);
                                    post.post = reader.GetString(1);
                                    post.id = reader.GetInt32(2);
                                    posts.Add(post);
                                }
                            }
                        }
                    }
                }
                listPost.ItemsSource = posts.OrderByDescending(x=>x.id);
            }
            catch(Exception ex)
            {
                Debugger.Break();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("insert into  Post( user_name,post)");
                stringBuilder.AppendLine("      VALUES( '"+ HomeScreen.username + "','" + newPost.Text + "')");
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
                                var post = new Post();
                                post.userName = HomeScreen.username;
                                post.post = newPost.Text;
                                post.id = posts.Max(x => x.id) + 1;
                                posts.Add(post);
                            }
                        }
                    }
                }
                listPost.ItemsSource = posts.OrderByDescending(x => x.id);
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
    }
}
