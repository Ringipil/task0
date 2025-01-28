namespace task0
{
    // Person class to represent a person
    public class Person : Object
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public Person(string name, string phoneNumber, string email)
        {
            Name = name;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Name}, {PhoneNumber}, {Email}";
        }
    }
}

