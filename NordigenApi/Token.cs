using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NordigenApi
{
    public class Token
    {
        public string access { get; set; }
        public int access_expires { get; set; }
        public string refresh { get; set; }
        public int refresh_expires { get; set; }

    }
}
