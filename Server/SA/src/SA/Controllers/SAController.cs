using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SA.Controllers
{
    [Route("api/[controller]")]
    public class SAController : Controller
    {
        public SAController(ISARepository SAItems)
        {
            this.SAItems = SAItems;
        }
        public ISARepository SAItems { get; set; }
        // PUT api/sa
        [HttpPut]
        public void Put([FromBody]request value)
        {
            try
            {
                SAItems.CheckDBConnection();
                SAItems.UpdateUserBPM(value.username, value.bpm);
            }
            catch (Exception ex)
            {
                
            }
        }
        // POST api/sa
        [HttpPost]
        public IActionResult Post([FromBody] request value)
        {
            try
            {
                SAItems.CheckDBConnection();
                if (value.action == 3)
                {
                    SAItems.StartTraining(value.username);
                    double bpm= 125;
                    switch (value.style)
                    {
                        case 2:
                            bpm = 140;
                            break;
                        case 3:
                            bpm = 80;
                            break;
                    }
                    SAItems.UpdateUserBPM(value.username, value.bpm);
                    return new ObjectResult(SAItems.GetTrackID(value.username, bpm, value.action));
                }
                else
                {
                    SAItems.UpdateUserBPM(value.username, value.bpm);
                    return new ObjectResult(SAItems.GetTrackID(value.username, value.bpm, value.action));
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.InnerException.Message);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            try
            {
                SAItems.CheckDBConnection();
                SAItems.EndTraining(id);
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}
