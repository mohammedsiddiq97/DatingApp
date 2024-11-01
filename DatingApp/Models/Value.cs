using System.ComponentModel.DataAnnotations;

namespace DatingApp.Models
{
    public class Value
    {
        [Key]
        public string Id {  get; set; }
        public string FileName { get; set; }
    }
}
