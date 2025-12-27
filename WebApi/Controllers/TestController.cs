using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Model.Binding;
using System.Threading.Tasks;

namespace WebApi.Controllers {
  public class TestController(TestService testService): BaseController {
    [HttpGet, Route("all")]
    public async Task<IActionResult> All(
      [FromQuery] string title,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 10,
      [FromQuery] string orderBy = "Id",
      [FromQuery] string direction = "asc") {
      var result = await testService.AllAsync(title, new(pageIndex, pageSize, orderBy, direction));
      return Ok(result);
    }

    [HttpGet, Route("single")]
    public async Task<IActionResult> Single([FromQuery] int id) {
      var result = await testService.SingleAsync(id);
      return Ok(result);
    }

    [HttpPost, Route("insert")]
    public async Task<IActionResult> Insert([FromBody] InsertTestBindingModel model) {
      if(!ModelState.IsValid)
        return BadRequest(ModelState);
      var affecedRows = await testService.InsertAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpPost, Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTestBindingModel model) {
      var affecedRows = await testService.UpdateAsync(model);
      return Ok(new { affecedRows });
    }

    [HttpGet, Route("delete")]
    public async Task<IActionResult> Delete([FromQuery] int id) {
      var affecedRows = await testService.DeleteAsync(id);
      return Ok(new { affecedRows });
    }
  }
}