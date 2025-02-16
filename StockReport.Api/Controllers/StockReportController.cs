using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using StockReport.Core;
using StockReport.Infrastructure;

namespace StockReport.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockDataController : ControllerBase
    {
        private readonly StockReportRepository _stockDatarepo;

        public StockDataController(StockReportRepository stockDatarepo)
        {
            _stockDatarepo = stockDatarepo;
        }
  [HttpPost("UploadData")]
public async Task<IActionResult> InsertExcelData([FromBody] List<StockReportModels> models)
{
    if (models == null || !models.Any())
    {
        return BadRequest(new { status = 400, message = "Invalid data received" });
    }

    JObject response = await _stockDatarepo.InsertExcelData(models);
    return Ok(response.ToString());
}

  [HttpGet("GetFilterdata")]
public async Task<IActionResult> GetFilterdata(string fiteredvalue)
{
    JObject response = await _stockDatarepo.GetFilteredStockData(fiteredvalue);
    return Ok(response.ToString());
}
    

    }
}