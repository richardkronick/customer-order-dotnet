using System.ComponentModel.DataAnnotations;

namespace Customer_Orders_C_.Models
{
    public class Customer
    {
        private Customer() { }

        public Customer(string customerId, string firstName, string lastName, int age, CustomerAddress address, string phoneNumber, string email)
        {
            Id = customerId ?? throw new ArgumentNullException(nameof(customerId));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Age = age;
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        [Key]
        public string Id { get; init; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        public CustomerAddress Address { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
