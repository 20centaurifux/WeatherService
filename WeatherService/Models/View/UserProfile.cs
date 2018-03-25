using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService.Models.View
{
    public class UserProfile
    {
        public UserProfile()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Required]
        [StringLength(36)]
        public string Id { get; set; }

        [Required]
        [StringLength(64)]
        public string Username { get; set; }

        [EmailAddress]
        [StringLength(64)]
        public string Email { get; set; }

        [StringLength(64)]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [StringLength(64)]
        [DataType(DataType.Password)]
        public string Password2 { get; set; }

        public bool IsAdmin { get; set; }
    }
}