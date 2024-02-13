using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Vetoshkin41Razmer
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            FrameManager.MainFrame.Navigate(new ProductPage(null));
            UserLogin.Text = "";
            UserPassword.Text = "";
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = UserLogin.Text;
            string password = UserPassword.Text;
            if (login == "" || password == "")
            {
                MessageBox.Show("Есть пустые поля");
                return;
            }

            User user = Vetoshkin_41razmerEntities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                FrameManager.MainFrame.Navigate(new ProductPage(user));
                UserLogin.Text = "";
                UserPassword.Text = "";
            }

            else
            {
                MessageBox.Show("Введены неверные данные");
                Login.IsEnabled = false;
                Login.IsEnabled = true;
            }
        }
    }
}
