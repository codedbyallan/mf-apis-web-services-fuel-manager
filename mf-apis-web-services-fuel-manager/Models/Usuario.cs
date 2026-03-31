using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace mf_apis_web_services_fuel_manager.Models
{
    [Table("Usuários")]
    public class Usuario : LinksHATEOS
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; }
        [Required]
        [JsonIgnore]
        public string Password { get; set; }
        [Required]
        public  Perfil Perfil{ get; set; }

        public ICollection <VeiculoUsuarios> Veiculos { get; set; }
    }

    public enum Perfil
    {
        [Display(Name = "Administrador")]
        Admin,
        [Display(Name = "Usuário")]
        User
    }
}
