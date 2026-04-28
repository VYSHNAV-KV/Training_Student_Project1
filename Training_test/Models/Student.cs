using System.Text.Json.Serialization;

namespace WebApplication3.Models
{
    public class Student
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public DateTime Dob { get; set; }

        // 🔥 Foreign Key
        public Guid CountryId { get; set; }

        public string Gender { get; set; }
        public bool IsIndian { get; set; }

        // 🔥 Navigation Property
        [JsonIgnore]
        public Country? Country { get; set; }
    }
}
