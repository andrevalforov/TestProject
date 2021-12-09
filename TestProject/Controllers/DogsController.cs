using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DogsController : ControllerBase
    {
        DogsContext db;
        public DogsController(DogsContext context)
        {
            db = context;

            if (!db.Dogs.Any())
            {
                db.Dogs.Add(new Dog { name = "Neo", color = "red & amber", tail_length = 22, weight = 32 });
                db.Dogs.Add(new Dog { name = "Jessy", color = "black & white", tail_length = 7, weight = 14 });
                db.SaveChanges();
            }
        }

        [Route("/ping")]
        [RateLimitDecorator(StrategyType = StrategyTypeEnum.IpAddress)]
        public string Ping()
        {
            return "Dogs house service. Version 1.0.1";
        }

        [HttpGet]
        [RateLimitDecorator(StrategyType = StrategyTypeEnum.IpAddress)]
        public async Task<ActionResult<IEnumerable<Dog>>> Get(string attribute, string order, int pageNumber = 1, int pageSize = 10)
        {
            if (order != "desc")
            {
                if (attribute == "name")
                    return await db.Dogs.OrderBy(x => x.name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "color")
                    return await db.Dogs.OrderBy(x => x.color).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "tail_length")
                    return await db.Dogs.OrderBy(x => x.tail_length).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "weight")
                    return await db.Dogs.OrderBy(x => x.weight).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else
                    return await db.Dogs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            else //descending
            {
                if (attribute == "name")
                    return await db.Dogs.OrderByDescending(x => x.name).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "color")
                    return await db.Dogs.OrderByDescending(x => x.color).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "tail_length")
                    return await db.Dogs.OrderByDescending(x => x.tail_length).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else if (attribute == "weight")
                    return await db.Dogs.OrderByDescending(x => x.weight).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                else
                    return await db.Dogs.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            }
        }

        [HttpPost]
        [RateLimitDecorator(StrategyType = StrategyTypeEnum.IpAddress)]
        public async Task<ActionResult<Dog>> Post(Dog d)
        {
            if (d == null)
            {
                return BadRequest();
            }

            if (d.tail_length < 0 || d.weight < 0)
            {
                return BadRequest("Incorrect data range");
            }

            foreach (var item in db.Dogs)
            {
                if (item.name.ToLower() == d.name.ToLower())
                    return BadRequest("This name already exists");
            }

            db.Dogs.Add(d);
            await db.SaveChangesAsync();
            return Ok(d);
        }
    }
}
