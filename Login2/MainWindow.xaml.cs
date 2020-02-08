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

namespace Login2
{
    public class Tools
    {
        // Mencegah aplikasi bisa di kill
        // WARNING : KILL PROCESS -> BSOD !!!
        // ** deprecated karena membutuhkan UAC,
        // sehingga aplikasi tidak dapat digunakan
        // saat windows startup

        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

        public void preventKill(int status)
        {
            int BreakOnTermi = 0x1D;  //breakoftermination value
                                      //https://undocumented.ntinternals.net/index.html?page=UserMode%2FUndocumented%20Functions%2FNT%20Objects%2FProcess%2FPROCESS_INFORMATION_CLASS.html
            Process.EnterDebugMode();
            NtSetInformationProcess(Process.GetCurrentProcess().Handle, BreakOnTermi, ref status, sizeof(int));
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
            }        }
    }

    public partial class MainWindow : Window
    {
        Tools tools = new Tools();
        public MainWindow()
        {
            InitializeComponent();
            //tools.preventKill(1);
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
            string NIM = NIMText.Text;
            string TglLahir = TglLahirText.Text;

            // Validasi Input
            if (NIM.Equals(""))
            {
                MessageBox.Show("NIM tidak boleh kosong!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.NIMText.Focus();
                return;
            }
            if (TglLahir.Equals(""))
            {
                MessageBox.Show("Tanggal lahir tidak boleh kosong!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.TglLahirText.Focus();
                return;
            }

            // Force login untuk keluar aplikasi
            if (NIM.ToLower().Equals("teknisi"))
            {
                if (TglLahir.ToLower().Equals("jtkpolban"))
                {
                    MessageBox.Show("FORCE LOGIN DETECTED, EXITING PROGRAM...", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    //tools.preventKill(0);
                    System.Environment.Exit(0);
                }
            }

            // Cek user data
            WebRequest req = WebRequest.Create("http://127.0.0.1:8000/api/mahasiswa/" + NIM);
            req.ContentType = "application/json; charset=utf-8";
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
                MessageBox.Show("Terjadi kesalahan pada jaringan, silakan coba lagi!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                this.NIMText.Focus();
                return;
            }

            // Retrieve data mahasiswa
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

                // Check user login validity
                if (mhs.nim != null && mhs.tanggal_lahir == TglLahir)
                {
                    tools.readxml(mhs);
                    Console.WriteLine("Running on PC : " + mhs.no_pc);

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
                    //tools.preventKill(0);
                    
                    Close();
                }
                else
                {
                    MessageBox.Show("NIM atau tanggal lahir salah !", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.NIMText.Focus();
                }
            }
            else
            {
                MessageBox.Show("NIM atau tanggal lahir salah !", "ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
                this.NIMText.Focus();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //tools.preventKill(0);
            System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
            //Close();
            System.Environment.Exit(0);
        }
    }
}

