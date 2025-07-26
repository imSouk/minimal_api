namespace minimal_api.api.Domain.DTO
{
   
        public record VehicleDto
        {
            public string  Nome { get; set; } = default!;
            public string Marca { get; set; } = default!;
            public int Ano{ get; set; } = default!;
    }
    
}
