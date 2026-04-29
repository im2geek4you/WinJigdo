using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Jigdo.Gui
{
    public class DownloadMirror
    {
        public string? alias { get; set; }
        public string? url { get; set; }
        public bool tryLast { get; set; }

    }
}
