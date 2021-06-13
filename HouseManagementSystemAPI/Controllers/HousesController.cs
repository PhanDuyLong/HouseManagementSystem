using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HMS.Data.Models;
using HMS.Data.Services;
using HMS.Data.ViewModels;
using HMS.Data.ViewModels.HouseViewModels;

namespace HouseManagementSystemAPI.Controllers
{
    [Route("api/houses/")]
    [ApiController]
    public class HousesController : ControllerBase
    {
        private readonly HMSDBContext _context;
        private IHouseService _houseService;
        public HousesController(HMSDBContext context, IHouseService houseService)
        {
            _context = context;
            _houseService = houseService;
        }
        /// <summary>
        /// Get Houses
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        // GET: api/houses/abc
        [HttpGet]
        public List<HouseBaseViewModel> GetHouses(String username)
        {
            return _houseService.GetByOwnerUsername(username);
        }

        /// <summary>
        /// Get House by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public HouseDetailViewModel GetHouse(string id)
        {
            return _houseService.GetByID(id);
        }
/*
        // PUT: api/houses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("house/{id}")]
        [HttpPut]
        public async Task<IActionResult> PutHouse(string id, House house)
        {
            if (id != house.Id)
            {
                return BadRequest();
            }

            _context.Entry(house).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(id))
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

        // POST: api/Houses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("house")]
        [HttpPost]
        public async Task<ActionResult<House>> PostHouse(House house)
        {
            _context.Houses.Add(house);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (HouseExists(house.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetHouse", new { id = house.Id }, house);
        }

        // DELETE: api/Houses/5
        [Route("house/{id}")]
        [HttpDelete]
        public async Task<ActionResult<House>> DeleteHouse(string id)
        {
            var house = await _context.Houses.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            return house;
        }

        private bool HouseExists(string id)
        {
            return _context.Houses.Any(e => e.Id == id);
        }*/
    }
}
