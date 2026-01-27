using System.ComponentModel.DataAnnotations;

namespace FINISHARK.DTO.Account
{
    public class LoginDto
    {

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
