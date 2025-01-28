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
            )";
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
                string query = "SELECT Name, PhoneNumber, Email FROM People";
                using (var command = new SQLiteCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Person(
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
            foreach (var person in peopleList.OrderBy(p => p.Name))
            {
                var item = new ListViewItem(person.Name);
                item.SubItems.Add(person.PhoneNumber);
                item.SubItems.Add(person.Email);
                listView1.Items.Add(item);
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

            if (!email.Contains("@") || !email.Contains("."))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (!phone.All(char.IsDigit) || phone.Length < 7)
            {
                MessageBox.Show("Invalid phone number.");
                return;
            }

            if (selectedPerson == null) // Adding a new person
            {
                var newPerson = new Person(name, phone, email);
                AddPersonToDatabase(newPerson);
                peopleList.Add(newPerson);
                MessageBox.Show("Person added succesfully.");
            }
            else // Editing a selected person
            {
                selectedPerson.Name = name;
                selectedPerson.PhoneNumber = phone;
                selectedPerson.Email = email;
                UpdatePersonInDatabase(selectedPerson);
            }

            UpdateListView();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedPerson = null;
            UpdateButtonState();
        }


        private void SaveToDatabase()
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                foreach (var person in peopleList)
                {
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
        }


        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (selectedPerson == null) return;

            peopleList.Remove(selectedPerson);
            UpdateListView();
            SaveToDatabase();
            DeletePersonFromDatabase(selectedPerson);

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            selectedPerson = null;
            UpdateButtonState();
            MessageBox.Show("Person deleted successfully.");
        }

        private void DeletePersonFromDatabase(Person person)
        {
            string connectionString = "Data Source=people.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM People WHERE Name = @Name AND PhoneNumber = @PhoneNumber AND Email = @Email";
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
            WHERE Name = @Name AND PhoneNumber = @PhoneNumber AND Email = @Email";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", person.Name);
                    command.Parameters.AddWithValue("@PhoneNumber", person.PhoneNumber);
                    command.Parameters.AddWithValue("@Email", person.Email);
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
            selectedPerson = peopleList.FirstOrDefault(p => p.Name == selectedItem.Text);
            textBox1.Text = selectedPerson?.Name ?? string.Empty;
            textBox2.Text = selectedPerson?.PhoneNumber ?? string.Empty;
            textBox3.Text = selectedPerson?.Email ?? string.Empty;
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            buttonAdd.Text = selectedPerson == null ? "Add" : "Save";
            buttonDelete.Enabled = selectedPerson != null;
        }
    }
}
