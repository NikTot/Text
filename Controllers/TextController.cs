using System;
using System.Collections.Generic;
using System.Linq;
using FTS.Models;
using FTS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FTS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TextController : ControllerBase
    {
        private readonly ILogger<TextController> _logger;
        private ClearText _clearText;
        private AppSettings AppSettings;

        public TextController(ILogger<TextController> logger, IOptions<AppSettings> settings)
        {
            _clearText = new ClearText();
            _logger = logger;
            AppSettings = settings.Value;
        }

        [HttpPost]
        public string Post([FromBody] TextApi text)
        {
            try
            {
                var filters = new List<string>();
                foreach(var item in AppSettings.DefaultFilters.ToList())
                {
                    filters.Add(item.filter);
                }
                
                var result = _clearText.CleanUp(text.value, filters);
                return result;                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return "";
            }
        }
    }
}