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
		public CustomerController(PublicEntitySystem<CustomerDto, Customer> userSystem)
		{
			_userSystem = userSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] CustomerDto user)
		{
			try
			{
				var result = await _userSystem.Create(user);
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
				var result = await _userSystem.Select(Where.Equal("Key", key ));
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

		[HttpGet("/search/{property}/{value}")]
		public async Task<ActionResult> Select(string property, string value)
		{
			try
			{
				var result = await _userSystem.Select(Where.Match(property, value + "%"), 
													  Sort.Descending("CreatedOn"));
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
				var result = await _userSystem.Update(user, Where.Equal(nameof(user.Key), user.Key));
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
					var result = await _userSystem.Update(Set.Value(property, value).And("LastUpdatedOn", DateTime.Now), 
														  Where.Equal("Key", key));
					return Ok(result);
				}
				else
				{
					var result = await _userSystem.Update(Set.Value(property, decimal.Parse(value)).And("LastUpdatedOn", DateTime.Now), 
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
				await _userSystem.Delete(Where.Equal("Key", value));
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

		private PublicEntitySystem<CustomerDto, Customer> _userSystem;
	}
}
