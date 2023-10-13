﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, int> drinks = new Dictionary<string, int>();
        Dictionary<string, int> orders = new Dictionary<string, int>();
        string takeout = "";
        public MainWindow()
        {
            InitializeComponent();
            AddNewDrink(drinks);
            DisplayDrinkMenu(drinks);
        }

        private void DisplayDrinkMenu(Dictionary<string, int> myDrinks)
        {
            foreach(var drink in myDrinks)
            {
                StackPanel sp = new StackPanel();
                CheckBox cb = new CheckBox();
                Slider sl = new Slider();
                Label lb = new Label();

                cb.Content = $"{drink.Key} : {drink.Value}元";
                cb.FontFamily = new FontFamily("Consolas");
                cb.FontSize = 18;
                cb.Foreground = Brushes.Red;
                cb.Width = 200;
                //cb.Height = 60;
                cb.Margin = new Thickness(5);

                sl.Width = 100;
                //sl.Height = 60;
                sl.Value = 0;
                sl.Minimum = 0;
                sl.Maximum = 10;//拉條數量
                sl.IsSnapToTickEnabled = true;//拉調整數
                sl.TickPlacement = TickPlacement.BottomRight;

                lb.Width = 50;
                //lb.Height = 60;
                lb.Content = "0";
                lb.FontFamily = new FontFamily("Consolas");
                lb.FontSize = 18;

                sp.Orientation = Orientation.Horizontal;
                sp.Margin = new Thickness(5);
                //sp.Height = 60;
                sp.Children.Add(cb);
                sp.Children.Add(sl);
                sp.Children.Add(lb);

                Binding myBunding = new Binding("Value");
                myBunding.Source = sl;
                lb.SetBinding(ContentProperty, myBunding);

                stachpane_DrinkMenu.Children.Add(sp);
            }
        }

        private void AddNewDrink(Dictionary<string, int> drinks)
        {
            //drinks.Add("紅茶大杯", 60);
            //drinks.Add("紅茶小杯", 40);
            //drinks.Add("綠茶大杯", 60);
            //drinks.Add("綠茶小杯", 40);
            //drinks.Add("咖啡大杯", 80);
            //drinks.Add("咖啡小杯", 60);
            //drinks.Add("可樂大杯", 50);
            //drinks.Add("可樂小杯", 30);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV檔案|*.csv|文字檔案|*.txt|全部檔案|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filename = openFileDialog.FileName;
                //string alltext = File.ReadAllText(filename);
                string[] lines = File.ReadAllLines(filename);
                foreach (var line in lines)
                {
                    string[] tokens = line.Split(',');
                    string drinkName = tokens[0];
                    int price = Convert.ToInt32(tokens[1]);
                    drinks.Add(drinkName, price);
                }
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            PlaceOrder(orders);

            DisplayOrderDetail(orders);
        }

        private void DisplayOrderDetail(Dictionary<string, int> myOrders)
        {
            displayTextBlock.Inlines.Clear();
            Run titleString = new Run
            {
                Text = "您所訂購的飲品:",
                Foreground = Brushes.Blue,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            Run takeoutString = new Run
             {
                Text = $"{takeout}\n",
                Background = Brushes.Aqua,
                FontSize = 16,
                FontWeight = FontWeights.Bold
             };

            displayTextBlock.Inlines.Add(titleString);
            displayTextBlock.Inlines.Add(takeoutString);
            displayTextBlock.Inlines.Add(new Run(" ˙訂購名下圖下 : \n"));

            string discountMessage = "";

            double total = 0.0;
            double sellPrice = 0.0;
            int i = 1;

            foreach (var item in myOrders)
            {
                string drinkName = item.Key;
                int quantity = myOrders[drinkName];
                int price = drinks[drinkName];

                total += quantity * price;
                Run detailString = new Run($"訂購品項{i}:{drinkName} X {quantity}杯˙總共{price * quantity}元\n");
                displayTextBlock.Inlines.Add(detailString);
                i++;
            }
            if (total >= 500)
            {
                discountMessage = "訂購滿500元以上者8折";
                sellPrice = total * 0.8;
            }
            else if (total >= 300)
            {
                discountMessage = "訂購滿300元以上者85折";
                sellPrice = total * 0.85;
            }
            else if (total >= 200)
            {
                discountMessage = "訂購滿200元以上者9折";
                sellPrice = total * 0.9;
            }
            else
            {
                discountMessage = "訂購未滿200元不打折";
                sellPrice = total;
            }

            Italic summaryString = new Italic(new Run
            {
                Text =$"本次訂購總共{myOrders.Count}項，總共{total}元，{discountMessage}，售價{sellPrice}元。",
                Foreground = Brushes.Red
            });
            displayTextBlock.Inlines.Add(summaryString);
        }

        private void PlaceOrder(Dictionary<string, int> myOrders)
        {
            myOrders.Clear();
            for (int i = 0; i < stachpane_DrinkMenu.Children.Count; i++)
            {
                var sp = stachpane_DrinkMenu.Children[i] as StackPanel;
                var cb = sp.Children[0] as CheckBox;
                var sl = sp.Children[1] as Slider;
                String drinkName = cb.Content.ToString().Substring(0, 4);
                int quantity = Convert.ToInt32(sl.Value);

                if (cb.IsChecked == true && quantity != 0)
                {
                    myOrders.Add(drinkName, quantity);
                }
            }
        }
        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb.IsChecked == true)
            {
                takeout = rb.Content.ToString();
                //MessageBox.Show(takeout);
            }
        }
    }
}

