using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Shared;
using System.IO;
using System.Xml;

namespace searchDB_CS
{ 
    class Program
    {
        static List<Document> GetTopics()
        {
            List<Document> docs = new List<Document>();

            string connetionString = null;
            SqlConnection connection;
            connetionString = "Data Source=DEGREE-PC;Initial Catalog=sophia_index_patents_g06;Integrated Security=True";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connection Opened!\n\n");

                

                SqlCommand command;
                SqlDataReader reader;

                command = new SqlCommand(
                    @"SELECT Doc_Topics, Doc_ID
                        FROM [dbo].[search_Documents]
					    ;"
                    , connection);

                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        /*
                        Console.WriteLine(String.Format("TOPICS:\t{0}",
                            reader[0]));
                        */
                        Document doc = new Document(reader[1].ToString());
                        doc.topics = reader[0].ToString();
                        docs.Add(doc);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error: {0}", ex.ToString()));
            }

            return docs;
        }

        static void GetSome()
        {
            string connetionString = null;
            SqlConnection connection;
            connetionString = "Data Source=DEGREE-PC;Initial Catalog=sophia_index_patents_g06;Integrated Security=True";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connection Opened!\n\n");

                String input = "network";
                SqlCommand command;
                SqlDataReader reader;

                command = new SqlCommand(
                    @"SELECT COUNT(indx.probability)
                        FROM [dbo].[search_words] wrds
                        LEFT JOIN [dbo].[search_terms] trms ON trms.term_id = wrds.term_id
					    LEFT JOIN [dbo].[search_Inverted_index] indx ON (indx.term_ID = trms.term_id)
					    LEFT JOIN [dbo].[search_Documents] docs ON	(indx.Doc_ID = docs.Doc_ID)
                        WHERE (wrds.word_word = @WORD) OR (wrds.word_stem = @WORD)
					    ;"
                    , connection);
                command.Parameters.Add(new SqlParameter("WORD", input));

                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("ALL DOCUMENTS FOUND: {0}\n\n",
                            reader[0]));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                command.Parameters.Clear();
                command = new SqlCommand(
                    @"SELECT TOP 10 indx.probability as probability, docs.Doc_ID as doc
                        FROM [dbo].[search_words] wrds
                        LEFT JOIN [dbo].[search_terms] trms ON trms.term_id = wrds.term_id
					    LEFT JOIN [dbo].[search_Inverted_index] indx ON (indx.term_ID = trms.term_id)
					    LEFT JOIN [dbo].[search_Documents] docs ON	(indx.Doc_ID = docs.Doc_ID)
                        WHERE (wrds.word_word = @WORD) OR (wrds.word_stem = @WORD)
					    ORDER BY indx.probability DESC;"
                    , connection);
                command.Parameters.Add(new SqlParameter("WORD", input));

