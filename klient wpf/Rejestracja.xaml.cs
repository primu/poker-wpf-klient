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
    /// Interaction logic for Rejestracja.xaml
    /// </summary>
    public partial class Rejestracja : Window
    {
       
        Glowny.GlownySoapClient SerwerGlowny = new Glowny.GlownySoapClient();
        Glowny.Komunikat komunikat = new Glowny.Komunikat();

        public Rejestracja()
        {
            InitializeComponent();
            SSLValidator.OverrideValidation();
        }      

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Show();
            blackwindow.Close();
            this.Close();
        }

        private void ZarejestrujBTN_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                komunikat = SerwerGlowny.Zarejestruj(LoginRej.Text,HasloRej.Password,EmailRej.Text);
                string messageBoxText =komunikat.trescKomunikatu;               
                if (komunikat.kodKomunikatu == 200)
                {//komunikat OK                
                    LBlad.Content = "Zarejestrowano poprawnie.";
                    LBlad.Foreground = Brushes.Green;
                    LBlad.Visibility = Visibility.Visible;
                    //this.Close();
                    LoginRej.Text = "Login";
                    HasloRej.Password = "Hasło";
                    EmailRej.Text = "Email";
                }
                else
                {//jakiś błąd nazwy/adresu email                   
                    //LBlad.Content = "Błąd. Spróbuj ponownie.";
                    LBlad.Content = komunikat.trescKomunikatu;
                    LBlad.Foreground = Brushes.Red;
                    LBlad.Visibility = Visibility.Visible;
                    LoginRej.Text = "Login";
                    HasloRej.Password = "Hasło";
                    EmailRej.Text = "Email";
                }               
            }
            catch (Exception exc)
            {//błąd np. usługa niedostępna             
                LBlad.Content = "Błąd. Nieobsługiwany wyjątek.";
                LBlad.Visibility = Visibility.Visible;
                LoginRej.Text = "Login";
                HasloRej.Password = "Hasło";
                EmailRej.Text = "Email";
                //this.Close();
            }                   
        }
        //===
    }
}
