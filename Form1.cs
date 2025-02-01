using System.Data.SQLite;
namespace task0
{
    public partial class Form1 : Form
    {
        private List<Person> peopleList = new List<Person>();
        private Person selectedPerson = null;

        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadPeople();
        }

        private void InitializeDatabase()
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS People (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        PhoneNumber TEXT NOT NULL,
                        Email TEXT NOT NULL
                    );";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void LoadPeople()
        {
            string connectionString = "Data Source=people.db;Version=3;";
            peopleList.Clear();

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Id, Name, PhoneNumber, Email FROM People";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Person(
                            Convert.ToInt32(reader["Id"]),
                            reader["Name"].ToString(),
                            reader["PhoneNumber"].ToString(),
                            reader["Email"].ToString()
                        );
                        peopleList.Add(person);
                    }
                }
            }

            UpdateListView();
        }

        private void UpdateListView()
        {
            listView1.Items.Clear();
            foreach (var person in peopleList)
            {
                var item = new ListViewItem(person.Name);
                item.SubItems.Add(person.PhoneNumber);
                item.SubItems.Add(person.Email);
                item.Tag = person.Id;
                listView1.Items.Add(item);
            }
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var name = textBox1.Text.Trim();
            var phone = textBox2.Text.Trim();
            var email = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("All fields must be filled.");
                return;
            }

            // Check if a person with the same name already exists
            var existingPerson = peopleList.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingPerson != null)
            {
                // Select and highlight the existing person in the ListView
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Text.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        item.Selected = true;
                        item.EnsureVisible();
                        MessageBox.Show("This name already exists. Selecting the existing entry for editing.");
                        return;
                    }
                }
            }

            if (selectedPerson == null)
            {
                var newPerson = new Person(0, name, phone, email);
                AddPersonToDatabase(newPerson);
                LoadPeople();
            }
            else
            {
                selectedPerson.Name = name;
                selectedPerson.PhoneNumber = phone;
                selectedPerson.Email = email;
                UpdatePersonInDatabase(selectedPerson);
                LoadPeople();
            }
        }


        private void AddPersonToDatabase(Person person)
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    INSERT INTO People (Name, PhoneNumber, Email)
                    VALUES (@Name, @PhoneNumber, @Email)";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", person.Email);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdatePersonInDatabase(Person person)
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE People
                    SET Name = @Name, PhoneNumber = @PhoneNumber, Email = @Email
                    WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", person.Email);
                    command.Parameters.AddWithValue("@Id", person.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (selectedPerson == null) return;

            DeletePersonFromDatabase(selectedPerson.Id);
            LoadPeople();

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedPerson = null;
            UpdateButtonState();
        }

        private void DeletePersonFromDatabase(int id)
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM People WHERE Id = @Id";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                selectedPerson = null;
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                UpdateButtonState();
                return;
            }

            var selectedItem = listView1.SelectedItems[0];
            var selectedId = (int)selectedItem.Tag;
            selectedPerson = peopleList.FirstOrDefault(p => p.Id == selectedId);

            if (selectedPerson != null)
            {
                textBox1.Text = selectedPerson.Name;
                textBox2.Text = selectedPerson.PhoneNumber;
                textBox3.Text = selectedPerson.Email;
            }

            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            buttonAdd.Text = selectedPerson == null ? "Add" : "Save";
            buttonDelete.Enabled = selectedPerson != null;
        }
    }
}
