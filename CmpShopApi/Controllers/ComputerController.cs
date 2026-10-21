using CmpShopApi.Models;
using CmpShopApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmpShopApi.Controllers
{
    [Route("computer")]
    [ApiController]
    public class ComputerController : ControllerBase
    {
        public CmpShopDbContext context = new CmpShopDbContext();
        [HttpPost]
        public object AddNewCompueter([FromBody]AddNewComputerDto addNewComputerDto)
        {
            var cmp = new Computer
            {
                Id = Guid.NewGuid(),
                Brand = addNewComputerDto.Brand,
                Type = addNewComputerDto.Type,
                Display = addNewComputerDto.Display,
                Memory = addNewComputerDto.Memory,
                CreatedTime = DateTime.Now,
                OsysId = addNewComputerDto.OsysId,
                
            };

            if (cmp != null)
            {
                context.Computers.Add(cmp);
                context.SaveChanges();

                return StatusCode(201, new { message = "Sikeres felvétel", result = cmp});
            }

            return StatusCode(400, new { message = "Sikertelen felvétel", result = cmp });
        }

        [HttpGet]
        public object GetAll() 
        {
            var computers = context.Computers.ToList();

            if (computers.Count > 0) 
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
            }

            return StatusCode(404, new { message = "Üres adatbázis", result = computers });
        }

        [HttpGet("byId")]
        public object GetById([FromQuery]Guid id)
        {
            var computer = context.Computers.SingleOrDefault(x => x.Id == id);

            if (computer != null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés", result = computer });
            }

            return StatusCode(404, new { message = "Nincs találat", result = computer });
        }

        [HttpPut]
        public object UpdateComputer([FromQuery] Guid id, [FromBody]UpdateComputerDto updateComputerDto)
        {
            var computer = context.Computers.Find(id);

            if (computer != null)
            {
                computer.Brand = updateComputerDto.Brand;
                computer.Type = updateComputerDto.Type;
                computer.Display = updateComputerDto.Display;
                computer.Memory = updateComputerDto.Memory;

                context.Computers.Update(computer);
                context.SaveChanges();

                return StatusCode(200, new { message = "Sikeres frissítés", result = computer });
            }

            return StatusCode(404, new { message = "Nincs találat", result = computer });
        }

        [HttpDelete]
        public object DeleteById([FromQuery] Guid id)
        {
            var computer = context.Computers.FirstOrDefault(x => x.Id == id);

            if (computer != null)
            {
                context.Computers.Remove(computer);
                context.SaveChanges();
                return StatusCode(204);
            }

            return StatusCode(404, new { message = "Nincs találat", result = computer });
        }

        [HttpGet("allComputerAndOpsystem")]
        public object AllComputerAndOpsystem()
        {
            var cmp = context.Computers
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Id, c.Brand, c.Type, c.Display, c.Memory, OsName = o.Name, OsVersion = o.Version })
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = cmp });
        }

        [HttpGet("onComputerOneOsystem/{id}")]
        public object OnComputerOneOsystem(Guid id)
        {
            var computer = context.Computers
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Id, c.Brand, c.Type, o.Name, o.Version })
                .FirstOrDefault(c => c.Id == id);

            if (computer != null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés", result = computer });
            }

            return StatusCode(404, new { message = "Nincs ilyen Id-vel találat" });
        }

        [HttpGet("computersByBrand/{brand}")]
        public object ComputersByBrand(string brand)
        {
            var computers = context.Computers
                .Where(c => c.Brand == brand)
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Id, c.Brand, c.Type, c.Display, c.Memory, OsName = o.Name, OsVersion = o.Version })
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
        }

        [HttpGet("computersSorted")]
        public object ComputersSorted()
        {
            var computers = context.Computers
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Brand, c.Type, c.Display, c.Memory, OsName = o.Name, OsVersion = o.Version })
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Type)
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
        }

        [HttpGet("minDisplay/{size}")]
        public object MinDisplay(double size)
        {
            var computers = context.Computers
                .Where(c => c.Display > size)
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Brand, c.Type, c.Display, OsName = o.Name, OsVersion = o.Version })
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
        }

        [HttpGet("computersByOsystem/{name}")]
        public object ComputersByOs(string name)
        {
            var computers = context.Computers
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Id, c.Brand, c.Type, c.Display, c.Memory, OsName = o.Name, OsVersion = o.Version })
                .Where(c => c.OsName == name)
                .ToList();

            if (computers.Count == 0)
            {
                return StatusCode(404, new { message = "Nincs találat", result = computers });
            }

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
        }

        [HttpGet("filterComputers")]
        public object Filter([FromQuery] int? minMemory, [FromQuery] int? osVersion)
        {
            var query = context.Computers
                .Join(context.Osystems,
                c => c.OsysId,
                o => o.Id,
                (c, o) => new { c.Id, c.Brand, c.Type, c.Display, c.Memory, OsName = o.Name, OsVersion = o.Version });

            if (minMemory != null)
            {
                query = query.Where(c => c.Memory >= minMemory);
            }

            if (osVersion != null)
            {
                query = query.Where(c => c.OsVersion == osVersion);
            }

            var computers = query.ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = computers });
        }

    }
}
