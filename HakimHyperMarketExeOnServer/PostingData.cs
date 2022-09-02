using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakimHyperMarketExeOnServer
{
    public class PostingData
    {
        public PostingData()
        {
            data = new List<Hashtable>();
        }
        public List<Hashtable> data { get; set; }
    }
}
