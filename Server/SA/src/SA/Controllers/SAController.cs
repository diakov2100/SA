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
        // GET: api/values
        [HttpPut]
        public void Put([FromBody]request value)
        {
            SAItems.ConnectDatabase();
            SAItems.UpdateUserBPM(value.username, value.bpm);
        }


        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] request value)
        {
            SAItems.ConnectDatabase();
            SAItems.UpdateUserBPM(value.username, value.bpm);
            if (value.action==2) SAItems.UpdateTrackInfo(value.username, 2);
            return new ObjectResult(SAItems.GetTrackID(value.username, value.bpm));
        }
    }
}
