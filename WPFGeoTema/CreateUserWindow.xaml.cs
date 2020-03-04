using System;
using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;

namespace WPFGeoTema
{
    public partial class CreateUserWindow : Window
    {
        public CreateUserWindow()
        {
            InitializeComponent();
        }
        //Closes the window
        private void Cancel__Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Closes the window
        private void Exit__Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Makes the window movable
        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //Creates a new user with the data from the textbox, passwordbox and the combobox
        private void Create__Click(object sender, RoutedEventArgs e)
        {
            if (UserNameCU_.Text == "" || UserNameCU_.Text == " " || UserTypeCU_.SelectedIndex == -1 || PasswordCU_.Password == "" || PasswordreCU_.Password == "")
            {
                MessageBox.Show("Please fill out the form correctly");
            }
            else
            {
                if (PasswordCU_.Password.Equals(PasswordreCU_.Password))
                {
                    SqlUtility sql = new SqlUtility();
                    try
                    {
                        sql.con = new SqlConnection(SqlUtility.ConnectionString);
                        sql.con.Open();
                        sql.cmd = new SqlCommand("IF NOT EXISTS (SELECT * FROM UserAccount WHERE UserName = @UserName) INSERT INTO UserAccount VALUES (@UserName, @UserPassword, @UserType)", sql.con);
                        sql.cmd.Parameters.AddWithValue("@UserName", UserNameCU_.Text);
                        sql.cmd.Parameters.AddWithValue("@UserPassword", PasswordCU_.Password);
                        sql.cmd.Parameters.AddWithValue("@UserType", UserTypeCU_.SelectedIndex + 1);
                        int succes = sql.cmd.ExecuteNonQuery();
                        if (succes == -1)
                        {
                            MessageBox.Show("Username is already taken try again!");
                            UserNameCU_.Clear();
                        }
                        else
                        {
                            MessageBox.Show("User Created");
                            this.Close();
                        }
                        sql.con.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Passwords doesnt match!");
                }
            }
        }
    }
}
