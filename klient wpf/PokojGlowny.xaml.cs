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

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBlock).ContextMenu.IsEnabled = true;
            (sender as TextBlock).ContextMenu.PlacementTarget = (sender as TextBlock);
            (sender as TextBlock).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as TextBlock).ContextMenu.IsOpen = true;
        }

        private void Top10_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Overlay.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            PokojGry main = new PokojGry();

            App.Current.MainWindow = main;
            main.Show();
            blackwindow.Close();
            this.Close();
        }
    }
}
