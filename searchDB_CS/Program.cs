using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace searchDB_CS
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
    }
 
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world");
            
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

                foreach (Document doc in docs) {
                    Console.WriteLine(String.Format("TITLE:\t\t{0}\nDATE:\t\t{1}\nAUTHORS:\t\t{2}\n\n",
                                doc.title, doc.date, doc.author));
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error: {0}", ex.ToString()));
            }

            

            

            Console.ReadLine();
        }
    }
}
