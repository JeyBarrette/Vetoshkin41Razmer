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
        //User currentUser;
        private double Cost = 0;

        private int SetDeliveryDay(List<Product> products)
        {

            int k = 0;
            foreach (var p in products)
            {
                k += p.Quantity;
            }

            if (k <= 3)
                return 6;
            else
                return 3;
        }

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO)
        {
            InitializeComponent();
            //currentUser = user;
            Cost = 0;
            var currentPickups = Vetoshkin_41razmerEntities.GetContext().PickupPoint.ToList();
            PickupCombo.ItemsSource = currentPickups.Select(x => $"{x.OrderPickupPointID}, {x.City}, {x.Adress}, {x.HouseNumber}");
            PickupCombo.SelectedIndex = 0;
            int currentID = selectedOrderProducts.First().OrderID;
            currentOrder.OrderID = currentID;

            ClientTB.Text = FIO;
            //TBOrderID.Text = selectedOrderProducts.First().OrderID.ToString();


            List<Order> allOrderCodes = Vetoshkin_41razmerEntities.GetContext().Order.ToList();
            List<int> OrderCodes = new List<int>();
            foreach (var p in allOrderCodes.Select(x => $"{x.OrderCodeNumber}").ToList())
            {
                OrderCodes.Add(Convert.ToInt32(p));
            }
            Random random = new Random();

            while (true)
            {
                int num = random.Next(100, 1000);
                if (!OrderCodes.Contains(num))
                {
                    currentOrder.OrderCodeNumber = num;
                    break;
                }
            }

            TBOrderID.Text = currentID.ToString();
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
            DeliveryDate.Text = DateTime.Now.ToString();
            DeliveryDate.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();

            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantity;
            }

            TotalCost.Text = Cost.ToString();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderPickupPoint = PickupCombo.SelectedIndex + 1;
            currentOrder.OrderDate = DateTime.Now;
            currentOrder.OrderDeliveryDate = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts));
            currentOrder.OrderStatus = "Новый";
            for (int i = 0; i < selectedProducts.Count; i++)
            {
                selectedProducts[i].ProductQuantityInStock -= selectedOrderProducts[i].Quantity;
            }
            foreach (var p in selectedOrderProducts)
            {
                Vetoshkin_41razmerEntities.GetContext().OrderProduct.Add(p);
            }

            Vetoshkin_41razmerEntities.GetContext().Order.Add(currentOrder);

            try
            {
                Vetoshkin_41razmerEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            this.Close();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity--;
            Cost = 0;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            if (prod.Quantity == 0)
            {
                selectedOrderProducts[index].Quantity = 0;
                var pr = ShoeOrderList.SelectedItem as Product;
                selectedOrderProducts.RemoveAt(index);
                selectedProducts.RemoveAt(index);
                if (ShoeOrderList.Items.Count == 0)
                {
                    this.Close();
                }
            }
            else
            {
                selectedOrderProducts[index].Quantity--;
                if (this.selectedProducts[index].ProductQuantityInStock > prod.Quantity)
                    prod.ProductQuantityInStock++;
            }
            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantity;
            }
            DeliveryDate.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();
            TotalCost.Text = Cost.ToString();
            ShoeOrderList.Items.Refresh();
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            Cost = 0;
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;
            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedOrderProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].Quantity++;
            if (prod.ProductQuantityInStock > 0)
            {
                prod.ProductQuantityInStock--;
            }
            ShoeOrderList.Items.Refresh();
            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantity;
            }
            TotalCost.Text = Cost.ToString();
            DeliveryDate.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity = 0;
            var selectedOP = selectedProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            int index = selectedProducts.IndexOf(selectedOP);
            selectedOrderProducts[index].Quantity = 0;
            var pr = ShoeOrderList.SelectedItem as Product;
            selectedOrderProducts.RemoveAt(index);
            selectedProducts.RemoveAt(index);

            for (int i = 0; i < selectedProducts.Count; i++)
            {
                Cost += (Convert.ToDouble(selectedProducts[i].ProductCost) - Convert.ToDouble(selectedProducts[i].ProductCost) * Convert.ToDouble(selectedProducts[i].ProductDiscountAmount) / 100) * selectedProducts[i].Quantity;
            }
            TotalCost.Text = Cost.ToString();
            ShoeOrderList.Items.Refresh();
            DeliveryDate.Text = DateTime.Now.AddDays(SetDeliveryDay(selectedProducts)).ToString();
            if (ShoeOrderList.Items.Count == 0)
            {
                this.Close();
            }
        }
    }
}
