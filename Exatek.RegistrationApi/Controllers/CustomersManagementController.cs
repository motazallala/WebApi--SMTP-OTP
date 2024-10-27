using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exatek.RegistrationCore.Model;
using Exatek.RegistrationEF.AppData;
using Exatek.RegistrationApi.Model.Request;

namespace Exatek.RegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersManagementController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomersManagement
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.AsTracking().ToListAsync();
        }

        // GET: api/CustomersManagement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/CustomersManagement/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(string id, Customer customer)
        {
            if (id != customer.ICNumber)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CustomersManagement
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDto customer)
        {
            _context.Customers.Add(new Customer
            {
                ICNumber = customer.ICNumber,
                Name = customer.Name,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                PhoneVerify = customer.PhoneVerify,
                EmailVerify = customer.EmailVerify,
                Policy = customer.Policy,
                biometric = customer.biometric
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(customer.ICNumber))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomer", new { id = customer.ICNumber }, customer);
        }

        // DELETE: api/CustomersManagement/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // POST: api/Registration/Policy
        [HttpPost("Policy")]
        public async Task<ActionResult> Policy(PolicyDto policyDto)
        {
            var user = await _context.Customers.FindAsync(policyDto.ICNumber);
            if (user is null)
            {
                return BadRequest("User not exist please register");
            }
            user.Policy = policyDto.PolicyStatas;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict("There is an Conflict");
            }
            return Ok();
        }
        // POST: api/Registration/EnableBiomitric
        [HttpPost("EnableBiomitric")]
        public async Task<ActionResult> EnableBiomitric(BiomitricDto biomitricDto)
        {
            var user = await _context.Customers.FindAsync(biomitricDto.ICNumber);
            if (user is null)
            {
                return BadRequest("User not exist please register");
            }
            user.biometric = biomitricDto.BiometricStatas;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Conflict("There is an Conflict");
            }
            return Ok();
        }

        // GET: api/Login/CheckPolicy/5
        [HttpGet("CheckPolicy/{id}")]
        public async Task<ActionResult<bool>> CheckPolicy(string id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer.Policy == true ? true : false;
        }
        // GET: api/Login/CheckPolicy/5
        [HttpGet("CheckBiomitric/{id}")]
        public async Task<ActionResult<bool>> CheckBiomitric(string id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer.biometric == true ? true : false;
        }
        private bool CustomerExists(string id)
        {
            return _context.Customers.Any(e => e.ICNumber == id);
        }
    }
}
