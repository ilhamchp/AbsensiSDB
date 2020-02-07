using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login2
{
    public sealed class Mahasiswa
    {
        public string nim { get; set; }
        public string nama { get; set; }
        public string tanggal_lahir { get; set; }
        public string kegiatan { get; set; }
        public string no_pc { get; set; }
        public string waktu_masuk { get; set; }
        public string waktu_keluar { get; set; }
        public int code { get; set; }
    }
}
