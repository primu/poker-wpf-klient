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
    /// Interaction logic for PokojGry.xaml
    /// </summary>
    public partial class PokojGry : Window
    {
        Image[] karo = new Image[13];
        Image[] kier = new Image[13];
        Image[] pik = new Image[13];
        Image[] trefl = new Image[13];

        Image[] tlo = new Image[4];

        public PokojGry()
        {
            InitializeComponent();
            //var karta = (Image)G1.Children[0];
            //karta = karo[0];
            
            ZaladujKarty();
            ZmienKarte(ref G1,1, ref pik[3]);
        }
        private void ZmienKarte(ref Grid g, int ktoraKarta, ref Image nowaKarta)
        {
            if (ktoraKarta >= g.Children.Count)
            {
                ktoraKarta = g.Children.Count - 1;
            }
            G1.Children.RemoveAt(ktoraKarta);
            G1.Children.Add(nowaKarta);
            Grid.SetColumn(nowaKarta, ktoraKarta);
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
        }

    }
}
