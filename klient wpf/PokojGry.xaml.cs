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
using System.Windows.Shapes;
using klient_wpf.Glowny;
using klient_wpf.Rozgrywki;

namespace klient_wpf
{
    /// <summary>
    /// Interaction logic for PokojGry.xaml
    /// </summary>
    public partial class PokojGry : Window
    {
        System.Windows.Threading.DispatcherTimer chatTimer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer ogolnyTimer = new System.Windows.Threading.DispatcherTimer();

        public byte[] token;
        public Int64 id;
        public Int64 idPokoju;

        Glowny.GlownySoapClient SerwerGlowny = new Glowny.GlownySoapClient();
        Rozgrywki.RozgrywkiSoapClient SerwerRozgrywki = new Rozgrywki.RozgrywkiSoapClient();
        Glowny.Komunikat komunikat = new Glowny.Komunikat();
        Rozgrywki.Komunikat komunikatR = new Rozgrywki.Komunikat();
        Rozgrywki.Gra gra = null;


        Rozgrywki.Uzytkownik ObecnyUzytkownik;
        Rozgrywki.Uzytkownik[] Uzytkownicy;

        Rozgrywki.Gracz[] Gracze;
        Rozgrywki.Gracz ja;        

        Rozgrywki.Pokoj ObecnyStol;

        Image[] karo = new Image[13];
        Image[] kier = new Image[13];
        Image[] pik = new Image[13];
        Image[] trefl = new Image[13];

        Image[] tlo = new Image[4];

