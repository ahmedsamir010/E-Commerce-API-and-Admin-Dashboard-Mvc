using System.ComponentModel.DataAnnotations;

namespace Store.Core.Entities.Identity
{
    public class Address
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required] // Added data annotation
        public string City { get; set; }

        public string Country { get; set; }
        public string Street { get; set; }

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; } // Navigational Property [ ONE ]
    }
}
