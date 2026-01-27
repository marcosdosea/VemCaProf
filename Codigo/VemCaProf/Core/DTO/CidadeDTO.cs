using System;

namespace Core.DTO
{
    public class CidadeDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Estado { get; set; } = null!;
    }
}