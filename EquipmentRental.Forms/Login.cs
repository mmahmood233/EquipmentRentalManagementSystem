using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    
    public partial class Login : Form
    {
        private EquipmentRentalDbContext context;

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            context = new EquipmentRentalDbContext();
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string usernameOrEmail = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username/email and password.");
                return;
            }

            var user = context.Users
                .Where(u => u.Email == usernameOrEmail || u.FullName == usernameOrEmail)
                .FirstOrDefault();

            if (user == null)
            {
                MessageBox.Show("User not found.");
                return;
            }

            if (!user.IsActive.GetValueOrDefault())
            {
                MessageBox.Show("Your account is inactive.");
                return;
            }

            if (VerifyPasswordHash(password, user.PasswordHash))
            {
                MessageBox.Show("Login successful!");

                var userRole = context.UserRoles
                                      .Where(ur => ur.RoleId == user.RoleId)
                                      .FirstOrDefault();

                if (userRole != null)
                {

                    GlobalUserInfo.UserId = user.UserId;
                    GlobalUserInfo.UserRole = userRole.RoleName;
                    GlobalUserInfo.UserName = user.FullName;

                    switch (userRole.RoleName)
                    {
                        case "Administrator":
                        case "Manager":
                            AdminHomePage adminHomePage = new AdminHomePage();
                            adminHomePage.Show();
                            this.Hide();
                            break;

                        case "Customer":
                            UserHomePage userHomePage = new UserHomePage();
                            userHomePage.Show();
                            this.Hide();
                            break;

                        default:
                            MessageBox.Show("Unknown user role.");
                            break;
                    }
                }
                else
                {
                    MessageBox.Show("Role not found.");
                }
            }
            else
            {
                MessageBox.Show("Invalid password.");
            }
        }

        // Method to verify the password hash using SHA-256
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                // Hash the entered password
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); 

                return storedHash == hashString;
            }
        }
    }
}
