using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Models.Request;

using Models.Models.Response;

using Models.Entites;
using Common;

namespace PracticeDb.Controllers;

public class CustomerController : BaseController<CustomerController>
{
    public CustomerController(PracticeDbContext dbContext,
        ILogger<CustomerController> logger,
        IConfiguration config)
        : base(dbContext, logger, config)
    {
    }

    [HttpGet]
    public IActionResult GetList()
    {
        var res = _dbContext.Customers.Select(m
            => new CustomerModel
            {
                Address = m.Address,
                Age = m.Age,
                Description = m.Description,
                Gender = m.Gender,
                Id = m.Id,
                Name = m.Name,
                Status = m.Status,
                Username = m.Username
            });
        return Ok(res);
    }

    [HttpGet("{id:long}")]
    public IActionResult GetDetails(long id)
    {
        var customer = _dbContext.Customers.Find(id);
        var product = from sc in _dbContext.Orders
                     join cs in _dbContext.Products on sc.ProductId equals cs.Id
                     where sc.CustomerId == id
                     select new Product
                     {
                         Id = cs.Id,
                         Name = cs.Name,
                         Description = cs.Description,
                         Status = cs.Status
                     };
        ;
        return Ok(new CustomerDetailModel
        {
            Id = customer.Id,
            Name = customer.Name,
            Age = customer.Age,
            Address = customer.Address,
            Gender = customer.Gender,
            Status = customer.Status,
            Username = customer.Username,
            Description = customer.Description,
            Product = product.ToList()
        });
    }


    [HttpPost]
    public IActionResult Add([FromBody] CustomerModel m)
    {
        var data = new Customer
        {
            Address = m.Address,
            Age = m.Age,
            Description = m.Description,
            Gender = m.Gender,
            Id = m.Id,
            Name = m.Name,
            Status = m.Status,
            Username = m.Username
        };
        _dbContext.Customers.Add(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }

    [HttpPut]
    public IActionResult Edit([FromBody] CustomerModel model)
    {
        var data = _dbContext.Customers.Find(model.Id);
        if (data == null) return NotFound("No data found");

        data.Name = model.Name;
        data.Address = model.Address;
        data.Age = model.Age;
        data.Gender = model.Gender;
        data.Status = model.Status;
        data.Description = model.Description;
        data.UpdatedBy = "";
        data.UpdatedDate = DateTime.Now;

        _dbContext.Customers.Update(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }

    [HttpPut]
    public IActionResult ResetPassword([FromBody] ResetPasswordModel model)
    {
        var data = _dbContext.Customers.Find(model.CustomerId);
        if (data == null) return NotFound("No data found");

        data.Salt ??= Guid.NewGuid().ToString();

        var passHash = data.Username.ComputeSha256Hash(data.Salt, model.Password);

        data.Password = passHash;
        data.UpdatedBy = "";
        data.UpdatedDate = DateTime.Now;

        _dbContext.Customers.Update(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }

    [HttpDelete]
    public IActionResult Delete([FromQuery] long id)
    {
        var data = _dbContext.Customers.Find(id);
        if (data == null) return NotFound("No data found");

        _dbContext.Customers.Remove(data);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }


    [HttpPost]
    public IActionResult RegistryCourse(RegistryProductModel model)
    {
        var dataCustomer = _dbContext.Customers.Find(model.CustomerId);
        if (dataCustomer == null) return NotFound("No data found");

        var dataProduct = _dbContext.Products.Find(model.ProductId);
        if (dataProduct == null) return NotFound("No data found");

        var data = _dbContext.Orders.Find(model.CustomerId, model.ProductId);
        if (data != null) return NotFound("Data Exists");

        var product = new Order
        {
            ProductId = model.ProductId,
            CustomerId = model.CustomerId
        };
        _dbContext.Orders.Add(product);
        var eff = _dbContext.SaveChanges();
        return eff > 0 ? Ok("Success") : BadRequest("Failed");
    }
}