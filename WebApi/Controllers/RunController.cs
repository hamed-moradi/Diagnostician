using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class RunController(RunService runService): BaseController {
    [HttpGet, Route("all")]
    public async Task<IActionResult> All(
      [FromQuery] int expectationId,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string orderBy = "Id",
      [FromQuery] string direction = "asc") {
      var result = await runService.AllAsync(expectationId, new(pageIndex, pageSize, orderBy, direction));
      return Ok(result);
    }

    [HttpGet, Route("single")]
    public async Task<IActionResult> Single([FromQuery] int id) {
      var result = await runService.SingleAsync(id);
      return Ok(result);
    }

    [HttpGet, Route("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      var affecedRows = await runService.DeleteAsync(id);
      return Ok(new { affecedRows });
    }
  }
}