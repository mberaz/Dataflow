using Microsoft.AspNetCore.Mvc;
using TPLDataFlowDemo;

namespace DataFlow.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NamesController : ControllerBase
    {
        private readonly INamesProcessor _namesProcessor;

        public NamesController(INamesProcessor namesProcessor)
        {
            _namesProcessor = namesProcessor;
        }


        [HttpPost("{name}")]
        public async Task<int> Create(string name)
        {
            return await _namesProcessor.ProcessName(name);
        }
    }
}
