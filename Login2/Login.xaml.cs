using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using Newtonsoft.Json;

namespace Login2
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 

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
