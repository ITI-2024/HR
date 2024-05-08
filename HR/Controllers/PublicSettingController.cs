using HR.DTO;
using HR.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;

namespace HR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class PublicSettingController : ControllerBase
    {
        private readonly HRDbcontext db;
        public PublicSettingController(HRDbcontext _db) {
            this.db = _db;
        }
        [HttpPost]
       // [Authorize(Roles = "PublicSetting.Create")]
        public IActionResult AddSetting(PublicSetting s) { 
            if(s == null) return BadRequest();
            if(ModelState.IsValid)
            {
                var settingofEmp = db.PublicSettings.ToList();
                if(settingofEmp.Count > 0 ) return BadRequest("Already Exist public Setting");
                db.PublicSettings.Add(s);
                db.SaveChanges();
                return Ok(s);
            }
           return BadRequest();

        }
        [HttpGet]
        // [Authorize(Roles = "PublicSetting.View")]
        public IActionResult getSetting()
        {
            var setting=db.PublicSettings.ToList();
            if (setting is null) return NotFound();
            return Ok(setting);

        }
        [HttpPut("{id}")]
        // [Authorize(Roles = "PublicSetting.Update")]
        public IActionResult UpdateSetting(int id, publicSettingDto s) 
        {
            if (s == null) return BadRequest();
            PublicSetting ps = new PublicSetting();
            if (ModelState.IsValid)
            {
                var settingofEmp = db.PublicSettings.ToList();
                if (settingofEmp.Count == 0)

                {
                   
                    ps.extraHours = s.extraHours;
                    ps.deductionHours= s.deductionHours;
                    ps.firstWeekend= s.firstWeekend;
                    ps.secondWeekend= s.secondWeekend;
                    db.PublicSettings.Add(ps);
                    db.SaveChanges();
                    return Ok(ps);
                }
                else
                { 
                    var dps=db.PublicSettings.FirstOrDefault(s=>s.id==id);
                    dps.extraHours = s.extraHours;
                    dps.deductionHours = s.deductionHours;
                    dps.firstWeekend = s.firstWeekend;
                    dps.secondWeekend = s.secondWeekend;
                  
                    db.SaveChanges();
                    return Ok(dps);
                }
            }
            return BadRequest();
            } 
    }    
    
}
