using Microsoft.AspNetCore.Identity;

namespace DatingApp.Models
{
    public class Role :IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