        Glowny.Wiadomosc[] Wiadomosci;
        int OstatnieOdswiezenie = (Int32)(DateTime.Now.Subtract(new TimeSpan(0, 1, 0)).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

        //int[,] miejscePrzyStole = new int[8,2];
        int[,] miejscePrzyStole;

        bool s = false;

        public PokojGry()
        {
            InitializeComponent();
            //var karta = (Image)G1.Children[0];
            //karta = karo[0];
            
            ZaladujKarty();
            
            UsunWszystkieKarty();
            //ZmienKarte(ref Stol, 0, ref kier[11]);
            //ZmienKarte(ref Stol, 1, ref trefl[11]);
            //ZmienKarte(ref Stol, 2, ref pik[11]);
            //UstawGracza(1, "primu", 150000,12,true,true,false,true);
            //ZmienKarte(ref G1, 0, ref pik[12]);
            //ZmienKarte(ref G1, 1, ref kier[12]);
            //UstawGracza(6, "Paweł", 1500,0, true, false, false, false, true);
            //ZmienKarte(ref G6, 0, ref pik[2]);
            //ZmienKarte(ref G6, 1, ref karo[3]);
            //UstawGracza(2, "Marcin", 0, 100, true);
            //ZmienKarte(ref G2, 0, ref kier[2]);
            //ZmienKarte(ref G2, 1, ref trefl[3]);
            //UstawGracza(8, "Komputer", 10, 199, true, true, true);
            //ZmienKarte(ref G8, 0, ref pik[4]);
            //ZmienKarte(ref G8, 1, ref trefl[4]);
            //UstawGracza(3);
            //UstawGracza(4);
            //UstawGracza(5);
            //UstawGracza(7);

        }
        private void Nakladka(bool widoczna = false)
        {
            if (widoczna && Overlay.Visibility == Visibility.Visible)
            {
                UzytkownicyNaStart();
            }
            else if (widoczna)
            {
                Overlay.Visibility = Visibility.Visible;
                UzytkownicyNaStart();
            }
            else
                Overlay.Visibility = Visibility.Hidden;
            //LStart.Visibility = Visibility.Visible;
            //LOczekiwanie.Visibility = Visibility.Visible;
            

        }
        private void UzytkownicyNaStart()
        {
            TBLUzytkownicyStart.Inlines.Clear();
            foreach (Rozgrywki.Uzytkownik u in Uzytkownicy)
            {
                TBLUzytkownicyStart.Inlines.Add(new Run() { Text = u.nazwaUzytkownika, Foreground = u.start ? Brushes.Green : Brushes.Red });
                TBLUzytkownicyStart.Inlines.Add(new LineBreak());
            }
        }
        private void PobierzObecnyStol()
        {
            Rozgrywki.Pokoj[] temp = SerwerRozgrywki.PobierzPokoje(token);
            foreach (Rozgrywki.Pokoj p in temp)
            {
                //if (ObecnyUzytkownik.numerPokoju == p.numerPokoju)
                if (idPokoju == p.numerPokoju)
                    ObecnyStol = p;
            }
        }
        public PokojGry(byte[] token, Int64 id, Int64 nrPokoju)
        {
            InitializeComponent();
            idPokoju = nrPokoju;
            this.token = token;
            this.id = id;
            //var karta = (Image)G1.Children[0];
            //karta = karo[0];
            //ObecnyStol = SerwerRozgrywki.zwrocStol(token);

            //ObecnyGracz = SerwerRozgrywki.PobierzGracza(token, id);


            PobierzUzytkownikow();

            PobierzObecnyStol();
            LStol.Content = ObecnyStol.nazwaPokoju;

            ZaladujKarty();

            //UsunWszystkieKarty();
            for (int i = 0; i < Uzytkownicy.Length; i++)
            {
                UstawGracza(i + 1, Uzytkownicy[i].nazwaUzytkownika,0,0,true);
            }
            for (int i = Uzytkownicy.Length; i < 8; i++)
            {
                UstawGracza(i + 1);
            }
            //ZmienKarte(ref Stol, 0, ref kier[11]);
            //ZmienKarte(ref Stol, 1, ref trefl[11]);
            //ZmienKarte(ref Stol, 2, ref pik[11]);
            //UstawGracza(1, ObecnyUzytkownik.nazwaUzytkownika, (int)ObecnyUzytkownik.kasiora, (int)ObecnyGracz.stawia, true, true, false, true);
            //ZmienKarte(ref G1, 0, ref pik[12]);
            //ZmienKarte(ref G1, 1, ref kier[12]);
            //UstawGracza(6, "Paweł", 1500, 0, true, false, false, false, true);
            //ZmienKarte(ref G6, 0, ref pik[2]);
            //ZmienKarte(ref G6, 1, ref karo[3]);
            //UstawGracza(2, "Marcin", 0, 100, true);
            //ZmienKarte(ref G2, 0, ref kier[2]);
            //ZmienKarte(ref G2, 1, ref trefl[3]);
            //UstawGracza(8, "Komputer", 10, 199, true, true, true);
            //ZmienKarte(ref G8, 0, ref pik[4]);
            //ZmienKarte(ref G8, 1, ref trefl[4]);
            //UstawGracza(3);
            //UstawGracza(4);
            //UstawGracza(5);
            //UstawGracza(7);
            
            PobierzWiadomosci();
            Nakladka(true);
            WystartujZegar();
        }

        private void PobierzUzytkownikow()
        {
            Uzytkownicy = SerwerRozgrywki.ZwrocUserowStart(token); // Tu jest czasem coś nie tak ;p
            foreach (Rozgrywki.Uzytkownik u in Uzytkownicy)
            {
                if (id == u.identyfikatorUzytkownika)
                    ObecnyUzytkownik = u;
            }
        }

        private void WystartujZegar()
        {
            chatTimer.Tick += new EventHandler(chatTimer_Tick);
            chatTimer.Interval = new TimeSpan(0, 0, 3);
            chatTimer.Start();
        }
        private void WystartujZegar2()
        {
            ogolnyTimer.Tick += new EventHandler(ogolnyTimer_Tick);
            ogolnyTimer.Interval = new TimeSpan(0, 0, 1);
            ogolnyTimer.Start();
        }

        private void chatTimer_Tick(object sender, EventArgs e)
        {
            if (ObecnyStol.graRozpoczeta==false)
            {
                PobierzUzytkownikow();
                if (gra == null)
                    Nakladka(true);
            }
            else
            {
                Nakladka(false);
            }
            
            PobierzWiadomosci();
            PobierzObecnyStol();
        }

        private void ogolnyTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                gra = SerwerRozgrywki.ZwrocGre(token);               
                if (gra != null)
                {
                    Gracze = SerwerRozgrywki.ZwrocGraczy(token);
                    if (!s) // Wykonuje się raz, jedynie przy starcie gry
                    {
                        s = true;
                        PobierzObecnyStol();
                        UstawMiejscePrzyStole();
                        
                        Nakladka(false);
                    }

                    
                    Gracze=SerwerRozgrywki.ZwrocGraczy(token);
                    for(int i=0;i<Gracze.Length;i++)     
                    {
                        if (Gracze[i].identyfikatorUzytkownika == id)
                        {
                            ja = Gracze[i];
                            SIleStawia.Maximum = ja.kasa;
                        }
                        else
                        {
                            UaktualnijMiejscePrzystole();
                        }
                        bool tempBB=false;
                        bool tempSB=false;
                        bool ruch=false;
                        bool fold=false;
                        if(Gracze[i].stan==StanGracza.BigBlind)
                            tempBB=true;
                        else if (Gracze[i].stan == StanGracza.SmallBlind)
                        {
                            tempBB = true;
                            tempSB = true;
                        }
                        if(Gracze[i].identyfikatorUzytkownika==gra.czyjRuch)
                            ruch=true;
                        if (Gracze[i].stan == StanGracza.Fold)
                            fold = true;

                        UstawGracza(i + 1, Gracze[miejscePrzyStole[i, 0]].nazwaUzytkownika, (int)Gracze[miejscePrzyStole[i, 0]].kasa, (int)Gracze[miejscePrzyStole[i, 0]].stawia, true, tempBB, tempSB, ruch, fold, (int)Gracze[miejscePrzyStole[i, 0]].identyfikatorUzytkownika);
                    }
                    LKasaStol.Content = gra.pula;
                    //wszystko co związane z naszym graczem
                    if (SerwerRozgrywki.PobierzGracza(token, id) != null)
                    {
                        Rozgrywki.Gracz t = gra.aktywni.Single<Gracz>(delegate(Gracz c) { return c.identyfikatorUzytkownika == id; });
                        if (t != null)
                        {
                            //if (ja != t)
                            //{
                                //ja = t;
                                if (gra.stan == Stan.PREFLOP)
                                {//pobranie kart i wyświetlenie ich
                                    List<Karta> k = new List<Karta>(SerwerRozgrywki.PobierzKarty(token));
                                    Image x=PowiazanieKart(k.ElementAt(0));
                                    Image y=PowiazanieKart(k.ElementAt(1));
                                    ZmienKarte(ref G1, 0, ref x);
                                    ZmienKarte(ref G1, 1, ref y);
                                  
                                }
                            //}
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nieoczekiwany wyjątek! " + ex.Message +" "+ex.StackTrace, "Fatal Error");
            }

        }

        private void UstawMiejscePrzyStole()
        {
            if (gra != null) 
            {
                miejscePrzyStole = new int[Gracze.Length, 2];
                int temp = -1;
                for (int j = 0; j < Gracze.Length; j++)
                {
                    if (Gracze[j].identyfikatorUzytkownika == ObecnyUzytkownik.identyfikatorUzytkownika)
                    {
                        miejscePrzyStole[0, 0] = j;
                        miejscePrzyStole[0, 1] = (int)ObecnyUzytkownik.identyfikatorUzytkownika;
                        temp = j + 1;
                    }
                }
                int k = 1;
                for (int i = 0; i < Gracze.Length; i++)
                {
                    if (Gracze[i].identyfikatorUzytkownika != ObecnyUzytkownik.identyfikatorUzytkownika)
                    {
                        if (temp >= Gracze.Length)
                            temp = 0;
                        miejscePrzyStole[k, 0] = temp;
                        miejscePrzyStole[k, 1] = (int)Gracze[i].identyfikatorUzytkownika;
                        temp++;
                        k++;
                    }
                }
            }
            //else
            //{
            //}
        }
        private void UaktualnijMiejscePrzystole()
        {
            for (int i = 0; i < Gracze.Length; i++)
            {
                for (int j = 0; j < miejscePrzyStole.GetLength(0); j++)
                {
                    if (miejscePrzyStole[j, 1] == Gracze[i].identyfikatorUzytkownika)
                    {
                        miejscePrzyStole[j, 0] = i;
                    }
                }
            }
        }

        private void displayCards()
        {

        }

        private Image PowiazanieKart(Karta card)
        {
            if (card.kolor == kolorKarty.karo)
            {
                for (int i = 0; i < 13; i++)
                {
                    if ((int)card.figura == (int)figuraKarty.K2 + i)
                    {
                        return karo[(int)card.figura];
                    }
                }
            }
            else if (card.kolor == kolorKarty.kier)
            {
                for (int i = 0; i < 13; i++)
                {
                    if ((int)card.figura == (int)figuraKarty.K2 + i)
                    {
                        return kier[(int)card.figura];
                    }
                }
            }
            else if (card.kolor == kolorKarty.pik)
            {
                for (int i = 0; i < 13; i++)
                {
                    if ((int)card.figura == (int)figuraKarty.K2 + i)
                    {
                        return pik[(int)card.figura];
                    }
                }
            }
            else if (card.kolor == kolorKarty.trefl)
            {
                for (int i = 0; i < 13; i++)
                {
                    if ((int)card.figura == (int)figuraKarty.K2 + i)
                    {
                        return trefl[(int)card.figura];
                    }
                }
            }
            return null;
        }

        private void PobierzWiadomosci()
        {
            Wiadomosci = SerwerGlowny.PobierzWiadomosci(token, OstatnieOdswiezenie, ObecnyStol.numerPokoju);
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

        private void ZmienKarte(ref Grid g, int ktoraKarta, ref Image nowaKarta)
        {
            //if (ktoraKarta >= g.Children.Count && g.Children.Count > 0)
            //{
            //    ktoraKarta = g.Children.Count - 1;
            //}
            if(g.Children.Count > 0 && ktoraKarta < g.Children.Count)
                g.Children.RemoveAt(ktoraKarta);
            //g.Children.Add(nowaKarta);
            g.Children.Insert(ktoraKarta, nowaKarta);
            Grid.SetColumn(nowaKarta, ktoraKarta);
        }

        private void PrzejdzDoPokojuGlownego()
        {
            Black blackwindow = new Black();
            blackwindow.Show();
            PokojGlowny main = new PokojGlowny(token,id);

            //main.token = token;
            //main.id = id;

            App.Current.MainWindow = main;
            main.Show();
            blackwindow.Close();
            this.Close();
        }
        private void UstawBlind(ref Ellipse kolko, bool maly = false)
        {
            RadialGradientBrush myRadialGradientBrush = new RadialGradientBrush();
            if (maly)
            {
                myRadialGradientBrush.GradientStops.Add(new GradientStop(Colors.DodgerBlue, 0.0));
                myRadialGradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 1));
                kolko.Stroke = Brushes.MidnightBlue;
            }
            else
            {
                myRadialGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FFE6960B"), 0.0));
                myRadialGradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#FF954F0A"), 1));
                kolko.Stroke = Brushes.SaddleBrown;
            }

            //stroke Stroke="#FF572801" grubosc 3
            //<GradientStop Color="#FFE6960B" Offset="0"/>
            //<GradientStop Color="#FF954F0A" Offset="1"/>

            //DirectCast(ColorConverter.ConvertFromString("#D8E0A627"), Color)

            kolko.StrokeThickness = 3;
            
            kolko.Fill = myRadialGradientBrush;

            kolko.Visibility = Visibility.Visible;
        }
        private void IleStawia(int pozycja, int kasa = 0)
        {
            switch (pozycja)
            {
                case 1:
                    if (kasa > 0)
                    {
                        LWkladG1.Content = kasa;
                        LWkladG1.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG1.Visibility = Visibility.Hidden;
                    break;
                case 2:
                    if (kasa > 0)
                    {
                        LWkladG2.Content = kasa;
                        LWkladG2.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG2.Visibility = Visibility.Hidden;
                    break;
                case 3:
                    if (kasa > 0)
                    {
                        LWkladG3.Content = kasa;
                        LWkladG3.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG3.Visibility = Visibility.Hidden;
                    break;
                case 4:
                    if (kasa > 0)
                    {
                        LWkladG4.Content = kasa;
                        LWkladG4.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG4.Visibility = Visibility.Hidden;
                    break;
                case 5:
                    if (kasa > 0)
                    {
                        LWkladG5.Content = kasa;
                        LWkladG5.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG5.Visibility = Visibility.Hidden;
                    break;
                case 6:
                    if (kasa > 0)
                    {
                        LWkladG6.Content = kasa;
                        LWkladG6.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG6.Visibility = Visibility.Hidden;
                    break;
                case 7:
                    if (kasa > 0)
                    {
                        LWkladG7.Content = kasa;
                        LWkladG7.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG7.Visibility = Visibility.Hidden;
                    break;
                case 8:
                    if (kasa > 0)
                    {
                        LWkladG8.Content = kasa;
                        LWkladG8.Visibility = Visibility.Visible;
                    }
                    else
                        LWkladG8.Visibility = Visibility.Hidden;
                    break;
                default:
                    break;
            }
        }
        private void UstawGracza(int pozycja, string nazwa = "", int kasa = 0, int ileStawia = 0, bool widoczny = false, bool blind = false, bool maly = false, bool ruch = false, bool fold = false,int identyfikator=-1)
        {
            //Foreground="#FFFF9700" - brak ruchu
            IleStawia(pozycja, ileStawia);
            switch (pozycja)
            {
                case 1:
                    if (widoczny)
                    {
                        //if (identyfikator == id && x!=null && y!=null)
                        //{
                        //    ZmienKarte(ref G1, 0, ref x);
                        //    ZmienKarte(ref G1, 1, ref y);
                        //}
                        LG1.Visibility = Visibility.Visible;
                        LG1.Content = nazwa;                      
                        LKasaG1.Content = kasa;
                        if (ruch)
                            LG1.Foreground = Brushes.Red;
                        else
                            LG1.Foreground = Brushes.Orange;

                        if (fold)
                            LG1.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG1, maly);
                        else
                            EG1.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG1.Visibility = Visibility.Hidden;
                        LKasaG1.Visibility = Visibility.Hidden;
                        EG1.Visibility = Visibility.Hidden;
                    }
                    break;
                case 2:
                    if (widoczny)
                    {
                        //if (identyfikator == id)
                        //{
                        //    ZmienKarte(ref G1, 0, ref x);
                        //    ZmienKarte(ref G1, 1, ref y);
                        //}
                        LG2.Visibility = Visibility.Visible;
                        LG2.Content = nazwa;
                        LKasaG2.Content = kasa;
                        if (ruch)
                            LG2.Foreground = Brushes.Red;
                        else
                            LG2.Foreground = Brushes.Orange;
                        if (fold)
                            LG2.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG2, maly);
                        else
                            EG2.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG2.Visibility = Visibility.Hidden;
                        LKasaG2.Visibility = Visibility.Hidden;
                        EG2.Visibility = Visibility.Hidden;
                    }
                    break;
                case 3:
                    if (widoczny)
                    {
                        LG3.Visibility = Visibility.Visible;
                        LG3.Content = nazwa;
                        LKasaG3.Content = kasa;
                        if (ruch)
                            LG3.Foreground = Brushes.Red;
                        else
                            LG3.Foreground = Brushes.Orange;
                        if (fold)
                            LG3.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG3, maly);
                        else
                            EG3.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG3.Visibility = Visibility.Hidden;
                        LKasaG3.Visibility = Visibility.Hidden;
                        EG3.Visibility = Visibility.Hidden;
                    }
                    break;
                case 4:
                    if (widoczny)
                    {
                        LG4.Visibility = Visibility.Visible;
                        LG4.Content = nazwa;
                        LKasaG4.Content = kasa;
                        if (ruch)
                            LG4.Foreground = Brushes.Red;
                        else
                            LG4.Foreground = Brushes.Orange;
                        if (fold)
                            LG4.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG4, maly);
                        else
                            EG4.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG4.Visibility = Visibility.Hidden;
                        LKasaG4.Visibility = Visibility.Hidden;
                        EG4.Visibility = Visibility.Hidden;
                    }
                    break;
                case 5:
                    if (widoczny)
                    {
                        LG5.Visibility = Visibility.Visible;
                        LG5.Content = nazwa;
                        LKasaG5.Content = kasa;
                        if (ruch)
                            LG5.Foreground = Brushes.Red;
                        else
                            LG5.Foreground = Brushes.Orange;
                        if (fold)
                            LG5.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG5, maly);
                        else
                            EG5.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG5.Visibility = Visibility.Hidden;
                        LKasaG5.Visibility = Visibility.Hidden;
                        EG5.Visibility = Visibility.Hidden;
                    }
                    break;
                case 6:
                    if (widoczny)
                    {
                        LG6.Visibility = Visibility.Visible;
                        LG6.Content = nazwa;
                        LKasaG6.Content = kasa;
                        if (ruch)
                            LG6.Foreground = Brushes.Red;
                        else
                            LG6.Foreground = Brushes.Orange;
                        if (fold)
                            LG6.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG6, maly);
                        else
                            EG6.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG6.Visibility = Visibility.Hidden;
                        LKasaG6.Visibility = Visibility.Hidden;
                        EG6.Visibility = Visibility.Hidden;
                    }
                    break;
                case 7:
                    if (widoczny)
                    {
                        LG7.Visibility = Visibility.Visible;
                        LG7.Content = nazwa;
                        LKasaG7.Content = kasa;
                        if (ruch)
                            LG7.Foreground = Brushes.Red;
                        else
                            LG7.Foreground = Brushes.Orange;
                        if (fold)
                            LG7.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG7, maly);
                        else
                            EG7.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG7.Visibility = Visibility.Hidden;
                        LKasaG7.Visibility = Visibility.Hidden;
                        EG7.Visibility = Visibility.Hidden;
                    }
                    break;
                case 8:
                    if (widoczny)
                    {
                        LG8.Visibility = Visibility.Visible;
                        LG8.Content = nazwa;
                        LKasaG8.Content = kasa;
                        if (ruch)
                            LG8.Foreground = Brushes.Red;
                        else
                            LG8.Foreground = Brushes.Orange;
                        if (fold)
                            LG8.Foreground = Brushes.Gray;

                        if (blind)
                            UstawBlind(ref EG8, maly);
                        else
                            EG8.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        LG8.Visibility = Visibility.Hidden;
                        LKasaG8.Visibility = Visibility.Hidden;
                        EG8.Visibility = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }

        private void ZaladujTla()
        {
            for (int i = 0; i < 4; i++)
            {
                BitmapImage temp = new BitmapImage();
                temp.BeginInit();
                temp.UriSource = new Uri("pack://application:,,,/resources/tlo/" + (i + 1).ToString() + ".png");
                temp.EndInit();
                tlo[i] = new Image();
                tlo[i].Source = temp;
            }
        }

        private void UsunWszystkieKarty()
        {
            try
            {
                G1.Children.RemoveRange(0, G1.Children.Count);
                G2.Children.RemoveRange(0, G2.Children.Count);
                G3.Children.RemoveRange(0, G3.Children.Count);
                G4.Children.RemoveRange(0, G4.Children.Count);
                G5.Children.RemoveRange(0, G5.Children.Count);
                G6.Children.RemoveRange(0, G6.Children.Count);
                G7.Children.RemoveRange(0, G7.Children.Count);
                G8.Children.RemoveRange(0, G8.Children.Count);
                Stol.Children.RemoveRange(0, Stol.Children.Count);
            }
            catch (Exception ex)
            {
            }
        }

        private void ZaladujKarty()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (i == 0)
                    {
                        BitmapImage temp = new BitmapImage();
                        temp.BeginInit();
                        temp.UriSource = new Uri("pack://application:,,,/resources/karo/" + (j + 2).ToString() + ".png");
                        temp.EndInit();
                        karo[j] = new Image();
                        karo[j].Source = temp;
                    }
                    if (i == 1)
                    {
                        BitmapImage temp = new BitmapImage();
                        temp.BeginInit();
                        temp.UriSource = new Uri("pack://application:,,,/resources/kier/" + (j + 2).ToString() + ".png");
                        temp.EndInit();
                        kier[j] = new Image();
                        kier[j].Source = temp;
                    }
                    if (i == 2)
                    {
                        BitmapImage temp = new BitmapImage();
                        temp.BeginInit();
                        temp.UriSource = new Uri("pack://application:,,,/resources/pik/" + (j + 2).ToString() + ".png");
                        temp.EndInit();
                        pik[j] = new Image();
                        pik[j].Source = temp;
                    }
                    if (i == 3)
                    {
                        BitmapImage temp = new BitmapImage();
                        temp.BeginInit();
                        temp.UriSource = new Uri("pack://application:,,,/resources/trefl/" + (j + 2).ToString() + ".png");
                        temp.EndInit();
                        trefl[j] = new Image();
                        trefl[j].Source = temp;
                    }
                }
            }

            ZaladujTla();
        }

