using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Diagnostics;                   //  USE FOR ANTI KILLS


namespace Login2
{
    // Mencegah aplikasi bisa di kill
    // WARNING : KILL PROCESS -> BSOD !!!
    public class preventKill
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        public void activate(int status)
        {
            int BreakOnTermi = 0x1D;  //breakoftermination value
                                      //https://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FProcess%2FPROCESS_INFORMATION_CLASS.html
            Process.EnterDebugMode();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermi, ref status, sizeof(int));
        }
    }
    public partial class MainWindow : Window
    {
        preventKill disablekill = new preventKill();

        public MainWindow()
        {
            InitializeComponent();
            disablekill.activate(1);
        }


        // mematikan fungsi dari alt + f4

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4 )||
               e.Key == Key.Delete && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control 
               )
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
            
        }

        // Proses login, ketika tombol Login di click
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MainWindow home = new MainWindow();

            string npa = NPAText.Text;
            string nim = NIMText.Text;
            // WebRequest req = WebRequest.Create("http://10.10.0.9/elib/api/user/read_one.php?npa=" + npa);
            WebRequest req = WebRequest.Create("http://encode.jtk.polban.ac.id/elib/api/user/read_one.php?npa=" + npa);

            //req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";

            //string postData = "npa=" + npa;
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //req.ContentLength = byteArray.Length;


            //Stream ds = req.GetRequestStream();
            //ds.Write(byteArray, 0, byteArray.Length);
            //ds.Close();


            var response = (HttpWebResponse)req.GetResponse();
            string jsonText;
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                jsonText = sr.ReadToEnd();
            }
            //var nama = js["nama"];

            //get data from json to model
            User user = JsonConvert.DeserializeObject<User>(jsonText);

            if (user.User_npa != null && user.nim == nim)
            {
                //Console.WriteLine("fakyu");
                //this.Close();
                //MainWindow signIn = new MainWindow();
                //signIn.ShowDialog();
                //login.TextBlockName.Text = nama;

                //---------------------------Penambahan User Active 1--------------------------------------
                string URI = "http://encode.jtk.polban.ac.id/elib/api/login/aktif.php";
                string myParameters = "npa="+ user.User_npa + "&active=1";

                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string HtmlResult = wc.UploadString(URI, myParameters);
                }
                //-------------------------End Penambahan User Active 1--------------------------------------

                Login login = new Login(user);
                login.Show();
                disablekill.activate(0);
                Close();
            }
            fail.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            disablekill.activate(0);
            //System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
            Close();
        }
    }
}

