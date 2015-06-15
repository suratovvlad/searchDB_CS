using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Shared
{
    [DataContract]
    public class Document
    {
        public Document(String _id)
        {
            id = _id;
        }

        public Document(Document doc)
        {
            id = doc.id;
            author = doc.author;
            date = doc.date;
            body = doc.body;
            title = doc.title;
            shortTitle = doc.shortTitle;
            topics = doc.topics;
            place = doc.place;
        }

        [DataMember]
        public String id { get; set; }
        [DataMember]
        public String author { get; set; }
        [DataMember]
        public String date { get; set; }
        [DataMember]
        public String body { get; set; }
        [DataMember]
        public String title { get; set; }
        [DataMember]
        public String shortTitle { get; set; }
        [DataMember]
        public String topics { get; set; }
        [DataMember]
        public String place { get; set; }


        public String myClass { get; set; }
    }
}
