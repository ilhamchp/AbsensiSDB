using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;                   //  USE FOR ANTI KILLS
using System.Xml;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;

namespace Login2
{
    public class Tools
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        // Mencegah aplikasi bisa di kill
        // WARNING : KILL PROCESS -> BSOD !!!

        public void preventKill(int status)
        {
            /*
            int BreakOnTermi = 0x1D;  //breakoftermination value
                                      //https://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FProcess%2FPROCESS_INFORMATION_CLASS.html
            Process.EnterDebugMode();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermi, ref status, sizeof(int));
            */
        }

        // Mencatat lokasi PC yang digunakan oleh User
        public void readxml(Mahasiswa mhs)
        {
            using (XmlReader reader = XmlReader.Create(@"C:\Login\config.xml"))
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
                                mhs.no_pc = reader.ReadString();
                                Console.WriteLine("Nomer Pc : " + mhs.no_pc);
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
    }

    public partial class MainWindow : Window
    {
        Tools tools = new Tools();
        public MainWindow()
        {
            InitializeComponent();
            tools.preventKill(1);
        }
        // mematikan fungsi dari alt + f4

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            /*
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
            */
        }

        // Proses login, ketika tombol Login di click
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow home = new MainWindow();

            //string npa = NPAText.Text;
            string TglLahir = TglLahirText.Text;
            string NIM = NIMText.Text;
            if (TglLahir.Equals(""))
            {
                MessageBox.Show("Tanggal lahir tidak boleh kosong!", "ERROR");
                this.TglLahirText.Focus();
                return;
            }
            if (NIM.Equals(""))
            {
                MessageBox.Show("NIM tidak boleh kosong!", "ERROR");
                this.NIMText.Focus();
                return;
            }
            // WebRequest req = WebRequest.Create("http://10.10.0.9/elib/api/user/read_one.php?npa=" + npa);
            //WebRequest req = WebRequest.Create("http://encode.jtk.polban.ac.id/elib/api/user/read_one.php?npa=" + npa);
            WebRequest req = WebRequest.Create("http://127.0.0.1:8000/api/mahasiswa/" + NIM);
            //req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";

            //string postData = "npa=" + npa;
            //byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            //req.ContentLength = byteArray.Length;


            //Stream ds = req.GetRequestStream();
            //ds.Write(byteArray, 0, byteArray.Length);
            //ds.Close();
            string jsonText = null;
            try
            {
                var response = (HttpWebResponse)req.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    jsonText = sr.ReadToEnd();
                }
            }catch(WebException w)
            {
                Console.WriteLine(w.Message);
                MessageBox.Show("Terjadi kesalahan pada jaringan, silakan coba lagi!", "ERROR");
                this.NIMText.Focus();
                return;
            }
            //var nama = js["nama"];

            //get data from json to model
            //User user = JsonConvert.DeserializeObject<User>(jsonText);
            Mahasiswa mhs = JsonConvert.DeserializeObject<Mahasiswa>(jsonText);
            if (mhs.code.Equals(200))
            {
                try
                {
                    DateTime dt = DateTime.ParseExact(mhs.tanggal_lahir, "yyyy-MM-dd",
                                      CultureInfo.InvariantCulture);
                    mhs.tanggal_lahir = dt.ToString("ddMMyyyy");
                }
                catch (FormatException fe)
                {
                    Console.WriteLine(fe);
                }
                if (mhs.nim != null && mhs.tanggal_lahir == TglLahir)
                {
                    tools.readxml(mhs);
                    Console.WriteLine("Running on PC : " + mhs.no_pc);
                    
                    //Console.WriteLine("fakyu");
                    //this.Close();
                    //MainWindow signIn = new MainWindow();
                    //signIn.ShowDialog();
                    //login.TextBlockName.Text = nama;

                    //---------------------------Penambahan User Active 1--------------------------------------
                    //string URI = "http://encode.jtk.polban.ac.id/elib/api/login/aktif.php";
                    //string myParameters = "npa="+ user.User_npa + "&active=1";
                    string URI = "http://127.0.0.1:8000/api/status/activate/" + mhs.nim;
                    string myParameters = "nomor_pc=" + mhs.no_pc;

                    using (WebClient wc = new WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string HtmlResult = wc.UploadString(URI,"PUT", myParameters);
                    }
                    //-------------------------End Penambahan User Active 1--------------------------------------
                    
                    Login login = new Login(mhs);
                    login.Show();
                    tools.preventKill(0);
                    
                    Close();
                }
                else
                {
                    MessageBox.Show("NIM atau tanggal lahir salah !", "ERROR");
                    this.NIMText.Focus();
                }
            }
            else
            {

                MessageBox.Show("NIM atau tanggal lahir salah !", "ERROR");
                this.NIMText.Focus();
            }
            //fail.IsOpen = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tools.preventKill(0);
            //System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
            Close();
            System.Environment.Exit(0);
        }
    }
}

