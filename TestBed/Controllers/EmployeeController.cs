using MicroEntities;
using MicroEntities.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TestBed.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class EmployeeController : Controller
	{
		public EmployeeController(PublicEntitySystem<EmployeeDto, Employee> orderSystem)
		{
			_employeeSystem = orderSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] EmployeeDto input)
		{
			try
			{
				var result = await _employeeSystem.Create(input);
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet]
		public async Task<ActionResult> Read(Guid id)
		{
			try
			{
				var result = await _employeeSystem.Select(Where.Equal("Id", id));
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet("/{property}/{value}")]
		public async Task<ActionResult> Select(string property, string value)
		{
			try
			{
				var result = await _employeeSystem.Select(Where.Equal(property, value));
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}


		[HttpPut]
		public async Task<ActionResult> Edit([FromBody] EmployeeDto employee)
		{
			try
			{
				var result = await _employeeSystem.Update(employee, Where.Equal("SSN", employee.SSN));
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpPut("/{ssn}/{property}/{value}")]
		public async Task<ActionResult> Edit(string ssn, string property, string value)
		{
			try
			{
				var result = await _employeeSystem.Update(Set.Value(property, value), Where.Equal("SSN", ssn));
				return Ok(result);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(ex.Message);
			}
			catch (Exception ex)
			{
				return StatusCode((int)HttpStatusCode.InternalServerError);
			}
		}


		[HttpDelete]
		public ActionResult Delete(int id)
		{
			return View();
		}

		private PublicEntitySystem<EmployeeDto, Employee> _employeeSystem;
	}
}
