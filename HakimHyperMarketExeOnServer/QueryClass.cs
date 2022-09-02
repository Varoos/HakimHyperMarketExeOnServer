using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HakimHyperMarketExeOnServer
{
   public class QueryClass
    {
        public class Table
        {
            public int iHeaderId { get; set; }
        }

        public class Datum
        {
            public List<Table> Table { get; set; }
        }

        public class Root
        {
            public List<Datum> data { get; set; }
            public string url { get; set; }
            public int result { get; set; }
            public object message { get; set; }
        }

        public class UpdateDatum
        {
            public int Result { get; set; }
        }

        public class UpdateRoot
        {
            public List<UpdateDatum> data { get; set; }
            public string url { get; set; }
            public int result { get; set; }
            public object message { get; set; }
        }
    }
}
