using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace searchDB_CS_GUI
{
    public partial class SearcherForm : Form
    {
        private List<Document> docs = new List<Document>();
        private Stemmer stemmer = new Stemmer();

        public SearcherForm()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            SearchResultListView.Items.Clear();
            TitleTextBox.Text = "";
            DateTextBox.Text = "";
            AuthorTextBox.Text = "";
            ContentTextBox.Text = "";
            StemmedTextBox.Text = "";

            string connetionString = null;
            SqlConnection connection;
            connetionString = "Data Source=DEGREE-PC;Initial Catalog=sophia_index_patents_g06;Integrated Security=True";
            connection = new SqlConnection(connetionString);
            try
            {
                connection.Open();
                toolStripStatusLabel1.Text = "Connection Opened!";
                docs.Clear();

                String input = SearchBox.Text.ToLower();
                
                stemmer.add(input.ToCharArray(), input.Length);
                stemmer.stem();

                String stemmedWord = stemmer.ToString();
                StemmedTextBox.Text = stemmedWord;

                SqlCommand command;
                SqlDataReader reader;

                command = new SqlCommand(
                    @"SELECT COUNT(indx.probability)
                        FROM [dbo].[search_words] wrds
                        LEFT JOIN [dbo].[search_terms] trms ON trms.term_id = wrds.term_id
					    LEFT JOIN [dbo].[search_Inverted_index] indx ON (indx.term_ID = trms.term_id)
					    LEFT JOIN [dbo].[search_Documents] docs ON	(indx.Doc_ID = docs.Doc_ID)
                        WHERE (wrds.word_stem = @WORD)
					    ;"
                    , connection);
                command.Parameters.Add(new SqlParameter("WORD", stemmedWord));

                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        toolStripStatusLabel1.Text = String.Format("ALL DOCUMENTS FOUND: {0}", reader[0]);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

                command.Parameters.Clear();
                command = new SqlCommand(
                    @"SELECT TOP 50 indx.probability as probability, docs.Doc_ID as doc
                        FROM [dbo].[search_words] wrds
                        LEFT JOIN [dbo].[search_terms] trms ON trms.term_id = wrds.term_id
					    LEFT JOIN [dbo].[search_Inverted_index] indx ON (indx.term_ID = trms.term_id)
					    LEFT JOIN [dbo].[search_Documents] docs ON	(indx.Doc_ID = docs.Doc_ID)
                        WHERE (wrds.word_stem = @WORD)
					    ORDER BY indx.probability DESC;"
                    , connection);
                command.Parameters.Add(new SqlParameter("WORD", stemmedWord));

                
                reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        docs.Add(new Document(reader[1].ToString()));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }

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

                var items = SearchResultListView.Items;
                foreach (Document doc in docs)
                {
                    items.Add(doc.id + ":" + doc.shortTitle);
                    
                }  

            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Error";
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private void SearchResultsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItems = SearchResultListView.SelectedItems;

            if (selectedItems.Count == 0)
            {
                return;
            }

            String[] properties = selectedItems[0].Text.Split(':');

            Document doc = docs.Find(x => x.id == properties[0]);
            TitleTextBox.Text = doc.title;
            DateTextBox.Text = doc.date;
            AuthorTextBox.Text = doc.author;
            ContentTextBox.Text = doc.body;
        }
    }
}