                List<Document> docs = new List<Document>();
                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(String.Format("{0}, {1}",
                        //    reader[0], reader[1]));
                        docs.Add(new Document(reader[1].ToString()));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                Console.WriteLine(String.Format("TOP 10 RESULTS\n\n"));
                List<String> docs_bodyes = new List<String>();
                foreach (Document doc in docs)
                {
                    command.Parameters.Clear();
                    command = new SqlCommand(
                        @"SELECT docs.Doc_Title, docs.Doc_Date, docs.Doc_Author, docs.short_title, docs.Doc_Body
					        FROM [dbo].[search_Documents] docs
					        WHERE docs.Doc_ID = @DOCID;"
                        , connection);
                    command.Parameters.Add(new SqlParameter("DOCID", doc.id));

                    reader = command.ExecuteReader();
                    try
                    {
                        while (reader.Read())
                        {
                            //Console.WriteLine(String.Format("TITLE:\t\t{0}\nAUTHORS:\t\t{1}\nBODY:\t\t{2}\n\n",
                            //    reader[0], reader[1], reader[2]));
                            //Console.WriteLine(String.Format("TITLE:\t\t{0}\nDATE:\t\t{1}\nAUTHORS:\t\t{2}\n\n",
                            //    reader[0], reader[1], reader[2]));
                            doc.title = reader[0].ToString();
                            doc.date = reader[1].ToString();
                            doc.author = reader[2].ToString();
                            doc.shortTitle = reader[3].ToString();
                            doc.body = reader[4].ToString();

                        }
                    }
                    finally
                    {
                        // Always call Close when done reading.
                        reader.Close();
                    }
                }

                connection.Close();

                foreach (Document doc in docs)
                {
                    Console.WriteLine(String.Format("TITLE:\t\t{0}\nDATE:\t\t{1}\nAUTHORS:\t\t{2}\n\n",
                                doc.title, doc.date, doc.author));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error: {0}", ex.ToString()));
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");

            //GetSome();
            List<Document> docs = GetTopics();
            List<string> sections = new List<string>();

            //Dictionary<string, List<string> > sectionsToClassSymbols = new Dictionary<string, List<string> >();



            Tree<Tuple<string, int>> root = new Tree<Tuple<string, int>>(new Tuple<string, int>("TREE", 0));


            List<Document> sectionA = new List<Document>();
            List<Document> sectionB = new List<Document>();
            List<Document> sectionC = new List<Document>();
            List<Document> sectionD = new List<Document>();
            List<Document> sectionE = new List<Document>();
            List<Document> sectionF = new List<Document>();
            List<Document> sectionG = new List<Document>();
            List<Document> sectionH = new List<Document>();


            foreach (Document doc in docs)
            {
                string[] topics = doc.topics.Split(new Char[] { ';' });

                


                char[] topic = topics[0].ToUpper().ToCharArray();


                // Section
                string ch = topic[0].ToString();

                sections.Add(topics[0]);

                
                if (ch == "1" || ch == "3" || ch == "L" || ch == "S" || ch == "N")
                {
                    Console.WriteLine(topics[0]);
                    continue;
                }
                

                // Class
                string ch2 = topic[1].ToString() + topic[2].ToString();

                /*
                if (ch == "A" || ch == "B" || ch == "C" || ch == "D" || ch == "E"
                    || ch == "F" || ch == "G" || ch == "H")
                {
                */
                    /*
                     List<string> list = new List<string>();
                     list.Add("asfd");
                     list.Add("adsfa");
                     list.Find(x => x == "asdfa");
                    */                   

                var childSection = root.Children.Find(x => x.Value.Item1 == ch);
                if (childSection == null)
                {
                    childSection = root.Children.Add(new TreeNode<Tuple<string, int>>(new Tuple<string, int>(ch, 1)));
                }
                else
                {
                    childSection.Value = new Tuple<string, int>(childSection.Value.Item1, childSection.Value.Item2 + 1);
                }
                var childClass = childSection.Children.Find(x => x.Value.Item1 == ch2);
                if (childClass == null)
                {
                    childClass = childSection.Children.Add(new TreeNode<Tuple<string, int>>(new Tuple<string, int>(ch2, 1)));
                }
                else
                {
                    childClass.Value = new Tuple<string, int>(childClass.Value.Item1, childClass.Value.Item2 + 1);
                }

                    
                                        
                //if (!sectionsToClassSymbols.ContainsKey(ch))
                //{
                //    sectionsToClassSymbols.Add(ch, new List<string>());
                //}

                //sectionsToClassSymbols[ch].Add(ch2);
                    
                //}


                // Subclass
                string ch3 = topic[3].ToString();
                var childSubClass = childClass.Children.Find(x => x.Value.Item1 == ch3);
                if (childSubClass == null)
                {
                    childSubClass = childClass.Children.Add(new TreeNode<Tuple<string, int>>(new Tuple<string, int>(ch3, 1)));
                }
                else
                {
                    childSubClass.Value = new Tuple<string, int>(childSubClass.Value.Item1, childSubClass.Value.Item2 + 1);
                }



                switch (ch)
                {
                    case "A":
                        if (sectionA.Count < 60)
                        {
                            sectionA.Add(doc);
                        }
                        break;
                    case "B":
                        if (sectionB.Count < 60)
                        {
                            sectionB.Add(doc);
                        }
                        break;
                    case "C":
                        if (sectionC.Count < 60)
                        {
                            sectionC.Add(doc);
                        }
                        break;
                    case "D":
                        if (sectionD.Count < 60)
                        {
                            sectionD.Add(doc);
                        }
                        break;
                    case "E":
                        if (sectionE.Count < 60)
                        {
                            sectionE.Add(doc);
                        }                    
                        break;
                    case "F":
                        if (sectionF.Count < 60)
                        {
                            sectionF.Add(doc);
                        } 
                        break;
                    case "G":
                        if (sectionG.Count < 60)
                        {
                            sectionG.Add(doc);
                        } 
                        break;
                    case "H":
                        if (sectionH.Count < 60)
                        {
                            sectionH.Add(doc);
                        } 
                        break;
                }
            }          

            var classesForSections = sections.GroupBy(x => x).Distinct();

            Console.WriteLine(docs.Count());
            Console.WriteLine(classesForSections.Count());
            Console.WriteLine();

            /*
            StreamWriter swTraining = new StreamWriter(".\\classes1.txt");

            string temp = "";

            foreach(var class1 in classesForSections) 
            {
                char[] MyChar = { '/', ' ' };
                temp += class1.Key.ToString().Replace(" ", "").Replace("/", "") + "\n";
            }

            swTraining.WriteLine(temp);
            swTraining.Close();
            */

            /*
            foreach (var pair in sectionsToClassSymbols)
            {
                StreamWriter sw1 = new StreamWriter(String.Format(".\\classesSection_{0}.txt", pair.Key));

                string temp1 = "";

                var classesForClassSymbol = pair.Value.GroupBy(x => x).Distinct();
                Console.WriteLine(pair.Key + ":\t" + classesForClassSymbol.Count());

                foreach (var classSymbol in classesForClassSymbol)
                {
                    temp1 += classSymbol.Key.ToString() + "\n";
                }

                sw1.WriteLine(temp1);
                sw1.Close();
            }
            */

            int sumOfSections = 0;

            Console.WriteLine("SECTIONS COUNT:\t" + root.Children.Count());

            foreach(var section in root.Children)
            {
                Console.WriteLine("\t" + section.Value + " COUNT:\t" + section.Children.Count());

                int sumOfClasses = 0;

                foreach (var classS in section.Children)
                {
                    Console.WriteLine("\t\t" + classS.Value + " COUNT:\t" + classS.Children.Count());

                    sumOfClasses += classS.Value.Item2;
                }

                Console.WriteLine("\tIn this Section:\t" + sumOfClasses);

                sumOfSections += section.Value.Item2;
            }

            Console.WriteLine("ALL VIEWD:\t" + sumOfSections);

            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("docs");
            xmlDoc.AppendChild(rootNode);



            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            settings.NewLineOnAttributes = true;

            XmlWriter xmlWriter = XmlWriter.Create(".\\trainigIDs.xml", settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("docs");

            XmlNode sectionNode = xmlDoc.CreateElement("sectionA");
            xmlWriter.WriteStartElement("sectionA");
            foreach (var doc in sectionA)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionB");
            xmlWriter.WriteStartElement("sectionB");
            foreach (var doc in sectionB)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionC");
            xmlWriter.WriteStartElement("sectionC");
            foreach (var doc in sectionC)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionD");
            xmlWriter.WriteStartElement("sectionD");
            foreach (var doc in sectionD)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionE");
            xmlWriter.WriteStartElement("sectionE");
            foreach (var doc in sectionE)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionF");
            xmlWriter.WriteStartElement("sectionF");
            foreach (var doc in sectionF)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionG");
            xmlWriter.WriteStartElement("sectionG");
            foreach (var doc in sectionG)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionH");
            xmlWriter.WriteStartElement("sectionH");
            foreach (var doc in sectionH)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                xmlWriter.WriteStartElement("doc");
                xmlWriter.WriteString(doc.id.ToString());
                xmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            xmlWriter.WriteEndElement();

            xmlDoc.Save("test-doc.xml");
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            Console.ReadLine();
        }
    }
}
