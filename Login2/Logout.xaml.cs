﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Login2
{
    /// <summary>
    /// Interaction logic for logout.xaml
    /// </summary>
    public partial class Logout : Window
    {
        Tools tools = new Tools();
        private static readonly HttpClient client = new HttpClient();
        Mahasiswa mhs;
        public Logout(Mahasiswa mhsw)
        {
            InitializeComponent();
            mhs = mhsw;
            Console.WriteLine("Jam Masuk nyaa-> " + mhs.waktu_masuk);
        }

        public void DeActive(Mahasiswa mhsw)
        {
            //--------------------------- Menonaktifkan Status User --------------------------------------
            string URI = "http://127.0.0.1:8000/api/status/deactivate/" + mhsw.nim;
            string myParameters = "";

            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                string HtmlResult = wc.UploadString(URI, "PUT", myParameters);
            }
            //------------------------- End Menonaktifkan Status User ------------------------------------
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(this.kegiatan.Text != "")
            {
                mhs.kegiatan = kegiatan.Text;
                mhs.waktu_keluar = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                string js = JsonConvert.SerializeObject(mhs);
                Console.WriteLine(js);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:8000/api/log/savedata");

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
                Console.WriteLine("Status : " + status);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.WriteLine(result);
                }
                if (status == 200)
                {
                    DeActive(mhs);
                    tools.preventKill(0);
                    Close();
                    //System.Diagnostics.Process.Start("shutdown", "/s /f /t 0");
                    Console.WriteLine("matiiii");
                } else if (status == 400)
                {
                    Console.WriteLine("Error jaringan");
                    MessageBox.Show("Terjadi kesalahan terhadap jaringan, silahkan coba lagi !!", "ERROR");
                    this.kegiatan.Focus();
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
