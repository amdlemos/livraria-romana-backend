using System.ComponentModel.DataAnnotations;

namespace LivrariaRomana.Domain.Entities
{
    public class User : IIdentityEntity
    {
        [Required]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Campo obrigatório.")]
        [StringLength(maximumLength:50, MinimumLength = 3, ErrorMessage ="Nome deve conter entre {0} e {1} caractéres.")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Campo obrigatório.")]        
        public string Password { get; set; }
        
        [Required(ErrorMessage = "Campo obrigatório.")]
        [StringLength(maximumLength: 30, ErrorMessage = "Email deve conter nó máximo 30 caractéres.")]               
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Token { get; set; }        
    }
}
