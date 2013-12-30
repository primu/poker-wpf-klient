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
//using Xceed.Wpf.Toolkit;

namespace klient_wpf
{
    /// <summary>
    /// Interaction logic for PokojGlowny.xaml
    /// </summary>
    public partial class PokojGlowny : Window
    {
        public byte[] token;
        public Int64 id;

        Glowny.GlownySoapClient SerwerGlowny = new Glowny.GlownySoapClient();
        Rozgrywki.RozgrywkiSoapClient SerwerRozgrywki = new Rozgrywki.RozgrywkiSoapClient();
        Glowny.Komunikat komunikat = new Glowny.Komunikat();

        public PokojGlowny()
        {
            InitializeComponent();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Wyloguj();
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
            PrzejdzDoPokoju();
        }
        private void Wyloguj()
        {
            if (MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wyloguj", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                //do no stuff
            }
            else
            {
                komunikat = SerwerGlowny.Wyloguj(token);
                PrzejdzDoOknaLogowania();
                //if (komunikat.kodKomunikatu == 200)
                //{
                //    PrzejdzDoOknaLogowania();
                //}
                //else
                //    MessageBox.Show(komunikat.trescKomunikatu);
            }
        }
        private void PrzejdzDoOknaLogowania()
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
        private void PrzejdzDoPokoju()
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            PokojGry main = new PokojGry();

            main.token = token;
            main.id = id;

            App.Current.MainWindow = main;
            main.Show();
            blackwindow.Close();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer chatTimer = new System.Windows.Threading.DispatcherTimer();
            chatTimer.Tick += new EventHandler(chatTimer_Tick);
            chatTimer.Interval = new TimeSpan(0, 0, 5);
            chatTimer.Start();
        }
        private void chatTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
        }
    }
}
