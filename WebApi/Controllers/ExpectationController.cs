using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class ExpectationController(ExpectationService expectationService): BaseController {
    [HttpGet, Route("all")]
    public async Task<IActionResult> All(
      [FromQuery] int requestId,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string orderBy = "Id",
      [FromQuery] string direction = "asc") {
      var result = await expectationService.AllAsync(requestId, new(pageIndex, pageSize, orderBy, direction));
      return Ok(result);
    }

    [HttpGet, Route("single")]
    public async Task<IActionResult> Single([FromQuery] int id) {
      var result = await expectationService.SingleAsync(id);
      return Ok(result);
    }

    [HttpPost, Route("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertExpectationBindingModel model) {
      var affecedRows = await expectationService.InsertAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateExpectationBindingModel model) {
      var affecedRows = await expectationService.UpdateAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpGet, Route("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      var affecedRows = await expectationService.DeleteAsync(id);
      return Ok(new { affecedRows });
    }
  }
}