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
        int chatWSek = 3; //czas dla obu timerów
        int ogolnyWSek = 5; //czas dla obu timerów

        System.Windows.Threading.DispatcherTimer chatTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer ogolnyTimer = new System.Windows.Threading.DispatcherTimer();

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

        Int64 WybranyID = 0;

        int OstatnieOdswiezenie = (Int32)(DateTime.Now.Subtract(new TimeSpan(0, 1, 0)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        int err = 0;

        Glowny.Wiadomosc[] Wiadomosci;

        public PokojGlowny()
        {
            InitializeComponent();
        }
        public PokojGlowny(byte[] token, Int64 id)
        {
            InitializeComponent();
            SSLValidator.OverrideValidation();
            this.token = token;
            this.id = id;

            //Uzytkownicy = SerwerGlowny.ZwrocZalogowanych();
            //foreach (Glowny.Uzytkownik a in Uzytkownicy)
            //{
            //    if (id == a.identyfikatorUzytkownika)
            //        ObecnyUzytkownik = a;
            //}
            
            
            //ObecnyUzytkownik 
            //this.Resources.Add("KeyNazwaUzytkownika", ObecnyUzytkownik.nazwaUzytkownika);

           
            
            PobierzPokoje();
            PobierzWiadomosci();
            PobierzUzytkownikow();

            if (ObecnyUzytkownik.kasiora < 500)
            {
                IUPStawka.Maximum = (int)ObecnyUzytkownik.kasiora;
            }
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
                LVListaPokoi.Items.Add(new Rozgrywki.Pokoj { numerPokoju = p.numerPokoju, nazwaPokoju = p.nazwaPokoju, iloscGraczyObecna = p.iloscGraczyObecna, iloscGraczyMax = p.iloscGraczyMax, stawkaWejsciowa = p.stawkaWejsciowa, graRozpoczeta = p.graRozpoczeta });
            }
            //foreach(ColumnDefinition c in LVListaPokoi.c
            //LVListaPokoi.View.
        }
        private void PobierzUzytkownikow()
        {
            bool jest = false;
            Uzytkownicy = SerwerGlowny.ZwrocZalogowanych(); // try i catch
            LVListaUzytkownikow.Items.Clear();
            foreach (Glowny.Uzytkownik a in Uzytkownicy)
            {
                //LVListaUzytkownikow.Items.Add(new Rozgrywki.Uzytkownik { nazwaUzytkownika = a.nazwaUzytkownika, kasiora = a.kasiora, numerPokoju = a.numerPokoju });
                LVListaUzytkownikow.Items.Add(new { nazwaUzytkownika = a.nazwaUzytkownika, kasiora = a.kasiora, numerPokoju = ZwrocNazwePokoju(a.numerPokoju) });
                if (id == a.identyfikatorUzytkownika)
                {
                    ObecnyUzytkownik = a;
                    jest = true;
                }
            }
            if (!jest)
            {
                komunikat = SerwerGlowny.Wyloguj(token);
                MessageBox.Show("Wystąpił problem z połączeniem. Nastąpiło wylogowanie.");
                PrzejdzDoOknaLogowania();
            }
        }

        private string ZwrocNazwePokoju(long idPok)
        {
            if (idPok == 0)
                return "Główny";
            for (int i = 0; i < Pokoje.Length; i++)
            {
                if (Pokoje[i].numerPokoju == idPok)
                {
                    return Pokoje[i].nazwaPokoju;
                }
            }
            return "";
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Wyloguj();
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            UstawRanking();
            Overlay.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // dołączanie do wybranego pokoju
        {
            //PrzejdzDoPokoju();
            if (WybranyID == 0)
            {
                MessageBox.Show("Wybierz jakiś pokój!");
            }
            else
            {
                komunikatR = SerwerRozgrywki.DolaczDoStolu(token, WybranyID); //try i catch w przypadku braku połączenia...
                if (komunikatR.kodKomunikatu == 404)
                {
                    MessageBox.Show("Wystąpił błąd! Spróbuj wejść do pokoju ponownie.");
                }else
                    PrzejdzDoPokoju(WybranyID);
            }
        }
        private void Wyloguj()
        {
            chatTimer.Stop();
            ogolnyTimer.Stop();

            if (MessageBox.Show("Czy na pewno chcesz się wylogować?", "Wyloguj", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                chatTimer.Start();
                ogolnyTimer.Start();
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
            try
            {
                blackwindow.Show();

                PokojGry main = new PokojGry(token, id, idPokoju);

                //main.token = token;
                //main.id = id;

                App.Current.MainWindow = main;
                main.Show();
                blackwindow.Close();
                this.Close();
            }
            catch (Exception ee)
            {
                blackwindow.Close();
                MessageBox.Show("Nie można przejść do pokoju! Spróbuj ponownie.");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            chatTimer.Tick += new EventHandler(chatTimer_Tick);
            chatTimer.Interval = new TimeSpan(0, 0, chatWSek);
            chatTimer.Start();

            ogolnyTimer.Tick += new EventHandler(ogolnyTimer_Tick);
            ogolnyTimer.Interval = new TimeSpan(0, 0, ogolnyWSek);
            ogolnyTimer.Start();
        }
        private void chatTimer_Tick(object sender, EventArgs e)
        {

            PobierzWiadomosci();
        }

        private void ogolnyTimer_Tick(object sender, EventArgs e)
        {

            try
            {
                PobierzPokoje();
                PobierzUzytkownikow();
                //PobierzWiadomosci();
                err = 0;

            }
            catch (Exception ee)
            {
                err++;
                if (err == 5)
                {
                    MessageBox.Show("Wystąpił problem z połączeniem! Nastąpi wylogowanie.");
                }
            }

        }

        //private void WyswietlWiadomosc()
        //{
        //   TBOknoCzatuPara.Inlines.Add(new Run("Siema"));
        //    
        //}
        private void PobierzWiadomosci()
        {
            Wiadomosci = SerwerGlowny.PobierzWiadomosci(token, OstatnieOdswiezenie, 0);
            OstatnieOdswiezenie = (Int32)(DateTime.Now.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            if (Wiadomosci != null)
            {
                foreach (Glowny.Wiadomosc w in Wiadomosci)
                {
                    if (w.nazwaUzytkownika == ObecnyUzytkownik.nazwaUzytkownika)
                    {
                        TBOknoCzatuPara.Inlines.Add(new Bold(new Run { Text = w.nazwaUzytkownika + ": ", Foreground = Brushes.RoyalBlue }));
                        TBOknoCzatuPara.Inlines.Add(new Run { Text = w.trescWiadomosci, Foreground = Brushes.SteelBlue });
                    }
                    else
                    {
                        TBOknoCzatuPara.Inlines.Add(new Bold(new Run(w.nazwaUzytkownika + ": ")));
                        TBOknoCzatuPara.Inlines.Add(new Run(w.trescWiadomosci));
                    }
                    TBOknoCzatuPara.Inlines.Add(new LineBreak());
                }
                RTBa.ScrollToEnd();
            }
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
                if (komunikatR.kodKomunikatu == 404)
                {
                    MessageBox.Show("Wystąpił błąd! Spróbuj wejść do pokoju ponownie.");
                }
                else
                    PrzejdzDoPokoju(idPokoju);
            }
            else
            {
                MessageBox.Show("Wpisz nazwę stołu!");
            }

        }

        private void LVListaPokoi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((Rozgrywki.Pokoj)LVListaPokoi.SelectedItem != null)
                WybranyID = ((Rozgrywki.Pokoj)LVListaPokoi.SelectedItem).numerPokoju;
        }

        private void TBPoleCzatu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (TBPoleCzatu.Text != "")
                {
                    try
                    {
                        komunikat = SerwerGlowny.WyslijWiadomosc(token, new Glowny.Wiadomosc { nazwaUzytkownika = ObecnyUzytkownik.nazwaUzytkownika, stempelCzasowy = 0, numerPokoju = 0, trescWiadomosci = TBPoleCzatu.Text });

                        //TBOknoCzatuPara.Inlines.Add(new Bold(new Run(ObecnyUzytkownik.nazwaUzytkownika + ": ")));
                        //TBOknoCzatuPara.Inlines.Add(new Run(TBPoleCzatu.Text));
                        //TBOknoCzatuPara.Inlines.Add(new LineBreak());
                        PobierzWiadomosci();

                        TBPoleCzatu.Text = "";
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatTimer.Stop();
            ogolnyTimer.Stop();
        }

        private void TBLStart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Overlay.Visibility = Visibility.Hidden;
        }

        private void UstawRanking()
        {
            //TBLUzytkownik.Inlines.Add(new Run("Witaj "));
            //TBLUzytkownik.Inlines.Add(new Run() { Text = ObecnyUzytkownik.nazwaUzytkownika, FontWeight = FontWeights.Bold, Foreground = Brushes.Red });
            //TBLUzytkownik.Inlines.Add(new Run("!"));
            //TBLUzytkownik.Inlines.Add(new LineBreak());
            //TBLUzytkownik.Inlines.Add(new Run() { Text = "Twoje środki: ", FontWeight = FontWeights.Bold });
            //TBLUzytkownik.Inlines.Add(new Run(ObecnyUzytkownik.kasiora.ToString()));
            try
            {
                Glowny.Uzytkownik[] najL = SerwerGlowny.PobierzListeNajlepszych(token);
                Glowny.Uzytkownik[] najB = SerwerGlowny.PobierzListeNajbogatszych(token);
                if (najL != null)
                {
                    TBLZwyciezcy.Inlines.Add(new Run() { Text = "Lp.\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLZwyciezcy.Inlines.Add(new Run() { Text = "Nazwa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLZwyciezcy.Inlines.Add(new Run() { Text = "Kasa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLZwyciezcy.Inlines.Add(new Run() { Text = "Zwycięstwa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLZwyciezcy.Inlines.Add(new LineBreak());
                    for (int i = 0; i < najL.Length; i++)
                    {
                        if (i == 0)
                        {
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            
                        }else
                        if (i == 1)
                        {
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                            TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                            
                        }else
                            if (i == 2)
                            {
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 18 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                            }
                            else
                            {
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                TBLZwyciezcy.Inlines.Add(new Run() { Text = najL[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                            }
                        TBLZwyciezcy.Inlines.Add(new LineBreak());
                    }
                }
                if (najB != null)
                {
                    TBLBogacze.Inlines.Add(new Run() { Text = "Lp.\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLBogacze.Inlines.Add(new Run() { Text = "Nazwa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLBogacze.Inlines.Add(new Run() { Text = "Kasa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLBogacze.Inlines.Add(new Run() { Text = "Zwycięstwa\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 18 });
                    TBLBogacze.Inlines.Add(new LineBreak());
                    for (int i = 0; i < najL.Length; i++)
                    {
                        if (i == 0)
                        {
                            TBLBogacze.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLBogacze.Inlines.Add(new Run() { Text = najB[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLBogacze.Inlines.Add(new Run() { Text = najB[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });
                            TBLBogacze.Inlines.Add(new Run() { Text = najB[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.Red, FontSize = 18 });

                        }
                        else
                            if (i == 1)
                            {
                                TBLBogacze.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                                TBLBogacze.Inlines.Add(new Run() { Text = najB[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                                TBLBogacze.Inlines.Add(new Run() { Text = najB[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });
                                TBLBogacze.Inlines.Add(new Run() { Text = najB[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.OrangeRed, FontSize = 16 });

                            }
                            else
                                if (i == 2)
                                {
                                    TBLBogacze.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 18 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.Tomato, FontSize = 14 });
                                }
                                else
                                {
                                    TBLBogacze.Inlines.Add(new Run() { Text = i.ToString() + ".\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].nazwaUzytkownika + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].kasiora + "\t", FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                    TBLBogacze.Inlines.Add(new Run() { Text = najB[i].identyfikatorUzytkownika.ToString(), FontWeight = FontWeights.Bold, Foreground = Brushes.White, FontSize = 12 });
                                }
                        TBLBogacze.Inlines.Add(new LineBreak());
                    }
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }

        }
    }
}
