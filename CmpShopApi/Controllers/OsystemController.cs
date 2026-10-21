using CmpShopApi.Models;
using CmpShopApi.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CmpShopApi.Controllers
{
    [Route("osystem")]
    [ApiController]
    public class OsystemController : ControllerBase
    {
        public CmpShopDbContext context = new CmpShopDbContext();

        [HttpGet("getAll")]
        public object GetAllOsystem() 
        {
            var osystems = context.Osystems.ToList();
            return new { message = "Sikeres lekérdezés", result = osystems };
        }

        [HttpPost]
        public object AddNewOsystem(AddNewOsystemDto addNewOsystemDto)
        {
            var osystem = new Osystem
            {
                Id = Guid.NewGuid(),
                Name = addNewOsystemDto.Name,
                Version = addNewOsystemDto.Version,
                RegisterTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            context.Osystems.Add(osystem);
            context.SaveChanges();

            return StatusCode(201, new { message = "Sikeres felvétel", result = osystem});
        }

        [HttpPut]
        public object UpdateOsystem([FromQuery]Guid id, [FromBody]UpdateOsystemDto updateOsystemDto)
        {
            var osystem = context.Osystems.FirstOrDefault(osystem => osystem.Id == id );

            if (osystem != null)
            {
                osystem.Name = updateOsystemDto.Name;
                osystem.Version = updateOsystemDto.Version;
                osystem.UpdateTime = DateTime.Now;

                context.Osystems.Update(osystem);
                context.SaveChanges();

                return StatusCode(200, new { message = "Sikeres frissítés", result = osystem });
            }

            return StatusCode(404, new { message = "Sikertlen frissítés", result = osystem });
        }

        [HttpDelete]
        public object DeleteOsystem([FromQuery]Guid id)
        {
            var osystem = context.Osystems.Find(id);

            if (osystem != null)
            {
                context.Osystems.Remove(osystem);
                context.SaveChanges();

                return StatusCode(204, new { message = "Sikeres törlés"});
            }

            return StatusCode(404, new { message = "Sikertlen törlés" });
        }

        [HttpGet("allOsystemAndComputer")]
        public object AllOsystemAndComputer()
        {
            var osystems = context.Osystems.Include(o => o.Computers).ToList();

            if (osystems.Count > 0)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés", result = osystems });
            }

            return StatusCode(404, new { message = "Üres adatbázis", result = osystems });
        }

        [HttpGet("oneOsystemAndComputers/{id}")]
        public object OneOsystemAndComputers(Guid id)
        {
            var osystem = context.Osystems
                .Include(o => o.Computers)
                .FirstOrDefault(o => o.Id == id);

            if (osystem != null)
            {
                return StatusCode(200, new { message = "Sikeres lekérdezés", result = osystem });
            }

            return StatusCode(404, new { message = "Nincs ilyen op-rendszer" });
        }

        [HttpGet("numberOfComputers")]
        public object NumberOfComputers()
        {
            var osystems = context.Osystems
                .GroupJoin(
                context.Computers,
                o => o.Id,
                c => c.OsysId,
                (o, c) => new { OsName = o.Name, Ver = o.Version, Count = c.Count() }
                )
                .OrderBy(o => o.OsName)
                .ThenBy(c => c.Ver)
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = osystems });
        }

        [HttpGet("osystemStats")]
        public object OsStats()
        {
            var osystems = context.Osystems.Include(o => o.Computers).ToList();

            var stats = osystems.Select(o => new
            {
                o.Name,
                o.Version,
                Count = o.Computers.Count,
                MaxMemory = o.Computers.Count > 0 ? o.Computers.Max(c => c.Memory) : 0,
                AvgMemory = o.Computers.Count > 0 ? o.Computers.Average(c => c.Memory) : 0,
                AvgDisplay = o.Computers.Count > 0 ? o.Computers.Average(c => c.Display) : 0
            });

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = stats });
        }

        [HttpGet("osystemWithoutComputer")]
        public object OsWithoutComputer()
        {
            var osystems = context.Osystems
                .Where(o => !context.Computers.Any(c => c.OsysId == o.Id))
                .ToList();

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = osystems });
        }

        [HttpGet("maxMemoryPerOsystem")]
        public object MaxMemoryPerOs()
        {
            var osystems = context.Osystems.Include(o => o.Computers).ToList();

            var result = osystems
                .Where(o => o.Computers.Count > 0)
                .Select(o => new
                {
                    OsName = o.Name,
                    OsVersion = o.Version,
                    Computer = o.Computers.OrderByDescending(c => c.Memory).First()
                });

            return StatusCode(200, new { message = "Sikeres lekérdezés", result = result });
        }

    }
}
