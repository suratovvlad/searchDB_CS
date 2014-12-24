using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace searchDB_CS_GUI
{
    class Document
    {
        public Document(String _id)
        {
            id = _id;
        }

        public String id { get; set; }
        public String author { get; set; }
        public String date { get; set; }
        public String body { get; set; }
        public String title { get; set; }
        public String shortTitle { get; set; }
        public String topics { get; set; }
        public String place { get; set; }
    }
}
