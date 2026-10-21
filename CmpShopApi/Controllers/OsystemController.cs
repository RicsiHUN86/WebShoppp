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
            var computers = context.Osystems.Include(o => o.Computers).ToList();

            if (computers.Count > 0)
            {
                return StatusCode(200, new { message = "Sikeres lekeérdezés", result = computers });
            }

            return StatusCode(404, new { message = "Üres adatbázis", result = computers });
        }

        [HttpGet("oneOsystemAndComputers")]
        public object OneOsystemAndComputers([FromQuery]Guid id)
        {
            var computers = context.Osystems
                .Where(o => o.Id == id)
                .Join(context.Computers,
                o => o.Id,
                c => c.OsysId,
                (o,c) => new {o.Name,c});

            if (computers != null)
            {
                return StatusCode(200, new { message = "Sikeres lekeérdezés", result = computers });
            }

            return StatusCode(404, new { message = "Üres adatbázis", result = computers });
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
                .OrderBy(o=>o.OsName)
                .ThenBy(c => c.Ver);
    

            if (osystems != null)
            {
                return StatusCode(200, new { message = "Sikeres lekeérdezés", result = osystems });
            }

            return StatusCode(404, new { message = "Üres adatbázis", result = osystems });
        }

    }
}
