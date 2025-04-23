using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using EquipmentRental.DataAccess.Models;

namespace EquipmentRental.Forms
{
    public partial class Login : Form
    {
        EquipmentRentalDbContext context;
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
            string usernameOrEmail = txtUsername.Text.Trim();  // Assuming txtUsername is the TextBox for Email or Username
            string password = txtPassword.Text.Trim();  // Assuming txtPassword is the TextBox for Password

            // Check if the fields are not empty
            if (string.IsNullOrEmpty(usernameOrEmail) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username/email and password.");
                return;
            }

            // Authenticate user from the database by either username or email
            var user = context.Users.SingleOrDefault(u => u.Email == usernameOrEmail || u.FullName == usernameOrEmail);

            if (user == null)
            {
                MessageBox.Show("User not found.");
                return;
            }

            // Check if the user account is active
            if (user.IsActive == false)
            {
                MessageBox.Show("Your account is inactive.");
                return;
            }

            if (VerifyPasswordHash(password, user.PasswordHash))
            {
                MessageBox.Show("Login successful!");
               
            }
            else
            {
                MessageBox.Show("Invalid password.");
            }
        }

        // Method to verify password hash (SHA256 hashing example)
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                Console.WriteLine("Entered password hash: " + hashString);
                Console.WriteLine("Stored password hash: " + storedHash);

                return storedHash == hashString;
            }
        }




    }
}
