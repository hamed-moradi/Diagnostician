using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class ScenarioController(ScenarioService scenarioService): BaseController {
    [HttpGet, Route("all")]
    public async Task<IActionResult> All(
      [FromQuery] string title,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string orderBy = "Id",
      [FromQuery] string direction = "asc") {
      var result = await scenarioService.AllAsync(title, new(pageIndex, pageSize, orderBy, direction));
      return Ok(result);
    }

    [HttpGet, Route("single")]
    public async Task<IActionResult> Single([FromQuery] int id) {
      var result = await scenarioService.SingleAsync(id);
      return Ok(result);
    }

    [HttpPost, Route("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertScenarioBindingModel model) {
      var affecedRows = await scenarioService.InsertAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateScenarioBindingModel model) {
      var affecedRows = await scenarioService.UpdateAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpGet, Route("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      var affecedRows = await scenarioService.DeleteAsync(id);
      return Ok(new { affecedRows });
    }
  }
}