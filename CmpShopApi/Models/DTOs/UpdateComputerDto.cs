using System.ComponentModel.DataAnnotations.Schema;

namespace CmpShopApi.Models.DTOs
{
    public class UpdateComputerDto
    {
        public string? Brand { get; set; }
        public string? Type { get; set; }
        public double Display { get; set; }
        public int Memory { get; set; }
    }
}
