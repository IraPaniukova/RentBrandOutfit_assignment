using System.ComponentModel.DataAnnotations;

namespace RentBrandOutfit.Auth //Added Ira 18/10/23
{
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
