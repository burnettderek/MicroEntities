using MicroEntities;
using MicroEntities.Application;
using MicroEntities.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TestBed.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class UserController : Controller
	{
		public UserController(PublicEntitySystem<UserDto, User> userSystem)
		{
			_userSystem = userSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] UserDto user)
		{
			try
			{
				NotNull.Check(nameof(user.UserName), user.UserName); NotNull.Check(nameof(user.Password), user.Password);
				NotNull.Check(nameof(user.Balance), user.Balance);
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
				NotNull.Check(nameof(key), key);
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
				NotNull.Check(nameof(property), property);
				NotNull.Check(nameof(value), value);
				var result = await _userSystem.Select(Where.Match(property, value + "%").
													  And(Where.GreaterThanOrEqual("Balance", 0M)), 
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
		public async Task<ActionResult> EditUser([FromBody] UserDto user)
		{
			try
			{
				NotNull.Check(nameof(user.Key), user.Key);
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
				NotNull.Check(nameof(key), key); NotNull.Check(nameof(property), property); NotNull.Check(nameof(value), value);
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
				NotNull.Check("the value specified", value);
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

		private PublicEntitySystem<UserDto, User> _userSystem;
	}
}
