using HR.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicSettingController : ControllerBase
    {
        private readonly HRDbcontext db;
        public PublicSettingController(HRDbcontext _db) {
            this.db = _db;
        }
        [HttpPost]
        public IActionResult AddSetting(PublicSetting s) { 
            if(s == null) return BadRequest();
            if(ModelState.IsValid)
            {
                db.PublicSettings.Add(s);
                db.SaveChanges();
                return Ok(s);
            }
           return BadRequest();

        }
        [HttpGet]
        public IActionResult getSetting()
        {
            var setting=db.PublicSettings.ToList();
            if (setting is null) return NotFound();
            return Ok(setting);

        }
        [HttpPut]
        public IActionResult UpdateSetting(PublicSetting s) 
        {
            if (s == null) return BadRequest();
            if (ModelState.IsValid)
            {
                db.PublicSettings.Update(s);
                db.SaveChanges();
                return Ok(s);
            }
            return BadRequest();
            } 
    }    
    
}