        private void SIleStawia_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LIleStawia.Content = SIleStawia.Value;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                komunikatR = SerwerRozgrywki.OpuscStol(token);
            }
            catch (Exception ee)
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            chatTimer.Stop();
            ogolnyTimer.Stop();
            if (MessageBox.Show("Czy na pewno chcesz opuścić pokój?", "Opuść pokój", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                komunikatR = SerwerRozgrywki.OpuscStol(token);
                PrzejdzDoPokojuGlownego();
                //do no stuff
            }
            else
            {
                chatTimer.Start();
                ogolnyTimer.Start();
            }
        }

        private void TBPoleCzatu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (TBPoleCzatu.Text != "")
                {
                    try
                    {
                        komunikat = SerwerGlowny.WyslijWiadomosc(token, new Glowny.Wiadomosc { nazwaUzytkownika = ObecnyUzytkownik.nazwaUzytkownika, stempelCzasowy = 0, numerPokoju = ObecnyStol.numerPokoju, trescWiadomosci = TBPoleCzatu.Text });

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

        private void TBLStart_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            komunikatR = SerwerRozgrywki.Start(token);
            TBLPoStart.Visibility = Visibility.Visible;
            TBLStart.Visibility = Visibility.Hidden;
            PobierzUzytkownikow();
            UzytkownicyNaStart();
            WystartujZegar2();
        }

    }
}
