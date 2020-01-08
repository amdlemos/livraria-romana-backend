using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LivrariaRomana.Domain.Entities
{
    public class Book : IIdentityEntity
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage ="Campo obrigatório.")]
        [StringLength(maximumLength: 150, MinimumLength = 1, ErrorMessage = "Máximo 150 caractéres.")]
        public string Title { get; set; }

        [StringLength(maximumLength: 150)]
        public string OriginalTitle { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [StringLength(maximumLength: 150)]
        public string Author { get; set; }

        [StringLength(maximumLength: 150)]
        public string PublishingCompany { get; set; }
        
        public string ISBN { get; set; }

        public int? PublicationYear{ get; set; }
    }
}
