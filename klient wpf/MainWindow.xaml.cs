﻿using System;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;

namespace klient_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ZalogujBTN_Click(object sender, RoutedEventArgs e)
        {
            
            //DoubleAnimation da = new DoubleAnimation();
            //da.From = 1;
           // da.To = 0;
            //da.Duration = new Duration(TimeSpan.FromSeconds(0.7));
            //da.Completed += new EventHandler(da_Completed);
            //da.AutoReverse = true;
            //da.RepeatBehavior = RepeatBehavior.Forever;
            //da.RepeatBehavior=new RepeatBehavior(3);
            
            //da.AutoReverse = true;
            //da.RepeatBehavior = RepeatBehavior.Forever;
            //da.RepeatBehavior=new RepeatBehavior(3);
           // this.BeginAnimation(OpacityProperty, da);
            Black blackwindow = new Black();
            blackwindow.Show();
            PokojGlowny main = new PokojGlowny();
            
            App.Current.MainWindow = main;
            //main.Opacity = 0;
            main.Show();
            blackwindow.Close();
            //main.Loaded += new RoutedEventHandler(da1_Completed);
            //MainWindow.BeginAnimation(OpacityProperty, da1);
            //this.GetAnimationBaseValue(da);
            
            //this.BeginAnimation
            //while(da.Completed==null)
            //this.Hide();
            //this.Close();
            
            this.Close();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}