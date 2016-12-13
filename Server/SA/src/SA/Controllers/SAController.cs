using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA.Models;
using Microsoft.Extensions.Logging;
using SA.Logs;
using MongoDB.Bson;


// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace SA.Controllers
{
    [Route("api/[controller]")]
    public class SAController : Controller
    {
        readonly ILogger<SAController> _log;
        public SAController(ISARepository SAItems, ILogger<SAController> log)
        {
            this.SAItems = SAItems;
            _log = log;
        }
        public ISARepository SAItems { get; set; }
        // PUT api/sa
        [HttpPut]
        public void Put([FromBody]request value)
        {
            if (!value.is_request_valid())
            {
                _log.LogWarning(LoggingEvents.PUT_REQUEST_ERROR, "request_error" + value.ToBsonDocument().ToString());
            }
            else
            {
                try
                {
                    _log.LogInformation(LoggingEvents.PUT, "request: " + value.ToBsonDocument().ToString());
                    //"Request argumets bpm {0} username {1} action {2} style {3}", value.bpm, value.username, value.action, value.style, );
                    SAItems.CheckDBConnection();
                    if (SAItems.CheckUser(value.username))
                    {
                        SAItems.UpdateUserBPM(value.username, value.bpm.Value);
                    }
                    else
                    {
                        _log.LogWarning(LoggingEvents.PUT_ITEM_NOTFOUND, "request: " + value.ToBsonDocument().ToString());
                    }
                }
                catch (Exception ex)
                {
                    _log.LogError(LoggingEvents.PUT_EXCEPTION, ex, "request: "+ value.ToBsonDocument().ToString());
                }
            }
        }
        // POST api/sa
        [HttpPost]
        public IActionResult Post([FromBody] request value)
        {
            if (!value.is_request_valid())
            {
                _log.LogWarning(LoggingEvents.POST_REQUEST_ERROR, "request_error" + value.ToBsonDocument().ToString());
                return new ObjectResult("request_error");
            }
            try
            {
                _log.LogInformation(LoggingEvents.PUT, "request: "+ value.ToBsonDocument().ToString());
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
                    SAItems.UpdateUserBPM(value.username, bpm);
                    return new ObjectResult(SAItems.GetTrackID(value.username, bpm, value.action.Value));
                }
                else
                {
                    if (SAItems.CheckUser(value.username))
                    {
                        SAItems.UpdateUserBPM(value.username, value.bpm.Value);
                        return new ObjectResult(SAItems.GetTrackID(value.username, value.bpm.Value, value.action.Value));
                    }
                    else
                    {
                        _log.LogWarning(LoggingEvents.POST_ITEM_NOTFOUND, "request: " + value.ToBsonDocument().ToString());
                        return new ObjectResult("user_error");
                    } 
                }
            }
            catch (Exception ex)
            {
                _log.LogError(LoggingEvents.POST_EXCEPTION, ex, "request: " + value.ToBsonDocument().ToString());
                return new ObjectResult(ex.InnerException.Message);
            }
        }
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            try
            {
                _log.LogInformation(LoggingEvents.DELETE_ITEM, "request: " + id);
                SAItems.CheckDBConnection();
                if (SAItems.CheckUser(id))
                {
                    SAItems.EndTraining(id);
                }
                else
                {
                    _log.LogWarning(LoggingEvents.DELETE_ITEM_NOTFOUND, "request: " + id);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(LoggingEvents.DELETE_EXCEPTION, ex, "request: " + id);
            }
        }
    }
}
