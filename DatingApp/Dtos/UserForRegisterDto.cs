﻿using System.ComponentModel.DataAnnotations;

namespace DatingApp.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [StringLength(8, MinimumLength =4, ErrorMessage ="you must sepcify the password between 4 and 8 charcters")]
        public string Password { get; set; }   
        [Required] 
        public string KnownAs { get; set; }
        [Required] 
        public string Gender { get; set; }
        [Required] 
        public DateTime? DateOfBirth { get; set; } 
        [Required] 
        public string City { get; set; }
        [Required] 
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public UserForRegisterDto()
        {
            Created = DateTime.UtcNow;
            LastActive = DateTime.UtcNow;
        }
    }
}
