using System.ComponentModel.DataAnnotations.Schema;

namespace CmpShopApi.Models.DTOs
{
    public class AddNewComputerDto
    {
        public string? Brand { get; set; }
        public string? Type { get; set; }
        public double Display { get; set; }
        public int Memory { get; set; }
        public Guid OsysId { get; set; }
    }
}
