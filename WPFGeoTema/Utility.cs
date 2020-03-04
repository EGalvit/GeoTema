using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace WPFGeoTema
{
    public static class Utility
    {
        //Static variables to save information
        public static string Name = "";
        public static int UserType = 0;
        public static bool Hide = false;

        //Raises the click event on a selected button SomeButton.PerformClick();
        public static void PerformClick(this Button Button)
        {
            Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
        //Opens the Login Window
        public static void OpenLogin()
        {
            LoginWindow lw = new LoginWindow();
            lw.Show();
        }
        //Opens the Primary Window
        public static void OpenPrimary()
        {
            PrimaryWindow pw = new PrimaryWindow();
            pw.Show();
        }
        //Opens the CreateUser Window
        public static void OpenCreateUser()
        {
            CreateUserWindow cu = new CreateUserWindow();
            cu.Show();
        }
    }
    public class SqlUtility
    {
        public SqlConnection con;
        public SqlCommand cmd;
        public SqlDataReader reader;
        public SqlDataAdapter adapter;
        public static string ConnectionString = "Data Source=192.168.4.103;Initial Catalog=GeoTema;Persist Security Info=True;User ID=sa;Password=Passw0rd";
    }
}
