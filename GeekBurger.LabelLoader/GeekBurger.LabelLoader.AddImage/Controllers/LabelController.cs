using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekBurger.LabelLoader.Contract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeekBurger.LabelLoader.AddImage.Controllers
{
    [Produces("application/json")]
    [Route("api/Label")]
    public class LabelController : Controller
    {
        [HttpPut("{AddLabelImage}")]
        public IActionResult AddLabel(AddLabelImage label)
        {
            return Ok();
        }

    }
}