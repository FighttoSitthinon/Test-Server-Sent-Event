using System.Diagnostics;
using System.Net.ServerSentEvents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TestSSE.Models;

namespace TestSSE.Controllers
{
    public class HomeController(SseBroadcaster broadcaster) : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ServerSentEventsResult<string> GetEvents(CancellationToken ct)
        {
            return TypedResults.ServerSentEvents(broadcaster.Subscribe(ct));
        }

        [HttpPost]
        public IActionResult SubmitRequest([FromBody] SubmitRequestRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            broadcaster.Broadcast(request.Message.Trim());
            return Accepted();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
