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
        System.Windows.Threading.DispatcherTimer chatTimer = new System.Windows.Threading.DispatcherTimer();

        public byte[] token;
        public Int64 id;

        Glowny.GlownySoapClient SerwerGlowny = new Glowny.GlownySoapClient();
        Rozgrywki.RozgrywkiSoapClient SerwerRozgrywki = new Rozgrywki.RozgrywkiSoapClient();
        Glowny.Komunikat komunikat = new Glowny.Komunikat();
        Rozgrywki.Komunikat komunikatR = new Rozgrywki.Komunikat();

        Glowny.Uzytkownik[] Uzytkownicy;

        Glowny.Uzytkownik ObecnyUzytkownik;

        //IList<Rozgrywki.Pokoj> Pokoje { get; set; }
        Rozgrywki.Pokoj[] Pokoje;

        public PokojGlowny()
        {
            InitializeComponent();
        }
        public PokojGlowny(byte[] token, Int64 id)
        {
            InitializeComponent();
            this.token = token;
            this.id = id;
            Uzytkownicy = SerwerGlowny.ZwrocZalogowanych();
            foreach (Glowny.Uzytkownik a in Uzytkownicy)
            {
                if (id == a.identyfikatorUzytkownika)
                    ObecnyUzytkownik = a;
            }
            //ObecnyUzytkownik 
            //this.Resources.Add("KeyNazwaUzytkownika", ObecnyUzytkownik.nazwaUzytkownika);

            if (ObecnyUzytkownik.kasiora < 500)
            {
                IUPStawka.Maximum = (int)ObecnyUzytkownik.kasiora;
            }
            
            PobierzPokoje();
            //LUzytkownik.Content = "<span>Witaj <Bold>" + ObecnyUzytkownik.nazwaUzytkownika + "</Bold><LineBreak/><Bold>Twoje środki: </Bold>"+ObecnyUzytkownik.kasiora.ToString()+"</span>";
            TBLUzytkownik.Inlines.Add(new Run("Witaj "));
            TBLUzytkownik.Inlines.Add(new Run() { Text = ObecnyUzytkownik.nazwaUzytkownika, FontWeight = FontWeights.Bold, Foreground=Brushes.Red });
            TBLUzytkownik.Inlines.Add(new Run("!"));
            TBLUzytkownik.Inlines.Add(new LineBreak());
            TBLUzytkownik.Inlines.Add(new Run() { Text = "Twoje środki: ", FontWeight = FontWeights.Bold });
            TBLUzytkownik.Inlines.Add(new Run(ObecnyUzytkownik.kasiora.ToString()));

        }
        //public void PrzekazUzytkownika(byte[] token, Int64 id)
        //{
        //    this.token = token;
        //    this.id = id;
        //    Uzytkownicy = SerwerGlowny.ZwrocZalogowanych();
        //    foreach (Glowny.Uzytkownik a in Uzytkownicy)
        //    {
        //        if (id == a.identyfikatorUzytkownika)
        //            ObecnyUzytkownik = a;
        //    }
        //    //ObecnyUzytkownik 
        //    this.Resources.Add("KeyNazwaUzytkownika", ObecnyUzytkownik.nazwaUzytkownika);
        //}

        private void PobierzPokoje()
        {
            
            Pokoje = SerwerRozgrywki.PobierzPokoje(token);
            LVListaPokoi.Items.Clear();
            foreach (Rozgrywki.Pokoj p in Pokoje)
            {
                LVListaPokoi.Items.Add(new { numerPokoju = p.numerPokoju, nazwaPokoju = p.nazwaPokoju, iloscGraczyObecna = p.iloscGraczyObecna, iloscGraczyMax = p.iloscGraczyMax, stawkaWejsciowa = p.stawkaWejsciowa, graRozpoczeta = p.graRozpoczeta });
            }
            //foreach(ColumnDefinition c in LVListaPokoi.c
            //LVListaPokoi.View.
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

        private void Button_Click(object sender, RoutedEventArgs e) // dołączanie do wybranego pokoju
        {
            PrzejdzDoPokoju();
        }
        private void Wyloguj()
        {
            chatTimer.Stop();
            if (MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wyloguj", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                chatTimer.Start();
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
        private void PrzejdzDoPokoju(Int64 idPokoju)
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            PokojGry main = new PokojGry(token,id,idPokoju);

            main.token = token;
            main.id = id;

            App.Current.MainWindow = main;
            main.Show();
            blackwindow.Close();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            chatTimer.Tick += new EventHandler(chatTimer_Tick);
            chatTimer.Interval = new TimeSpan(0, 0, 5);
            chatTimer.Start();
        }
        private void chatTimer_Tick(object sender, EventArgs e)
        {
            // code goes here
            PobierzPokoje();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) //Tworzenie pokoju
        {
            if (TBNazwaStolu.Text != "")
            {
                komunikatR = SerwerRozgrywki.UtworzStol(token, TBNazwaStolu.Text, (int)IUPStawka.Value, (int)IUPBlind.Value, (int)SLiczbaGraczy.Value);
                PobierzPokoje();
                Int64 idPokoju = 0;
                foreach (Rozgrywki.Pokoj p in Pokoje)
                {
                    if (TBNazwaStolu.Text == p.nazwaPokoju)
                    {
                        idPokoju = p.numerPokoju;
                    }
                }
                komunikatR = SerwerRozgrywki.DolaczDoStolu(token, idPokoju);
                PrzejdzDoPokoju(idPokoju);
            }
            else
            {
                MessageBox.Show("Wpisz nazwę stołu!");
            }

        }
    }
}
