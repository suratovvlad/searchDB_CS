namespace searchDB_CS_GUI
{
    partial class SearcherForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.SearchButton = new System.Windows.Forms.Button();
            this.SearchResultListView = new System.Windows.Forms.ListView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.DateLabel = new System.Windows.Forms.Label();
            this.AuthorLabel = new System.Windows.Forms.Label();
            this.ContentLabel = new System.Windows.Forms.Label();
            this.TitleTextBox = new System.Windows.Forms.TextBox();
            this.DateTextBox = new System.Windows.Forms.TextBox();
            this.AuthorTextBox = new System.Windows.Forms.TextBox();
            this.ContentTextBox = new System.Windows.Forms.TextBox();
            this.SearchLabel = new System.Windows.Forms.Label();
            this.SearchResultsLabel = new System.Windows.Forms.Label();
            this.StemmedLabel = new System.Windows.Forms.Label();
            this.StemmedTextBox = new System.Windows.Forms.TextBox();
            this.TopicsLabel = new System.Windows.Forms.Label();
            this.TopicsTextBox = new System.Windows.Forms.TextBox();
            this.PlaceLabel = new System.Windows.Forms.Label();
            this.PlaceTextBox = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(52, 35);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(231, 20);
            this.SearchBox.TabIndex = 0;
            this.SearchBox.Text = "network";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(289, 33);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(88, 23);
            this.SearchButton.TabIndex = 1;
            this.SearchButton.Text = "Go";
            this.SearchButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // SearchResultListView
            // 
            this.SearchResultListView.BackColor = System.Drawing.SystemColors.Control;
            this.SearchResultListView.Location = new System.Drawing.Point(52, 89);
            this.SearchResultListView.MultiSelect = false;
            this.SearchResultListView.Name = "SearchResultListView";
            this.SearchResultListView.Size = new System.Drawing.Size(325, 345);
            this.SearchResultListView.TabIndex = 2;
            this.SearchResultListView.TileSize = new System.Drawing.Size(300, 30);
            this.SearchResultListView.UseCompatibleStateImageBehavior = false;
            this.SearchResultListView.View = System.Windows.Forms.View.Tile;
            this.SearchResultListView.SelectedIndexChanged += new System.EventHandler(this.SearchResultsListView_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(972, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(38, 17);
            this.toolStripStatusLabel1.Text = "status";
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Location = new System.Drawing.Point(399, 66);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(27, 13);
            this.TitleLabel.TabIndex = 4;
            this.TitleLabel.Text = "Title";
            // 
            // DateLabel
            // 
            this.DateLabel.AutoSize = true;
            this.DateLabel.Location = new System.Drawing.Point(399, 92);
            this.DateLabel.Name = "DateLabel";
            this.DateLabel.Size = new System.Drawing.Size(30, 13);
            this.DateLabel.TabIndex = 5;
            this.DateLabel.Text = "Date";
            // 
            // AuthorLabel
            // 
            this.AuthorLabel.AutoSize = true;
            this.AuthorLabel.Location = new System.Drawing.Point(399, 118);
            this.AuthorLabel.Name = "AuthorLabel";
            this.AuthorLabel.Size = new System.Drawing.Size(38, 13);
            this.AuthorLabel.TabIndex = 6;
            this.AuthorLabel.Text = "Author";
            // 
            // ContentLabel
            // 
            this.ContentLabel.AutoSize = true;
            this.ContentLabel.Location = new System.Drawing.Point(399, 194);
            this.ContentLabel.Name = "ContentLabel";
            this.ContentLabel.Size = new System.Drawing.Size(44, 13);
            this.ContentLabel.TabIndex = 7;
            this.ContentLabel.Text = "Content";
            // 
            // TitleTextBox
            // 
            this.TitleTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TitleTextBox.Location = new System.Drawing.Point(481, 66);
            this.TitleTextBox.Name = "TitleTextBox";
            this.TitleTextBox.ReadOnly = true;
            this.TitleTextBox.Size = new System.Drawing.Size(473, 13);
            this.TitleTextBox.TabIndex = 8;
            // 
            // DateTextBox
            // 
            this.DateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DateTextBox.Location = new System.Drawing.Point(481, 92);
            this.DateTextBox.Name = "DateTextBox";
            this.DateTextBox.ReadOnly = true;
            this.DateTextBox.Size = new System.Drawing.Size(473, 13);
            this.DateTextBox.TabIndex = 9;
            // 
            // AuthorTextBox
            // 
            this.AuthorTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AuthorTextBox.Location = new System.Drawing.Point(481, 118);
            this.AuthorTextBox.Name = "AuthorTextBox";
            this.AuthorTextBox.ReadOnly = true;
            this.AuthorTextBox.Size = new System.Drawing.Size(473, 13);
            this.AuthorTextBox.TabIndex = 10;
            // 
            // ContentTextBox
            // 
            this.ContentTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ContentTextBox.Location = new System.Drawing.Point(481, 194);
            this.ContentTextBox.Multiline = true;
            this.ContentTextBox.Name = "ContentTextBox";
            this.ContentTextBox.ReadOnly = true;
            this.ContentTextBox.Size = new System.Drawing.Size(473, 240);
            this.ContentTextBox.TabIndex = 11;
            // 
            // SearchLabel
            // 
            this.SearchLabel.AutoSize = true;
            this.SearchLabel.Location = new System.Drawing.Point(49, 19);
            this.SearchLabel.Name = "SearchLabel";
            this.SearchLabel.Size = new System.Drawing.Size(58, 13);
            this.SearchLabel.TabIndex = 12;
            this.SearchLabel.Text = "Enter word";
            // 
            // SearchResultsLabel
            // 
            this.SearchResultsLabel.AutoSize = true;
            this.SearchResultsLabel.Location = new System.Drawing.Point(49, 66);
            this.SearchResultsLabel.Name = "SearchResultsLabel";
            this.SearchResultsLabel.Size = new System.Drawing.Size(74, 13);
            this.SearchResultsLabel.TabIndex = 13;
            this.SearchResultsLabel.Text = "Search results";
            // 
            // StemmedLabel
            // 
            this.StemmedLabel.AutoSize = true;
            this.StemmedLabel.Location = new System.Drawing.Point(399, 40);
            this.StemmedLabel.Name = "StemmedLabel";
            this.StemmedLabel.Size = new System.Drawing.Size(77, 13);
            this.StemmedLabel.TabIndex = 14;
            this.StemmedLabel.Text = "Stemmed word";
            // 
            // StemmedTextBox
            // 
            this.StemmedTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.StemmedTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StemmedTextBox.Location = new System.Drawing.Point(481, 40);
            this.StemmedTextBox.Name = "StemmedTextBox";
            this.StemmedTextBox.ReadOnly = true;
            this.StemmedTextBox.Size = new System.Drawing.Size(473, 13);
            this.StemmedTextBox.TabIndex = 15;
            // 
            // TopicsLabel
            // 
            this.TopicsLabel.AutoSize = true;
            this.TopicsLabel.Location = new System.Drawing.Point(398, 144);
            this.TopicsLabel.Name = "TopicsLabel";
            this.TopicsLabel.Size = new System.Drawing.Size(39, 13);
            this.TopicsLabel.TabIndex = 16;
            this.TopicsLabel.Text = "Topics";
            // 
            // TopicsTextBox
            // 
            this.TopicsTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.TopicsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TopicsTextBox.Location = new System.Drawing.Point(481, 144);
            this.TopicsTextBox.Name = "TopicsTextBox";
            this.TopicsTextBox.ReadOnly = true;
            this.TopicsTextBox.Size = new System.Drawing.Size(473, 13);
            this.TopicsTextBox.TabIndex = 17;
            // 
            // PlaceLabel
            // 
            this.PlaceLabel.AutoSize = true;
            this.PlaceLabel.Location = new System.Drawing.Point(399, 169);
            this.PlaceLabel.Name = "PlaceLabel";
            this.PlaceLabel.Size = new System.Drawing.Size(34, 13);
            this.PlaceLabel.TabIndex = 18;
            this.PlaceLabel.Text = "Place";
            // 
            // PlaceTextBox
            // 
            this.PlaceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PlaceTextBox.Location = new System.Drawing.Point(481, 169);
            this.PlaceTextBox.Name = "PlaceTextBox";
            this.PlaceTextBox.ReadOnly = true;
            this.PlaceTextBox.Size = new System.Drawing.Size(473, 13);
            this.PlaceTextBox.TabIndex = 19;
            // 
            // SearcherForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 459);
            this.Controls.Add(this.PlaceTextBox);
            this.Controls.Add(this.PlaceLabel);
            this.Controls.Add(this.TopicsTextBox);
            this.Controls.Add(this.TopicsLabel);
            this.Controls.Add(this.StemmedTextBox);
            this.Controls.Add(this.StemmedLabel);
            this.Controls.Add(this.SearchResultsLabel);
            this.Controls.Add(this.SearchLabel);
            this.Controls.Add(this.ContentTextBox);
            this.Controls.Add(this.AuthorTextBox);
            this.Controls.Add(this.DateTextBox);
            this.Controls.Add(this.TitleTextBox);
            this.Controls.Add(this.ContentLabel);
            this.Controls.Add(this.AuthorLabel);
            this.Controls.Add(this.DateLabel);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.SearchResultListView);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SearcherForm";
            this.Text = "Searcher Application";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SearchBox;
        private System.Windows.Forms.Button SearchButton;
        private System.Windows.Forms.ListView SearchResultListView;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label DateLabel;
        private System.Windows.Forms.Label AuthorLabel;
        private System.Windows.Forms.Label ContentLabel;
        private System.Windows.Forms.TextBox TitleTextBox;
        private System.Windows.Forms.TextBox DateTextBox;
        private System.Windows.Forms.TextBox AuthorTextBox;
        private System.Windows.Forms.TextBox ContentTextBox;
        private System.Windows.Forms.Label SearchLabel;
        private System.Windows.Forms.Label SearchResultsLabel;
        private System.Windows.Forms.Label StemmedLabel;
        private System.Windows.Forms.TextBox StemmedTextBox;
        private System.Windows.Forms.Label TopicsLabel;
        private System.Windows.Forms.TextBox TopicsTextBox;
        private System.Windows.Forms.Label PlaceLabel;
        private System.Windows.Forms.TextBox PlaceTextBox;
    }
}

