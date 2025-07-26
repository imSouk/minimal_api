using minimal_api.api.Domain.Enums;

namespace minimal_api.api.Domain.DTO
{
   
        public class AdmDTO
        {
            public string Email { get; set; } = default!;
            public string Senha { get; set; } = default!;
            public Perfil Perfil { get; set; } = default!;
    }
    
}
