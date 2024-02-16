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
                        RoleTB.Text = "Администратор"; break;
                    case 2:
                        RoleTB.Text = "Клиент"; break;
                    case 3:
                        RoleTB.Text = "Менеджер"; break;
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


        private List<OrderProduct> currentOrderProds = new List<OrderProduct>();
        private List<Product> selectedProducts = new List<Product>();
        private OrderProduct newOrderProd = new OrderProduct();
        int newOrderID = 0;

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;
                selectedProducts.Add(prod);

                var newOrderProd = new OrderProduct();
                newOrderProd.OrderID = newOrderID;
                newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
                newOrderProd.Quantity = 1;

                var selOP = currentOrderProds.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));
                if (selOP.Count() == 0)
                {
                    currentOrderProds.Add(newOrderProd);
                }
                else
                {
                    foreach(OrderProduct p in currentOrderProds)
                    {
                        if (p.ProductArticleNumber == prod.ProductArticleNumber)
                            p.Quantity++;
                    }
                }

                CheckOrderBTN.Visibility = Visibility.Visible;
                ProductListView.SelectedIndex = -1;
            }
        }

        private void CheckOrderBTN_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void CheckOrderBTN_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();
            OrderWindow orderWindow = new OrderWindow(currentOrderProds, selectedProducts, FIOTB.Text);
            orderWindow.ShowDialog();
        }
    }
}
