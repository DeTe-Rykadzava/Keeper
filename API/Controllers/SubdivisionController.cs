using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeeperAPI.DataBase;

namespace KeeperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubdivisionController : ControllerBase
    {
        private readonly DataBaseKeeperContext _context;

        public SubdivisionController(DataBaseKeeperContext context)
        {
            _context = context;
        }

        // GET: api/Subdivision
        [HttpGet]
        public async Task<ActionResult<object>> GetSubdivisions()
        {
          if (_context.Subdivisions == null)
          {
              return NotFound();
          }
            return await _context.Subdivisions.Include(i=> i.Employees)
            .Select(s=> new{SubdivisionName = s.SubdivisionName, EmployeeName = s.Employees.First().FullName})
            .ToListAsync();
        }
    }
}
