using WebApplication3.Models;

namespace WebApplication3.Models
{
    public class Country
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int SortOrder { get; set; }

        // 🔥 One-to-Many relationship
        public List<Student>? Students { get; set; }
    }
}
