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
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        public ProductPage(User user)
        {
            InitializeComponent();
            if (user == null)
            {
                FIOTB.Text = "Гость";
                RoleTB.Text = "Гость";
            }
            else
            {
                FIOTB.Text = user.UserSurname + " " + user.UserName + " " + user.UserPatronymic;
                switch (user.UserRole)
                {
                    case 1:
                        RoleTB.Text = "Клиент"; break;
                    case 2:
                        RoleTB.Text = "Менеджер"; break;
                    case 3:
                        RoleTB.Text = "Администратор"; break;
                }
            }

            var currentProducts = Vetoshkin_41razmerEntities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;
            ProdOverall.Text = Convert.ToString(currentProducts.Count);
            DiscountFilter.SelectedIndex = 0;
            RBClear.IsChecked = true;
            UpdateProducts();
        }

        private void UpdateProducts()
        {
            var currentProducts = Vetoshkin_41razmerEntities.GetContext().Product.ToList();
            currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            if (RBClear.IsChecked.Value)
            {

            }

            if (RBTop.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();
            }

            if (RBBottom.IsChecked.Value)
            {
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            }

            if (DiscountFilter.SelectedIndex == 0)
            {

            }

            if (DiscountFilter.SelectedIndex == 1)
            {
                currentProducts = currentProducts.Where(p => (p.ProductCurrentDiscount >= 0 && p.ProductCurrentDiscount < 10)).ToList();
            }

            if (DiscountFilter.SelectedIndex == 2)
            {
                currentProducts = currentProducts.Where(p => (p.ProductCurrentDiscount >= 10 && p.ProductCurrentDiscount < 15)).ToList();
            }

            if (DiscountFilter.SelectedIndex == 3)
            {
                currentProducts = currentProducts.Where(p => (p.ProductCurrentDiscount >= 15)).ToList();
            }

            ProdRN.Text = Convert.ToString(currentProducts.Count);
            ProductListView.ItemsSource = currentProducts;
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RBTop_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RBBottom_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RBClear_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void DiscountFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }
    }
}
