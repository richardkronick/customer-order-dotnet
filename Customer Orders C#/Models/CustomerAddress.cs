using System.ComponentModel.DataAnnotations;

namespace Customer_Orders_C_.Models
{
    public class CustomerAddress
    {
        private CustomerAddress() { }

        public CustomerAddress(string customerAddressId, string streetAddress, string city, string state, string postalCode)
        {
            Id = customerAddressId ?? throw new ArgumentNullException(nameof(customerAddressId));
            StreetAddress = streetAddress ?? throw new ArgumentNullException(nameof(streetAddress));
            City = city ?? throw new ArgumentNullException(nameof(city));
            State = state ?? throw new ArgumentNullException(nameof(state));
            PostalCode = postalCode ?? throw new ArgumentNullException(nameof(postalCode));
        }

        [Key]
        public string Id { get; init; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }
    }
}
