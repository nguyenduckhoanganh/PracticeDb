using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entites;
using PracticeDb.Controllers;

public class ProductController : BaseController<ProductController>
{
    public ProductController(PracticeDbContext dbContext,
        ILogger<ProductController> logger,
        IConfiguration config)
        : base(dbContext, logger, config)
    {
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var res = _dbContext.Products;
        return Ok(res);
    }

    [HttpGet("{id:long}")]
    public IActionResult GetDetails(long id)
    {
        return Ok(_dbContext.Customers.Find(id));
    }


    [HttpPost]
    public IActionResult Add([FromBody] Product model)
    {
        _dbContext.Products.Add(model);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }

    [HttpPut]
    public IActionResult Edit([FromBody] Product model)
    {
        var data = _dbContext.Products.Find(model.Id);
        if (data == null) return NotFound("No data found");

        data.Name = model.Name;
        data.Status = model.Status;
        data.Description = model.Description;
        data.UpdatedBy = "";
        data.UpdatedDate = DateTime.Now;

        _dbContext.Products.Update(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }

    [HttpDelete]
    public IActionResult Delete([FromQuery] long id)
    {
        var data = _dbContext.Products.Find(id);
        if (data == null) return NotFound("No data found");

        _dbContext.Products.Remove(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }
}