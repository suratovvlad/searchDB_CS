using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using Shared;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Json;

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

        static void Count_G06()
        {
            List<Document> docs = GetTopics();

            string pattern = "G06[cdefgjkmnqtCDEFGJKMNQT]";

            Console.WriteLine(docs.Count());


            List<Document> cleanedDocs = new List<Document>();

            foreach (Document doc in docs)
            {

                string[] topics = doc.topics.Split(new Char[] { ';' });

                char[] topic = topics[0].ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();

                foreach (string topic1 in topics)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(topic1, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        topic = topic1.ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();
                        Document newdoc = new Document(doc);
                        newdoc.topics = new string(topic, 3, topic.Length - 3);

                        cleanedDocs.Add(newdoc);
                        break;
                    }
                }


                //if (System.Text.RegularExpressions.Regex.IsMatch(doc.topics, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                //{
                //    cleanedDocs.Add(doc);
                //}
            }

            

            SortedDictionary<char, int> themes = new SortedDictionary<char, int>();
            SortedDictionary<char, List<Document>> themedCollection = new SortedDictionary<char, List<Document>>();
            List<Document> cleaned2 = new List<Document>();
            foreach (Document doc in cleanedDocs)
            {
                if (themes.ContainsKey(doc.topics.ToUpper().FirstOrDefault()))
                {
                    themes[doc.topics.ToUpper().FirstOrDefault()]++;
                    themedCollection[doc.topics.ToUpper().FirstOrDefault()].Add(doc);
                }
                else
                {
                    themes.Add(doc.topics.ToUpper().FirstOrDefault(), 1);
                    List<Document> tempList = new List<Document>();
                    tempList.Add(doc);
                    themedCollection[doc.topics.ToUpper().FirstOrDefault()] = tempList;
                }
            }


            // take 30%

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            //settings.NewLineOnAttributes = true;

            XmlWriter xmlWriter = XmlWriter.Create(".\\themedCollectionTrain.xml", settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("docs");

            XmlWriter xmlWriter2 = XmlWriter.Create(".\\themedCollectionTest.xml", settings);
            xmlWriter2.WriteStartDocument();
            xmlWriter2.WriteStartElement("docs");

            foreach (char key in themedCollection.Keys)
            {
                int allcount = themes[key];
                int xcount = System.Convert.ToInt32(0.3*allcount);
                xmlWriter.WriteStartElement("section");
                xmlWriter.WriteAttributeString("class", System.Convert.ToString(key));

                xmlWriter2.WriteStartElement("section");
                xmlWriter2.WriteAttributeString("class", System.Convert.ToString(key));

                int i = 0;
                foreach (var doc in themedCollection[key])
                {
                    if (i > xcount)
                    {
                        xmlWriter2.WriteStartElement("doc");
                        xmlWriter2.WriteAttributeString("class", System.Convert.ToString(key));
                        xmlWriter2.WriteString(doc.id.ToString());
                        xmlWriter2.WriteEndElement();
                    }
                    else
                    {
                        xmlWriter.WriteStartElement("doc");
                        xmlWriter.WriteAttributeString("class", System.Convert.ToString(key));
                        xmlWriter.WriteString(doc.id.ToString());
                        xmlWriter.WriteEndElement();
                    }                    
                    ++i;
                }
                xmlWriter.WriteEndElement();
                xmlWriter2.WriteEndElement();

                
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            xmlWriter2.WriteEndElement();
            xmlWriter2.WriteEndDocument();
            xmlWriter2.Close();

            Console.WriteLine(cleanedDocs.Count());
            Console.ReadLine();
        }

        static List<Document> getDocsByInt(List<int> docIds)
        {
            List<Document> docs = new List<Document>();

            string connetionString = null;
            SqlConnection connection;
            connetionString = "Data Source=DEGREE-PC;Initial Catalog=sophia_index_patents_g06;Integrated Security=True";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                //Console.WriteLine("Connection Opened!\n");



                SqlCommand command;
                SqlDataReader reader;

                var sql = @"SELECT docs.Doc_Title, docs.Doc_Date, docs.Doc_Author, docs.short_title, docs.Doc_Body, docs.Doc_ID, docs.Doc_Topics, docs.Doc_Place
					        FROM [dbo].[search_Documents] docs
					        WHERE docs.Doc_ID IN ({0});";

                

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;


                var idParameterList = new List<string>();
                var index = 0;
                foreach (var id in docIds)
                {
                    var paramName = "@idParam" + index;
                    command.Parameters.AddWithValue(paramName, id);
                    idParameterList.Add(paramName);
                    index++;
                }

                command.CommandText = String.Format(sql, string.Join(",", idParameterList));

                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(String.Format("TITLE:\t\t{0}\nAUTHORS:\t\t{1}\nBODY:\t\t{2}\n\n",
                        //    reader[0], reader[1], reader[2]));
                        //Console.WriteLine(String.Format("TITLE:\t\t{0}\nDATE:\t\t{1}\nAUTHORS:\t\t{2}\n\n",
                        //    reader[0], reader[1], reader[2]));

                        Document doc = new Document(reader[5].ToString());
                        doc.title = reader[0].ToString();
                        doc.date = reader[1].ToString();
                        doc.author = reader[2].ToString();
                        doc.shortTitle = reader[3].ToString();
                        doc.body = reader[4].ToString();
                        doc.topics = reader[6].ToString();
                        doc.place = reader[7].ToString();

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

        static void createDataSet(string mode, string xmlpath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlpath);

            string targetPath = @".\text-data\" + mode;

            SortedDictionary<string, List<int>> themedCollection = new SortedDictionary<string, List<int>>();

            foreach (XmlNode n in xml.SelectNodes("/docs/section/doc"))
            {
                if (themedCollection.ContainsKey(n.Attributes["class"].Value))
                {
                    themedCollection[n.Attributes["class"].Value].Add(System.Convert.ToInt32(n.InnerText));
                }
                else
                {
                    List<int> tempList = new List<int>();
                    tempList.Add(System.Convert.ToInt32(n.InnerText));
                    themedCollection[n.Attributes["class"].Value] = tempList;
                }
            }

            xml = null;
            GC.Collect();


            foreach (string key in themedCollection.Keys)
            {
                string targetClassPath = targetPath + @"\" + key;
                if (!System.IO.Directory.Exists(targetClassPath))
                {
                    System.IO.Directory.CreateDirectory(targetClassPath);
                }

                List<int> docIds = (themedCollection[key]);


                int MAX = 2000;
                int i = MAX;
                int j = 0;
                while (docIds.Count >= i)
                {
                    List<int> smallPart = new List<int>();
                    smallPart.AddRange(docIds.GetRange(j * MAX, MAX));
                    i += MAX;
                    List<Document> docs = getDocsByInt(smallPart);
                    foreach (var doc in docs)
                    {
                        // Use Path class to manipulate file and directory paths.
                        string destFile = System.IO.Path.Combine(targetClassPath, doc.id);

                        using (FileStream fs = File.Create(destFile))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(doc.body);
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);
                        }
                    }
                    j++;
                }


                List<int> smallPart_ = new List<int>();
                smallPart_.AddRange(docIds.GetRange(i - MAX, docIds.Count - MAX * j));

                List<Document> docs_ = getDocsByInt(smallPart_);
                foreach (var doc in docs_)
                {
                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, doc.id);

                    using (FileStream fs = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(doc.body);
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                }

                Console.WriteLine(key + " end");
            }

            Console.ReadLine();
        }

        static void Count_All()
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

            int countPU = 0;

            foreach (Document doc in docs)
            {
                string[] topics = doc.topics.Split(new Char[] { ';' });

                char[] topic = topics[0].ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();

                foreach (string topic1 in topics)
                {
                    if (topic1.Contains("G06")) {
                        topic = topic1.ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();
                        break;
                    }
                }               


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

                if (topic.Length <= 3)
                {
                    ++countPU;
                    Console.WriteLine(topic);
                    continue;
                }


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
                            sectionA.Add(doc);
                        break;
                    case "B":
                            sectionB.Add(doc);
                        break;
                    case "C":
                            sectionC.Add(doc);
                        break;
                    case "D":
                            sectionD.Add(doc);
                        break;
                    case "E":
                            sectionE.Add(doc);
                        break;
                    case "F":
                            sectionF.Add(doc);
                        break;
                    case "G":
                            sectionG.Add(doc);
                        break;
                    case "H":
                            sectionH.Add(doc);
                        break;
                }
                /*
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
                */
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
            /*
            XmlDocument xmlDoc = new XmlDocument();
            XmlNode rootNode = xmlDoc.CreateElement("docs");
            xmlDoc.AppendChild(rootNode);



            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";
            //settings.NewLineOnAttributes = true;

            XmlWriter trainXmlWriter = XmlWriter.Create(".\\fullset.xml", settings);

            trainXmlWriter.WriteStartDocument();
            trainXmlWriter.WriteStartElement("docs");

            XmlNode sectionNode = xmlDoc.CreateElement("sectionA");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "A");
            foreach (var doc in sectionA)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "A");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionB");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "B");
            foreach (var doc in sectionB)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "B");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionC");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "C");
            foreach (var doc in sectionC)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "C");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionD");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "D");
            foreach (var doc in sectionD)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "D");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionE");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "E");
            foreach (var doc in sectionE)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "E");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionF");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "F");
            foreach (var doc in sectionF)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "F");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionG");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "G");
            foreach (var doc in sectionG)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "G");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            sectionNode = xmlDoc.CreateElement("sectionH");
            trainXmlWriter.WriteStartElement("section");
            trainXmlWriter.WriteAttributeString("class", "H");
            foreach (var doc in sectionH)
            {
                XmlNode docNode = xmlDoc.CreateElement("doc");
                docNode.InnerText = doc.id.ToString();
                rootNode.AppendChild(docNode);
                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", "H");
                trainXmlWriter.WriteString(doc.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            rootNode.AppendChild(sectionNode);
            trainXmlWriter.WriteEndElement();

            xmlDoc.Save("test-doc.xml");
            trainXmlWriter.WriteEndElement();
            trainXmlWriter.WriteEndDocument();
            trainXmlWriter.Close();
            */
            Console.ReadLine();
        }



        static List<Document> GetDocsByTitleLike(string like_arg)
        {
             List<Document> docs = new List<Document>();

            string connetionString = null;
            SqlConnection connection;
            connetionString = "Data Source=DEGREE-PC;Initial Catalog=sophia_index_patents_g06;Integrated Security=True";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                Console.WriteLine("Connection Opened!\n");



                SqlCommand command;
                SqlDataReader reader;

                var sql = @"SELECT docs.Doc_Title, docs.Doc_Date, docs.Doc_Author, docs.short_title, docs.Doc_Body, docs.Doc_ID, docs.Doc_Topics, docs.Doc_Place
					        FROM [dbo].[search_Documents] docs
					        WHERE docs.Doc_Topics LIKE @ARG_LIKE_PARAM;";

                

                command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = CommandType.Text;
                string tmp = String.Format("{0}{1}{0}", "%", like_arg);
                command.Parameters.Add(new SqlParameter("ARG_LIKE_PARAM", tmp));

                command.CommandText = sql;

                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        //Console.WriteLine(String.Format("TITLE:\t\t{0}\nAUTHORS:\t\t{1}\nBODY:\t\t{2}\n\n",
                        //    reader[0], reader[1], reader[2]));
                        //Console.WriteLine(String.Format("TITLE:\t\t{0}\nDATE:\t\t{1}\nAUTHORS:\t\t{2}\n\n",
                        //    reader[0], reader[1], reader[2]));

                        Document doc = new Document(reader[5].ToString());
                        doc.title = reader[0].ToString();
                        doc.date = reader[1].ToString();
                        doc.author = reader[2].ToString();
                        doc.shortTitle = reader[3].ToString();
                        doc.body = reader[4].ToString();
                        doc.topics = reader[6].ToString();
                        doc.place = reader[7].ToString();

                        doc.myClass = like_arg;

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

        static List<Document> GetRandomDocuments(List<Document> docs, int count)
        {
            List<Document> newDocs = new List<Document>();
            Random rnd1 = new Random();

            List<int> usedNumbers = new List<int>();
            for (int i = 0; i < count; ++i)
            {
                int rand = rnd1.Next(0, docs.Count - 1);
                while (usedNumbers.Contains(rand)) {
                    rand = rnd1.Next(0, docs.Count - 1);
                }
                usedNumbers.Add(rand);
                newDocs.Add(docs[rand]);                
            }
            return newDocs;
        }

        static void GenerateJsonDataSet(List<Document> docs)
        {

            Dictionary<string, List<Document>> themedCollection = new Dictionary<string, List<Document>>();

            foreach (var doc in docs)
            {
                if (themedCollection.ContainsKey(doc.myClass))
                {
                    themedCollection[doc.myClass].Add(doc);
                }
                else
                {
                    List<Document> tempList = new List<Document>();
                    tempList.Add(doc);
                    themedCollection[doc.myClass] = tempList;
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            XmlWriter xmlWriter = XmlWriter.Create(".\\themedCollection-20000-3-Train.xml", settings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("docs");

            XmlWriter xmlWriter2 = XmlWriter.Create(".\\themedCollection-20000-3-Test.xml", settings);
            xmlWriter2.WriteStartDocument();
            xmlWriter2.WriteStartElement("docs");
            
            foreach (string key in themedCollection.Keys)
            {
                int allcount = themedCollection[key].Count;
                //int xcount = System.Convert.ToInt32(0.17 * allcount);
                int xcount = 15000;

                xmlWriter.WriteStartElement("section");
                xmlWriter.WriteAttributeString("class", System.Convert.ToString(key));

                xmlWriter2.WriteStartElement("section");
                xmlWriter2.WriteAttributeString("class", System.Convert.ToString(key));

                int i = 0;
                foreach (var doc in themedCollection[key])
                {
                    

                    MemoryStream memoryStream = new MemoryStream();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Document));
                    serializer.WriteObject(memoryStream, doc);

                    string mode;
                    if (i < xcount)
                    {
                        mode = "test";

                        xmlWriter2.WriteStartElement("doc");
                        xmlWriter2.WriteAttributeString("class", System.Convert.ToString(key));
                        xmlWriter2.WriteString(doc.id.ToString());
                        xmlWriter2.WriteEndElement();
                    }
                    else
                    {
                        mode = "train";

                        xmlWriter.WriteStartElement("doc");
                        xmlWriter.WriteAttributeString("class", System.Convert.ToString(key));
                        xmlWriter.WriteString(doc.id.ToString());
                        xmlWriter.WriteEndElement();
                    }

                    string targetPath = @".\text-data-G06F-20000-3\" + mode;
                    string targetClassPath = targetPath + @"\" + key;
                    if (!System.IO.Directory.Exists(targetClassPath))
                    {
                        System.IO.Directory.CreateDirectory(targetClassPath);
                    }
                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, doc.id);

                    using (FileStream fileStream = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(doc.author + "\n" + doc.title + "\n" + doc.body);
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                        //memoryStream.WriteTo(fileStream);
                    }

                    ++i;
                }

                xmlWriter.WriteEndElement();
                xmlWriter2.WriteEndElement();

            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            xmlWriter2.WriteEndElement();
            xmlWriter2.WriteEndDocument();
            xmlWriter2.Close();

            //serializer.WriteObject(memoryStream, docs[0]);
            //stream1.Position = 0;
            //StreamReader sr = new StreamReader(stream1);
            //Console.Write("JSON form of Person object: ");
            //Console.WriteLine(sr.ReadToEnd());
        }

        static void CreateDataSetByCrossValidation(List<Document> docs, int MAX)
        {
            Console.WriteLine("Start processing data set");
            string globalPath = @".\text-data-G06F-20000-CV-" + MAX.ToString() + @"\";
            if (!System.IO.Directory.Exists(globalPath))
            {
                System.IO.Directory.CreateDirectory(globalPath);
            }
            
            DocumentEqualityComparer comparer = new DocumentEqualityComparer();
            
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            int step = 0;
            for (int i = 0; i < 20000; i += MAX)
            {
                string stepPath = globalPath + @"\" + "step-" + step.ToString();
                if (!System.IO.Directory.Exists(stepPath))
                {
                    System.IO.Directory.CreateDirectory(stepPath);
                }

                List<Document> docs_smallpart_train = docs.GetRange(i, MAX);

                string destTrainXmlFile = System.IO.Path.Combine(stepPath, "TrainData.xml");                
                XmlWriter trainXmlWriter = XmlWriter.Create(destTrainXmlFile, settings);
                trainXmlWriter.WriteStartDocument();
                trainXmlWriter.WriteStartElement("docs");
                
                foreach (Document document in docs_smallpart_train)
                {
                    string targetPath = stepPath + @"\" + "train";
                    string targetClassPath = targetPath + @"\" + document.myClass;
                    if (!System.IO.Directory.Exists(targetClassPath))
                    {
                        System.IO.Directory.CreateDirectory(targetClassPath);
                    }

                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                    using (FileStream fileStream = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(document.title + "\n" + document.body);
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                    }

                    trainXmlWriter.WriteStartElement("doc");
                    trainXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                    trainXmlWriter.WriteString(document.id.ToString());
                    trainXmlWriter.WriteEndElement();
                }
                trainXmlWriter.WriteEndElement();
                trainXmlWriter.WriteEndDocument();
                trainXmlWriter.Close();


                string destTestXmlFile = System.IO.Path.Combine(stepPath, "TestData.xml");
                XmlWriter testXmlWriter = XmlWriter.Create(destTestXmlFile, settings);
                testXmlWriter.WriteStartDocument();
                testXmlWriter.WriteStartElement("docs");

                List<Document> docs_smallpart_test = docs.Except(docs_smallpart_train, comparer).ToList();

                foreach (Document document in docs_smallpart_test)
                {
                    string targetPath = stepPath + @"\" + "test";
                    string targetClassPath = targetPath + @"\" + document.myClass;
                    if (!System.IO.Directory.Exists(targetClassPath))
                    {
                        System.IO.Directory.CreateDirectory(targetClassPath);
                    }

                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                    using (FileStream fileStream = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(document.title + "\n" + document.body);
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                    }

                    testXmlWriter.WriteStartElement("doc");
                    testXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                    testXmlWriter.WriteString(document.id.ToString());
                    testXmlWriter.WriteEndElement();
                }
                testXmlWriter.WriteEndElement();
                testXmlWriter.WriteEndDocument();
                testXmlWriter.Close();

                step++;
            }
        }

        static void WriteDataSetIdsToXml(List<Document> docs)
        {
            DateTime saveNow = DateTime.Now;
            string datePatt = @"dd-MM-yyyy-hh-mm-ss-tt";
            string dtString = saveNow.ToString(datePatt);

            string globalPath = @".\" + dtString;
            if (!System.IO.Directory.Exists(globalPath))
            {
                System.IO.Directory.CreateDirectory(globalPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            string destRightOrderXmlFile = System.IO.Path.Combine(globalPath, "AllDataInRightOrder.xml");
            XmlWriter rightOrderXmlWriter = XmlWriter.Create(destRightOrderXmlFile, settings);
            rightOrderXmlWriter.WriteStartDocument();
            rightOrderXmlWriter.WriteStartElement("docs");


            foreach (Document document in docs)
            {
                rightOrderXmlWriter.WriteStartElement("doc");
                rightOrderXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                rightOrderXmlWriter.WriteString(document.id.ToString());
                rightOrderXmlWriter.WriteEndElement();
            }

            rightOrderXmlWriter.WriteEndElement();
            rightOrderXmlWriter.WriteEndDocument();
            rightOrderXmlWriter.Close();
        }

        static List<Document> RestorePreviousDataSet(string xmlpath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlpath);

            List<Document> all_docs = new List<Document>();

            int i = 0;
            foreach (XmlNode n in xml.SelectNodes("/docs/doc"))
            {
                List<int> tempInts = new List<int>();
                tempInts.Add(System.Convert.ToInt32(n.InnerText));
                List<Document> tempList = getDocsByInt(tempInts);
                foreach (var doc in tempList)
                {
                    doc.myClass = n.Attributes["class"].Value;
                }
                all_docs.AddRange(tempList);

                ++i;

                if (i % 100 == 0)
                {
                    Console.WriteLine(String.Format("I = {0}", i.ToString()));
                }
            }

            xml = null;
            GC.Collect();

            return all_docs;
        }

        static void SimulateSplit(List<Document> docs)
        {
            Console.WriteLine("Start processing data set");
            string globalPath = @".\text-data-G06F-20000-CV-Split-Simulate-5000" + @"\";
            if (!System.IO.Directory.Exists(globalPath))
            {
                System.IO.Directory.CreateDirectory(globalPath);
            }

            DocumentEqualityComparer comparer = new DocumentEqualityComparer();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            int step = 3;

            string stepPath = globalPath + @"\" + "step-" + step.ToString();
            if (!System.IO.Directory.Exists(stepPath))
            {
                System.IO.Directory.CreateDirectory(stepPath);
            }

            List<Document> docs_smallpart_train = docs.GetRange(15000, 5000);

            string destTrainXmlFile = System.IO.Path.Combine(stepPath, "TrainData.xml");
            XmlWriter trainXmlWriter = XmlWriter.Create(destTrainXmlFile, settings);
            trainXmlWriter.WriteStartDocument();
            trainXmlWriter.WriteStartElement("docs");

            foreach (Document document in docs_smallpart_train)
            {
                string targetPath = stepPath + @"\" + "train";
                string targetClassPath = targetPath + @"\" + document.myClass;
                if (!System.IO.Directory.Exists(targetClassPath))
                {
                    System.IO.Directory.CreateDirectory(targetClassPath);
                }

                // Use Path class to manipulate file and directory paths.
                string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                using (FileStream fileStream = File.Create(destFile))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(document.author + "\n" + document.title + "\n" + document.body);
                    // Add some information to the file.
                    fileStream.Write(info, 0, info.Length);
                }

                trainXmlWriter.WriteStartElement("doc");
                trainXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                trainXmlWriter.WriteString(document.id.ToString());
                trainXmlWriter.WriteEndElement();
            }
            trainXmlWriter.WriteEndElement();
            trainXmlWriter.WriteEndDocument();
            trainXmlWriter.Close();


            string destTestXmlFile = System.IO.Path.Combine(stepPath, "TestData.xml");
            XmlWriter testXmlWriter = XmlWriter.Create(destTestXmlFile, settings);
            testXmlWriter.WriteStartDocument();
            testXmlWriter.WriteStartElement("docs");

            List<Document> docs_smallpart_test = docs.Except(docs_smallpart_train, comparer).ToList();

            foreach (Document document in docs_smallpart_test)
            {
                string targetPath = stepPath + @"\" + "test";
                string targetClassPath = targetPath + @"\" + document.myClass;
                if (!System.IO.Directory.Exists(targetClassPath))
                {
                    System.IO.Directory.CreateDirectory(targetClassPath);
                }

                // Use Path class to manipulate file and directory paths.
                string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                using (FileStream fileStream = File.Create(destFile))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(document.author + "\n" + document.title + "\n" + document.body);
                    // Add some information to the file.
                    fileStream.Write(info, 0, info.Length);
                }

                testXmlWriter.WriteStartElement("doc");
                testXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                testXmlWriter.WriteString(document.id.ToString());
                testXmlWriter.WriteEndElement();
            }
            testXmlWriter.WriteEndElement();
            testXmlWriter.WriteEndDocument();
            testXmlWriter.Close();
        }

        static Dictionary<string, List<Document>> GetCollectionByClass(List<Document> docs)
        {
            Dictionary<string, List<Document>> themedCollection = new Dictionary<string, List<Document>>();

            foreach (var doc in docs)
            {
                if (themedCollection.ContainsKey(doc.myClass))
                {
                    themedCollection[doc.myClass].Add(doc);
                }
                else
                {
                    List<Document> tempList = new List<Document>();
                    tempList.Add(doc);
                    themedCollection[doc.myClass] = tempList;
                }
            }
            return themedCollection;
        }

        static Dictionary<string, List<Document>> PrintClass(Dictionary<string, List<Document>> themedCollection, string keyClass)
        {
            Dictionary<string, List<Document>> subThemedCollection = new Dictionary<string, List<Document>>();

            int count = 0;

            foreach (var doc in themedCollection[keyClass])
            {
                string pattern = keyClass + @"\d{2,4}?";

                string[] topics = doc.topics.Split(new Char[] { ';' });

                //char[] topic = topics[0].ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();

                bool isFound = false;
                string temp = "Not found";

                foreach (string topic1 in topics)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(topic1.ToUpper().Replace(" ", "").Replace("/", ""),
                                                        pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        isFound = true;
                        temp = topic1.ToUpper().Replace(" ", "").Replace("/", "");
                        break;
                        /*
                        char[] topic = topic1.ToUpper().Replace(" ", "").Replace("/", "").ToCharArray();
                        Document newdoc = new Document(doc);
                        newdoc.topics = new string(topic, 3, topic.Length - 3);

                        cleanedDocs.Add(newdoc);
                        break;
                         * */
                    }
                }

                if (!isFound)
                {
                    Console.WriteLine(doc.id + " : " + temp);
                    continue;
                }

                count++;

                if (subThemedCollection.ContainsKey(temp))
                {
                    subThemedCollection[temp].Add(doc);
                }
                else
                {
                    Document newDoc = new Document(doc);
                   
                    newDoc.myClass = temp;
                    List<Document> tempList = new List<Document>();
                    tempList.Add(newDoc);
                    subThemedCollection[temp] = tempList;
                }
            }

            Console.WriteLine("Count : " + count.ToString());

            return subThemedCollection;
        }

        static void SubClassDataSet(Dictionary<string, List<Document>> trainSet, Dictionary<string, List<Document>> testSet, string parentClass)
        {
            Console.WriteLine("Start processing data set");
            string globalPath = @".\new-proc\text-data-" + parentClass + "-" + trainSet.Count.ToString() + "-" + testSet.Count.ToString() +  @"\";
            if (!System.IO.Directory.Exists(globalPath))
            {
                System.IO.Directory.CreateDirectory(globalPath);
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "\t";

            string destTrainXmlFile = System.IO.Path.Combine(globalPath, "TrainData.xml");
            XmlWriter trainXmlWriter = XmlWriter.Create(destTrainXmlFile, settings);
            trainXmlWriter.WriteStartDocument();
            trainXmlWriter.WriteStartElement("docs");

            

            foreach (string key in trainSet.Keys)
            {
                trainXmlWriter.WriteStartElement("section");
                trainXmlWriter.WriteAttributeString("class", System.Convert.ToString(key));

                foreach (Document document in trainSet[key])
                {
                    string targetPath = globalPath + @"\" + "train";
                    string targetClassPath = targetPath + @"\" + key;
                    if (!System.IO.Directory.Exists(targetClassPath))
                    {
                        System.IO.Directory.CreateDirectory(targetClassPath);
                    }

                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                    using (FileStream fileStream = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(document.title + "\n" + document.body);
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                    }

                    trainXmlWriter.WriteStartElement("doc");
                    trainXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                    trainXmlWriter.WriteString(document.id.ToString());
                    trainXmlWriter.WriteEndElement();
                }
                trainXmlWriter.WriteEndElement();
            }

            trainXmlWriter.WriteEndElement();
            trainXmlWriter.WriteEndDocument();
            trainXmlWriter.Close();

            

            string destTestXmlFile = System.IO.Path.Combine(globalPath, "TestData.xml");
            XmlWriter testXmlWriter = XmlWriter.Create(destTestXmlFile, settings);
            testXmlWriter.WriteStartDocument();
            testXmlWriter.WriteStartElement("docs");

            foreach (string key in testSet.Keys)
            {
                testXmlWriter.WriteStartElement("section");
                testXmlWriter.WriteAttributeString("class", System.Convert.ToString(key));

                foreach (Document document in testSet[key])
                {
                    string targetPath = globalPath + @"\" + "test";
                    string targetClassPath = targetPath + @"\" + key;
                    if (!System.IO.Directory.Exists(targetClassPath))
                    {
                        System.IO.Directory.CreateDirectory(targetClassPath);
                    }

                    // Use Path class to manipulate file and directory paths.
                    string destFile = System.IO.Path.Combine(targetClassPath, document.id);

                    using (FileStream fileStream = File.Create(destFile))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(document.title + "\n" + document.body);
                        // Add some information to the file.
                        fileStream.Write(info, 0, info.Length);
                    }

                    testXmlWriter.WriteStartElement("doc");
                    testXmlWriter.WriteAttributeString("class", document.myClass.ToString());
                    testXmlWriter.WriteString(document.id.ToString());
                    testXmlWriter.WriteEndElement();
                }
                testXmlWriter.WriteEndElement();
            }

            testXmlWriter.WriteEndElement();
            testXmlWriter.WriteEndDocument();
            testXmlWriter.Close();
        }

        static void Main(string[] args)
        {
            /*
            string targetPath = @".\text-data";

            // To copy a folder's contents to a new location:
            // Create a new target folder, if necessary.
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }


            List<Document> docs = new List<Document>();
            docs.AddRange(GetDocsByTitleLike("G06F3"));
            docs.AddRange(GetDocsByTitleLike("G06F7"));
            docs.AddRange(GetDocsByTitleLike("G06F15"));
            docs.AddRange(GetDocsByTitleLike("G06F17"));

            docs = GetRandomDocuments(docs, 20000);
            WriteDataSetIdsToXml(docs);

            //GenerateJsonDataSet(docs);

            CreateDataSetByCrossValidation(docs, 1000);
            CreateDataSetByCrossValidation(docs, 2000);
            CreateDataSetByCrossValidation(docs, 4000);
            CreateDataSetByCrossValidation(docs, 5000);

            //createDataSet("train", ".\\themedCollectionTrain.xml");
            //createDataSet("test", ".\\themedCollectionTest.xml");
             * */
            /*
            string train_data_path = @"C:\Users\surat_000\Documents\Visual Studio 2013\Projects\searchDB_CS\";
            train_data_path += @"searchDB_CS\bin\Debug\06-06-2015-09-44-13-PM\AllDataInRightOrder.xml";
            List<Document> docs = RestorePreviousDataSet(train_data_path);
            //CreateDataSetByCrossValidation(docs, 10000);
            SimulateSplit(docs);
             * */

            string train_data_path = @"C:\Users\surat_000\Documents\Visual Studio 2013\Projects\searchDB_CS\";
            train_data_path += @"searchDB_CS\bin\Debug\text-data-G06F-20000-CV-1000\step-1\TestData.xml";

            List<Document> train_doc = RestorePreviousDataSet(train_data_path);
            Dictionary<string, List<Document>> trainThemedCollection = GetCollectionByClass(train_doc);

            Dictionary<string, List<Document>> G06F7_trainSubCollection = PrintClass(trainThemedCollection, "G06F7");
            Dictionary<string, List<Document>> G06F3_trainSubCollection = PrintClass(trainThemedCollection, "G06F3");
            Dictionary<string, List<Document>> G06F15_trainSubCollection = PrintClass(trainThemedCollection, "G06F15");
            Dictionary<string, List<Document>> G06F17_trainSubCollection = PrintClass(trainThemedCollection, "G06F17");




            string test_data_path = @"C:\Users\surat_000\Documents\Visual Studio 2013\Projects\searchDB_CS\";
            test_data_path += @"searchDB_CS\bin\Debug\text-data-G06F-20000-CV-1000\step-1\TrainData.xml";

            List<Document> test_doc = RestorePreviousDataSet(test_data_path);
            Dictionary<string, List<Document>> testThemedCollection = GetCollectionByClass(test_doc);

            Dictionary<string, List<Document>> G06F7_testSubCollection = PrintClass(testThemedCollection, "G06F7");
            Dictionary<string, List<Document>> G06F3_testSubCollection = PrintClass(testThemedCollection, "G06F3");
            Dictionary<string, List<Document>> G06F15_testSubCollection = PrintClass(testThemedCollection, "G06F15");
            Dictionary<string, List<Document>> G06F17_testSubCollection = PrintClass(testThemedCollection, "G06F17");


            SubClassDataSet(G06F3_trainSubCollection, G06F3_testSubCollection, "G06F3");
            SubClassDataSet(G06F7_trainSubCollection, G06F7_testSubCollection, "G06F7");
            SubClassDataSet(G06F15_trainSubCollection, G06F15_testSubCollection, "G06F15");
            SubClassDataSet(G06F17_trainSubCollection, G06F17_testSubCollection, "G06F17");
        }
    }
}
