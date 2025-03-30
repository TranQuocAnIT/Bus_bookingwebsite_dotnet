using Microsoft.AspNetCore.Identity;

namespace DOANCOSO26.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }    
    }
}
