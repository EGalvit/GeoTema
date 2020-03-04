using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;

namespace WPFGeoTema
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        //Closes the program when the Exit_ button is pressed
        private void Exit__Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Checks if the entered information in the UserName_ TextBox and the Password_ PasswordBox matches with the data from the SQL Database
        private void Login__Click(object sender, RoutedEventArgs e)
        {
            SqlUtility sql = new SqlUtility();
            try
            { 
                sql.con = new SqlConnection(SqlUtility.ConnectionString);
                sql.con.Open();
                sql.cmd = new SqlCommand("Select * from UserAccount where UserName =@UserName", sql.con);
                sql.cmd.Parameters.AddWithValue("@UserName", UserName_.Text); 
                sql.reader = sql.cmd.ExecuteReader();
                if (sql.reader.Read())
                {
                    if (sql.reader["UserPassword"].ToString().Equals(Password_.Password, StringComparison.InvariantCulture))
                    {
                        Utility.Name = UserName_.Text;
                        Utility.UserType = (int)sql.reader["UserType"];
                        Utility.OpenPrimary();
                        this.Close();
                        sql.con.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid login credentials");
                        Password_.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString()); 
            }
            sql.con.Close();
        }
        //If the enter button is pressed while the UserName_ TextBox is focused it focuses on the Password_ PasswordBox
        private void UserName__KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Password_.Focus();
            }
        }
        //If the enter button is pressed while the Password_ PasswordBox is focused it executes the PerformClick method on the Login Button
        private void Password__KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_.PerformClick();
            }
        }
        //Makes the LoginWindow movable
        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
