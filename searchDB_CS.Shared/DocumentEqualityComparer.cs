using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class DocumentEqualityComparer : IEqualityComparer<Document>
    {
        public bool Equals(Document d1, Document d2)
        {
            if (d1.id == d2.id && d1.myClass == d2.myClass)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(Document doc)
        {
            int hCode = doc.id.GetHashCode() ^ doc.myClass.GetHashCode();
            return hCode.GetHashCode();
        }
    }
}
