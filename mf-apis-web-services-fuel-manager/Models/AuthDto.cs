using System.ComponentModel.DataAnnotations;

namespace mf_apis_web_services_fuel_manager.Models
{
    public class AuthDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
