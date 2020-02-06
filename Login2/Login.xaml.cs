using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Newtonsoft.Json;

namespace Login2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        User usr;

        


        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        public Login(User user)
        {

            

            usr = user;
            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Click);

            Timer.Interval = new TimeSpan(0, 0, 1);

            Timer.Start();

            Console.WriteLine("LOGIN " + user.User_npa);
            string js = JsonConvert.SerializeObject(user);
            Console.WriteLine(js);

            //get data from model
            nama.Content = user.nama;
            nim.Content = user.nim;
            Console.WriteLine("LOGIN " + user.User_npa);
            user.jam_masuk = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            Console.WriteLine("Jam Masuk nyaa-> " + user.jam_masuk);
            readxml(user);

        }

        // Mencatat lokasi PC yang digunakan oleh User ELIB
        public void readxml(User usr)
        {
            using (XmlReader reader = XmlReader.Create(@"C:\Login\ha.xml"))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        //return only when you have START tag  
                        switch (reader.Name.ToString())
                        {
                            case "No":
                                //Console.WriteLine("Name of the Element is : " + reader.ReadString());
                                usr.no_pc = reader.ReadString();
                                Console.WriteLine("Nomer Pc : " + usr.no_pc);
                                break;
                            case "Location":
                                Console.WriteLine("Your Location is : " + reader.ReadString());
                                break;
                        }
                    }
                    Console.WriteLine("");
                }
            }
            //Console.ReadKey();
        }


        // mematikan fungsi dari alt + f4
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4 ||
               Keyboard.Modifiers == ModifierKeys.Control && Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.Delete)
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }

        }

        private void Timer_Click(object sender, EventArgs e)

        {

            DateTime d;

            d = DateTime.Now;

            label1.Content = d.Hour + " : " + d.Minute + " : " + d.Second;
            tanggal.Content = d.DayOfWeek.ToString() + ", " + d.Day + "/" + d.Month + "/" + d.Year;


        }


  
        private void Button_Hiden(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Logout(object sender, RoutedEventArgs e)
        {
            
            Logout logout = new Logout(usr);
            logout.Show();
            Close();
        }
    }
}
