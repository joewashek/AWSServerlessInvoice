using AWSServerlessInvoice.Models;
using AWSServerlessInvoice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace AWSServerlessInvoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly IOrderQueue _orderQueue;

        public QueueController(IOrderQueue orderQueue)
        {
            _orderQueue = orderQueue;
        }
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            

            if(Request.Form.TryGetValue("recordId",out var recordId))
            {
                await _orderQueue.AddToQueueAsync(recordId);
                return Ok();
            }

            return BadRequest();
        }
    }
}
