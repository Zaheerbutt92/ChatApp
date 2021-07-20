using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }  
               
        [Required]
        [StringLength(8, MinimumLength = 4)]
        public string Password { get; set; }

    }
}