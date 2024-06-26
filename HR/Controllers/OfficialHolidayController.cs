﻿using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")]
    public class OfficialHolidayController : ControllerBase
    {
        private readonly HRDbcontext db;
        public OfficialHolidayController(HRDbcontext _db)
        {
            this.db = _db;
        }
        [HttpGet]
       // [Authorize(Roles = "Holiday.View")]
        public ActionResult GetOfficialHolidays()
        {
            var officialHolidays = db.Holidays.Select(h => h).ToList();
           
            return Ok(officialHolidays);
        }
        [HttpGet("id")]
       // [Authorize(Roles = "Holiday.View")]
        public IActionResult GetHolidayByDate(int id)
        {
            var Holiday = db.Holidays.Where(h =>h.id == id).FirstOrDefault();
            if (Holiday == null) return NotFound();
            return Ok(Holiday);
        }
        [HttpPost]
       // [Authorize(Roles = "Holiday.Create")]
        public IActionResult AddOfficialHolidays(HolidaySetting holiday)
        {
            if (holiday == null) BadRequest();
            if (ModelState.IsValid)
            {
                holiday.dayName= holiday.HolidayDate.DayOfWeek.ToString();
                db.Holidays.Add(holiday);
                db.SaveChanges();
                return Ok(holiday);
            }
            return BadRequest();

        }
        [HttpPut("Edit")]
       // [Authorize(Roles = "Holiday.Update")]
        public IActionResult EditHoliday(HolidaySetting Holiday)
        {
                var officialHoliday = db.Holidays.Where(h => h.id == Holiday.id).FirstOrDefault();
            if (officialHoliday == null) return NotFound();
            officialHoliday.HolidayDate=Holiday.HolidayDate;
            officialHoliday.Name = Holiday.Name;
            officialHoliday.dayName = Holiday.HolidayDate.DayOfWeek.ToString();
            db.SaveChanges();
            return Ok(officialHoliday);
        }
        [HttpDelete("Delete")]
      //  [Authorize(Roles = "Holiday.Delete")]
        public IActionResult DeleteOfficialHoliday(int id)
        {
            var officialHoliday = db.Holidays.Where(h => h.id == id).FirstOrDefault();
            if (officialHoliday == null) return NotFound();
            db.Holidays.Remove(officialHoliday);
            db.SaveChanges();
            return Ok(officialHoliday);
        }
    }
}
