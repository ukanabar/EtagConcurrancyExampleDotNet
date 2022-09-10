using EtagConcurrancyExample.Libs;
using EtagConcurrancyExample.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EtagConcurrancyExample.Controllers
{
    [ApiController]
    [Route("movies")]
    public class MoviesController : ControllerBase
    {
        const string ETAG_HEADER = "ETag";
        const string MATCH_HEADER = "If-Match";

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var model_from_db = new Movie
            {
                Id = 1,
                Title = "Thunderball",
                ReleaseYear = 1965,
            };

            var eTag = HashFactory.GetHash(model_from_db);
            HttpContext.Response.Headers.Add(ETAG_HEADER, eTag);

            if (HttpContext.Request.Headers.ContainsKey(MATCH_HEADER) &&
                HttpContext.Request.Headers[MATCH_HEADER].RemoveQuotes() == eTag)
                return new StatusCodeResult(StatusCodes.Status304NotModified);

            return Ok(model_from_db);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Movie model)
        {
            var model_from_db = new Movie
            {
                Id = 1,
                Title = "Thunderball-changed", // data changed
                ReleaseYear = 1965,
            };

            var eTag = HashFactory.GetHash(model_from_db);
            HttpContext.Response.Headers.Add(ETAG_HEADER, eTag);

            if (!HttpContext.Request.Headers.ContainsKey(MATCH_HEADER) ||
                HttpContext.Request.Headers[MATCH_HEADER].RemoveQuotes() != eTag)
            {
                return new StatusCodeResult(StatusCodes.Status412PreconditionFailed);
            }
            else
            {
                // saving should be OK
            }

            return NoContent();
        }
    }
}
