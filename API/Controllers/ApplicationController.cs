using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeeperAPI.DataBase;
using Newtonsoft.Json;
using KeeperAPI.Models;

namespace KeeperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly DataBaseKeeperContext _context;

        public ApplicationController(DataBaseKeeperContext context)
        {
            _context = context;
        }

        // GET: api/Application
        [HttpGet]
        public async Task<ActionResult<string>> GetApplications()
        {
          if (_context.Applications == null)
          {
              return NotFound();
          }

          
            return JsonConvert.SerializeObject( 
                await _context.Applications.Include(i=> i.ApplicaitonCustomers)
                    .ThenInclude(i=> i.Customer)
                    .Include(i=> i.Subdivision)
                    .ToListAsync(),
                new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        // GET: api/Application/5
        [HttpGet("{UserID}")]
        public async Task<ActionResult<IEnumerable<object>>> GetApplication(int UserID)
        {
            if (_context.Applications == null)
            {
              return NotFound();
            }

            var UserApplications = await _context.Applications.Include(i=> i.Subdivision)
                                                         .Include(i => i.ApplicaitonCustomers)
                                                         .Where(x => x.UserId == UserID)
                                                         .Select(s => new 
                                                          {
                                                           Subdivision = s.Subdivision.SubdivisionName,
                                                           BeginVisit = s.BeginVisit,
                                                           Status = s.Status,
                                                           StatusDescription = s.StatusDescription,
                                                           Type = s.ApplicaitonCustomers.Count() > 1 ? "Групповое" : "Одиночное",
                                                           CountCustomers = s.ApplicaitonCustomers.Count()
                                                          })
                                                         .ToListAsync();

            if (UserApplications == null)
            {
                return NotFound();
            }

            return UserApplications;
        }

        // PUT: api/Application/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutApplication(int Id, ApplicationPutModel model)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(x=> x.Id == Id);
            if(application != null)
            {
                if(model.BeginVisit == null && model.EndVisit == null && model.BeginVisitOnSubdivision == null &&
                   model.EndVisitOnSubdivision == null && model.Status == null)
                {
                    return BadRequest();
                }

                if(model.BeginVisit != null)
                {
                    application.BeginVisit = model.BeginVisit;
                    application.Status = "Одобрена";
                }
                if(model.EndVisit != null)
                {
                    application.EndVisit = model.EndVisit;
                    application.Status = "Закрыта";
                    application.StatusDescription = null;
                }
                if(model.BeginVisitOnSubdivision != null)
                {
                    application.BeginVisitOnSubdivision = model.BeginVisitOnSubdivision;
                }
                if(model.EndVisitOnSubdivision != null)
                {
                    application.EndVisitOnSubdivision = model.EndVisitOnSubdivision;
                }
                if(model.Status != null)
                {
                    application.Status = model.Status;
                    application.StatusDescription = model.StatusDescription;
                }

                _context.Entry(application).State = EntityState.Modified;
            }
            else
            {
                return BadRequest();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApplicationExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/Application
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Application>> PostApplication(ApplicationModel model)
        {
            if (_context.Applications == null)
            {
                return Problem("Entity set 'DataBaseKeeperContext.Applications'  is null.");
            }

            try
            {
                var newAppl = new Application()
                {
                    Access = false,                
                    ValidityApplicationBegin = model.ValidityApplicationBegin,
                    ValidityApplicationEnd = model.ValidityApplicationEnd,
                    PasportB64 = model.PasportB64,
                    Purpose = model.Purpose,
                    Status = "В обработке",
                    Subdivision = (await _context.Subdivisions.FirstOrDefaultAsync(x=> x.SubdivisionName == model.SubdivisionName))!,
                    UserId = model.UserId == null ? 31 : model.UserId.Value,
                };

                _context.Applications.Add(newAppl);
                await _context.SaveChangesAsync();

                var currentGroupNumber = model.Customers.Count() > 1 ? _context.ApplicaitonCustomers.Select(s=> s.GroupNumber).Max() + 1 : null;

                foreach (var customer in model.Customers)
                {
                    var newCustomer = new Customer()
                    {
                        Surname = customer.Surname,
                        Name = customer.Name,
                        Patronymic = customer.Patronymic,
                        Phone = customer.Phone,
                        Email = customer.Email,
                        Organization = customer.Organization,
                        Note = customer.Note,
                        SeriaPasport = customer.SeriaPasport,
                        NumberPasport = customer.NumberPasport,
                        Blocked = false,
                        BitrthOfDate = customer.BirthOfDate,
                        PhotoB64 = customer.PhotoB64
                    };

                    _context.Customers.Add(newCustomer);
                    await _context.SaveChangesAsync();

                    var newAppCustomer = new ApplicaitonCustomer()
                    {
                        ApplicationId = newAppl.Id,
                        CustomerId = newCustomer.Id,
                        GroupNumber = currentGroupNumber
                    };

                    _context.ApplicaitonCustomers.Add(newAppCustomer);
                    await _context.SaveChangesAsync();
                }

            }
            catch (System.Exception ex)
            {  
                return Problem(ex.Message);
            }

            return Ok();
        }
        private bool ApplicationExists(int id)
        {
            return (_context.Applications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
