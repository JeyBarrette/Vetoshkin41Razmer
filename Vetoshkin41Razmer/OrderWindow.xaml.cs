using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vetoshkin41Razmer
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct = new OrderProduct();
        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO)
        {
            InitializeComponent();
            var currentPickups = Vetoshkin_41razmerEntities.GetContext().PickupPoint.ToList();
            PickupCombo.ItemsSource = currentPickups.Select(x => $"{x.OrderPickupPointID}, {x.City}, {x.Adress}, {x.HouseNumber}");

            ClientTB.Text = FIO;
            TBOrderID.Text = selectedOrderProducts.First().OrderID.ToString();

            ShoeOrderList.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.Quantity = 1;
                foreach (OrderProduct q in selectedOrderProducts)
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                        p.Quantity = q.Quantity;
                }
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            FormationDate.Text = DateTime.Now.ToString();
            //SetDeliveryDate();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity--;
            if (prod.Quantity < 1)
                MessageBox.Show("Товаров не может быть меньше одного.");
            else
            {
                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                int index = selectedOrderProducts.IndexOf(selectedOP);
                selectedOrderProducts[index].Quantity--;
                //SetDeliveryDate();
                ShoeOrderList.Items.Refresh();
            }
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;
            if (prod.Quantity > prod.ProductQuantityInStock)
                MessageBox.Show("Недостаточно товаров на складе.");
            else
            {
                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
                int index = selectedOrderProducts.IndexOf(selectedOP);
                selectedOrderProducts[index].Quantity++;
                //SetDeliveryDate();
                ShoeOrderList.Items.Refresh();
            }
        }
    }
}
