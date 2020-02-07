using System;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Login2
{

    public partial class Login : Window
    {
        Mahasiswa mhs;
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        public Login(Mahasiswa mhsw)
        {
            mhs = mhsw;
            InitializeComponent();

            Timer.Tick += new EventHandler(Timer_Click);

            Timer.Interval = new TimeSpan(0, 0, 1);

            Timer.Start();

            Console.WriteLine("LOGIN " + mhs.nim);
            string js = JsonConvert.SerializeObject(mhs);
            Console.WriteLine(js);

            //get data from model
            nama.Content = mhs.nama;
            nim.Content = mhs.nim;
            Console.WriteLine("LOGIN " + mhs.nim);
            mhs.waktu_masuk = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            Console.WriteLine("Jam Masuk nyaa-> " + mhs.waktu_masuk);
            Console.WriteLine("Mhs nya ada di pc -> " + mhs.no_pc);
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
            
            Logout logout = new Logout(mhs);
            logout.Show();
            Close();
        }
    }
}
