namespace task0
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBox1 = new TextBox();
            buttonAdd = new Button();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            listView1 = new ListView();
            buttonDelete = new Button();
            SuspendLayout();

            listView1.Columns.Add("Name", 150);
            listView1.Columns.Add("Phone", 150);
            listView1.Columns.Add("Email", 200);
            listView1.FullRowSelect = true;
            listView1.Location = new Point(30, 80);
            listView1.Name = "listView1";
            listView1.Size = new Size(600, 200);
            listView1.UseCompatibleStateImageBehavior = false;
            listView1.View = View.Details;
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;

            textBox1.Location = new Point(30, 37);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Name";
            textBox1.Size = new Size(100, 23);

            textBox2.Location = new Point(136, 37);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Phone";
            textBox2.Size = new Size(100, 23);

            textBox3.Location = new Point(242, 37);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Email";
            textBox3.Size = new Size(100, 23);

            buttonAdd.Location = new Point(690, 37);
            buttonAdd.Name = "buttonAdd";
            buttonAdd.Size = new Size(75, 23);
            buttonAdd.Text = "Add";
            buttonAdd.Click += buttonAdd_Click;

            buttonDelete.Enabled = false;
            buttonDelete.Location = new Point(690, 100);
            buttonDelete.Name = "buttonDelete";
            buttonDelete.Size = new Size(75, 23);
            buttonDelete.Text = "Delete";
            buttonDelete.Click += buttonDelete_Click;

            Controls.Add(listView1);
            Controls.Add(textBox1);
            Controls.Add(textBox2);
            Controls.Add(textBox3);
            Controls.Add(buttonAdd);
            Controls.Add(buttonDelete);

            ClientSize = new Size(792, 450);
            Name = "Form1";
            Text = "People Manager";
            ResumeLayout(false);
            PerformLayout();
        }

        private TextBox textBox1;
        private Button buttonAdd;
        private TextBox textBox2;
        private TextBox textBox3;
        private ListView listView1;
        private Button buttonDelete;
    }
}
