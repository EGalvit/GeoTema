using System;
using System.Windows;
using System.Windows.Input;
using System.Data;
using System.Data.SqlClient;

namespace WPFGeoTema
{
    public partial class PrimaryWindow : Window
    {
        public PrimaryWindow()
        {
            InitializeComponent();
            Welcome_.Content = "Welcome " + Utility.Name;
            FillGrid();
            UserInfo();
        }
        //adds a list of User names to the ComboBox in the admin tab
        public void UserList()
        {
            UserList_.Items.Clear();
            DelUserList_.Items.Clear();
            SqlUtility sql = new SqlUtility();
            try
            {
                sql.con = new SqlConnection(SqlUtility.ConnectionString);
                sql.con.Open();
                sql.cmd = new SqlCommand("Select UserName from UserAccount", sql.con);
                sql.reader = sql.cmd.ExecuteReader();
                while (sql.reader.Read())
                {
                    UserList_.Items.Add(sql.reader["UserName"].ToString());
                    DelUserList_.Items.Add(sql.reader["UserName"].ToString());
                }
                sql.reader.Close();
                sql.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //fills the datagrid in the read tab with the data from the database
        private void FillGrid()
        {
            SqlUtility sql = new SqlUtility();
            try
            {
                sql.con = new SqlConnection(SqlUtility.ConnectionString);
                sql.con.Open();
                sql.cmd = new SqlCommand("SELECT GeoTema_Land.Land, GeoTema_Land.Verdensdel, GeoTema_Rang.Rang, GeoTema_Rang.Fødselsrate FROM GeoTema_Land INNER JOIN GeoTema_Rang ON GeoTema_Land.Rang=GeoTema_Rang.Rang ORDER BY Rang", sql.con);
                sql.adapter = new SqlDataAdapter(sql.cmd);
                DataTable dt = new DataTable();
                sql.adapter.Fill(dt);
                DataGrid_.ItemsSource = dt.DefaultView;
                sql.cmd.Dispose();
                sql.adapter.Dispose();
                sql.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //enables the create and or admin tab item if the user type matches the requirements
        private void UserInfo()
        {
            UserNameD_.Content = Utility.Name;
            if (Utility.UserType == 1)
            {
                UserTypeD_.Content = "Standard User";
            }
            else if (Utility.UserType == 2)
            {
                UserTypeD_.Content = "Super User";
                Create_.IsEnabled = true;
            }
           else if (Utility.UserType == 3)
            {
                UserTypeD_.Content = "Admin User";
                Create_.IsEnabled = true;
                Admin_.IsEnabled = true;
                UserList();
            }
        }
        //Closes the program when the Exit_ button is pressed
        private void Exit__Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Makes the LoginWindow movable
        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        //Either shows or hides the change password stackpanel 
        private void ChangePassword__Click(object sender, RoutedEventArgs e)
        {
            if (Utility.Hide == false)
            {
            StackPass.IsEnabled = true;
            StackPass.Visibility = Visibility.Visible;
                Utility.Hide = true;
            }
            else
            {
                StackPass.IsEnabled = false;
                StackPass.Visibility = Visibility.Hidden;
                Utility.Hide = false;
            }
        }
        //Opens the CreateUser window
        private void CreateUser__Click(object sender, RoutedEventArgs e)
        {
            Utility.OpenCreateUser();
        }
        //remove a users password
        private void ResetPassword__Click(object sender, RoutedEventArgs e)
        {
            SqlUtility sql = new SqlUtility();
            if (UserList_.SelectedIndex != -1)
            {
                try
                {
                    sql.con = new SqlConnection(SqlUtility.ConnectionString);
                    sql.con.Open();
                    sql.cmd = new SqlCommand("update UserAccount set UserPassword='' where UserName = @UserName", sql.con);
                    sql.cmd.Parameters.AddWithValue("@UserName", UserList_.SelectedItem.ToString());
                    sql.reader = sql.cmd.ExecuteReader();
                    MessageBox.Show("Password has been successfully removed ask the user to change their password on next login");
                    sql.con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please chose a user first");
            }
        }
        //Updates the users password if the requirements are met
        private void PasswordSave__Click(object sender, RoutedEventArgs e)
        {
            SqlUtility sql = new SqlUtility();
            try
            {
                sql.con = new SqlConnection(SqlUtility.ConnectionString);
                sql.con.Open();
                sql.cmd = new SqlCommand("Select * from UserAccount where UserName =@UserName", sql.con);
                sql.cmd.Parameters.AddWithValue("@UserName", Utility.Name);
                sql.reader = sql.cmd.ExecuteReader();
                if (sql.reader.Read())
                {
                    if (sql.reader["UserPassword"].ToString().Equals(CurrentPassword_.Password, StringComparison.InvariantCulture))
                    {
                        if (NewPassword_.Password.Equals(NewPasswordRe_.Password))
                        {
                            sql.cmd = new SqlCommand("Update UserAccount set UserPassword =@UserPassword where UserName =@UserName", sql.con);
                            sql.cmd.Parameters.AddWithValue("@UserName", Utility.Name);
                            sql.cmd.Parameters.AddWithValue("@UserPassword", NewPassword_.Password);
                            sql.reader.Close();
                            sql.cmd.ExecuteNonQuery();
                            sql.con.Close();
                            MessageBox.Show("Password has succesfully been changed");
                            NewPassword_.Clear();
                            NewPasswordRe_.Clear();
                            CurrentPassword_.Clear();
                            ChangePassword_.PerformClick();
                        }
                        else
                        {
                            MessageBox.Show("Passwords doesnt match!");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Wrong Password!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //Opens the login window and closes the current window
        private void Logout__Click(object sender, RoutedEventArgs e)
        {
            Utility.OpenLogin();
            this.Close();
        }
        //Creates a new country in database
        private void Opret__Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(Rang_.Text , out _) || !decimal.TryParse(Fødselsrate_.Text, out _) || Verdensdel_.SelectedIndex == -1  || Land_.Text == "" || Land_.Text == " ")
            {
                MessageBox.Show("Please fill out the form correctly");
            }
            else
            {
                SqlUtility sql = new SqlUtility();
                try
                {
                    sql.con = new SqlConnection(SqlUtility.ConnectionString);
                    sql.con.Open();
                    sql.cmd = new SqlCommand("IF NOT EXISTS (SELECT * FROM GeoTema_Rang WHERE Rang = @Rang) INSERT INTO GeoTema_Rang VALUES (@Rang, @Fødselsrate) IF NOT EXISTS (SELECT * FROM GeoTema_Land WHERE Land = @Land) INSERT INTO GeoTema_Land VALUES (@Land, @Verdensdel, @Rang)", sql.con);
                    sql.cmd.Parameters.AddWithValue("@Rang", Rang_.Text);
                    sql.cmd.Parameters.AddWithValue("@Fødselsrate", Fødselsrate_.Text);
                    sql.cmd.Parameters.AddWithValue("@Land", Land_.Text);
                    sql.cmd.Parameters.AddWithValue("@Verdensdel", Verdensdel_.Text);
                    int succes = sql.cmd.ExecuteNonQuery();
                    if (succes == -1)
                    {
                        MessageBox.Show("Entry already exists in the database");
                    }
                    else
                    {
                        MessageBox.Show("Successfully created new entry");
                    }
                    sql.con.Close();
                    FillGrid();
                    Land_.Clear();
                    Rang_.Clear();
                    Fødselsrate_.Clear();
                    Verdensdel_.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
        //Deletes the user selected in the ComboBox
        private void DeleteUser__Click(object sender, RoutedEventArgs e)
        {
            SqlUtility sql = new SqlUtility();
            if (DelUserList_.SelectedIndex != -1)
            {
                try
                {
                    sql.con = new SqlConnection(SqlUtility.ConnectionString);
                    sql.con.Open();
                    sql.cmd = new SqlCommand("DELETE FROM UserAccount where UserName = @UserName", sql.con);
                    sql.cmd.Parameters.AddWithValue("@UserName", DelUserList_.SelectedItem.ToString());
                    sql.reader = sql.cmd.ExecuteReader();
                    MessageBox.Show("Successfully deleted the user");
                    sql.con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Please chose a user first");
            }

        }
    }
}