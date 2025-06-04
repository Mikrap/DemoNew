
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using Demo.Pages;
using System.Windows;
using System;

namespace Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private readonly string _connectionString = "Server=MySQL-8.4;Database=simple_auth;Uid=root;Pwd='';";
        public AuthPage()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Логин и пароль не могут быть пустыми!";
                return;
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                SELECT u.Password, r.RoleName 
                FROM Users u
                JOIN Roles r ON u.RoleID = r.RoleID
                WHERE u.Login = @login";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@login", login);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["Password"].ToString();
                                string roleName = reader["RoleName"].ToString();

                                if (password == storedPassword)
                                {
                                    var mainWindow = (MainWindow)Window.GetWindow(this);

                                    // Автоматическое определение роли по данным из БД
                                    if (roleName == "Admin")
                                        mainWindow.MainFrame.Navigate(new AdminPage());
                                    else
                                        mainWindow.MainFrame.Navigate(new UserPage());
                                }
                                else
                                {
                                    lblMessage.Text = "Неверный пароль!";
                                }
                            }
                            else
                            {
                                lblMessage.Text = "Пользователь не найден!";
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    lblMessage.Text = "Ошибка подключения к базе данных!";
                    Console.WriteLine($"Database error: {ex.Message}");
                }
            }
        }
    }
    
}
