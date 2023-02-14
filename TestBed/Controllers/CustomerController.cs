using MicroEntities;
using MicroEntities.Application;
using MicroEntities.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TestBed.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CustomerController : Controller
	{
		public CustomerController(EntityMappingLayer<CustomerDto, Customer> userSystem)
		{
			_customerSystem = userSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] CustomerDto user)
		{
			try
			{
				var result = await _customerSystem.Create(user);
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
		public async Task<ActionResult> Read(Guid key)
		{
			try
			{
				var result = await _customerSystem.Select(Where.Equal("Key", key ));
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

		[HttpGet("/search/{property}/{value}/{page}")]
		public async Task<ActionResult> Select(string property, string value, int page)
		{
			try
			{
				var result = await _customerSystem.Select(Where.Match(property, value), 
													  Sort.Ascending("Balance"), new Page(page, 10));
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


		[HttpPut("/edit")]
		public async Task<ActionResult> EditUser([FromBody] CustomerDto user)
		{
			try
			{
				var result = await _customerSystem.Update(user, Where.Equal(nameof(user.Key), user.Key));
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

		[HttpPut("/edit/{key}/{property}/{value}")]
		public async Task<ActionResult> Edit(string key, string property, string value)
		{
			try
			{
				if (!property.Equals("Balance"))
				{
					var result = await _customerSystem.Update(Set.Value(property, value).And("LastUpdatedOn", DateTime.Now), 
														  Where.Equal("Key", key));
					return Ok(result);
				}
				else
				{
					var result = await _customerSystem.Update(Set.Value(property, decimal.Parse(value)).And("LastUpdatedOn", DateTime.Now), 
														  Where.Equal("Key", key));
					return Ok(result);
				}
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
		public async Task<ActionResult> Delete(Guid value)
		{
			try
			{
				await _customerSystem.Delete(Where.Equal("Key", value));
				return Ok();
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

		private EntityMappingLayer<CustomerDto, Customer> _customerSystem;
	}
}
