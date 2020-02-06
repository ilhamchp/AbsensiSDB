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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using Flurl.Http;

namespace Login2
{
    /// <summary>
    /// Interaction logic for logout.xaml
    /// </summary>
    public partial class Logout : Window
    {
        preventKill disablekill = new preventKill();
        private static readonly HttpClient client = new HttpClient();
        User usr;
        public Logout(User user)
        {
            InitializeComponent();
            usr = user;
            Console.WriteLine("Jam Masuk nyaa-> " + user.jam_masuk);
        }

        public void DeActive(User user)
        {
            string URI = "http://encode.jtk.polban.ac.id/elib/api/login/aktif.php";
            string myParameters = "npa=" + user.User_npa + "&active=0";

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, myParameters);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.kegiatan.Text != "")
            {
                usr.kegiatan = kegiatan.Text;
                usr.jam_keluar = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                string js = JsonConvert.SerializeObject(usr);
                Console.WriteLine(js);


                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://encode.jtk.polban.ac.id/elib/api/login/create.php");

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";



                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    streamWriter.Write(js);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                Console.WriteLine((int)httpResponse.StatusCode);
                int status = (int)httpResponse.StatusCode;

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }

                if (status == 201)
                {
                    DeActive(usr);
                    disablekill.activate(0);
                    //System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
                    Console.WriteLine("matiiii");
                } else if (status == 400)
                {
                    Console.WriteLine("Error jaringan");
                }
            }
            else
            {
                MessageBox.Show("Harap masukkan kegiatan !!", "ERROR");
                this.kegiatan.Focus();
            }

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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
