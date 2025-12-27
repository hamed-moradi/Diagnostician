using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class RequestController(RequestService requestService): BaseController {
    [HttpGet, Route("all")]
    public async Task<IActionResult> All(
      [FromQuery] int scenarioId,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string orderBy = "Id",
      [FromQuery] string direction = "asc") {
      var result = await requestService.AllAsync(scenarioId, new(pageIndex, pageSize, orderBy, direction));
      return Ok(result);
    }

    [HttpGet, Route("single")]
    public async Task<IActionResult> Single([FromQuery] int id) {
      var result = await requestService.SingleAsync(id);
      return Ok(result);
    }

    [HttpPost, Route("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertRequestBindingModel model) {
      var affecedRows = await requestService.InsertAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateRequestBindingModel model) {
      var affecedRows = await requestService.UpdateAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpGet, Route("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      var affecedRows = await requestService.DeleteAsync(id);
      return Ok(new { affecedRows });
    }
  }
}