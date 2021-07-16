using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }         
        [Required]
        public string Password { get; set; }

    }
}