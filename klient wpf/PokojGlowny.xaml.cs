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
using System.Windows.Shapes;

namespace klient_wpf
{
    /// <summary>
    /// Interaction logic for PokojGlowny.xaml
    /// </summary>
    public partial class PokojGlowny : Window
    {
        public PokojGlowny()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            MainWindow main = new MainWindow();

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
    }
}
