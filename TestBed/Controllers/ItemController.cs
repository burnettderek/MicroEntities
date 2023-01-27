using MicroEntities.Application;
using MicroEntities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TestBed.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ItemController : Controller
	{
		public ItemController(PublicEntitySystem<ItemDto, Item> itemSystem)
		{
			_itemSystem = itemSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] ItemDto input)
		{
			try
			{
				var result = await _itemSystem.Create(input);
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
				var result = await _itemSystem.Select(Where.Equal("Key", key));
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

		[HttpGet("search/{property}/{value}")]
		public async Task<ActionResult> Select(string property, string value)
		{
			try
			{
				var result = await _itemSystem.Select(Where.Equal(property, value));
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
		public async Task<ActionResult> Edit([FromBody] ItemDto order)
		{
			try
			{
				var result = await _itemSystem.Update(order, Where.Equal("Key", order.Key));
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

		[HttpPut("replace/{id}/{property}/{value}")]
		public async Task<ActionResult> Edit(Guid key, string property, string value)
		{
			try
			{
				var result = await _itemSystem.Update(Set.Value(property, value), Where.Equal("Key", key));
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


		[HttpDelete("{key}")]
		public async Task<ActionResult> Delete(Guid key)
		{
			await _itemSystem.Delete(Where.Equal("Key", key));
			return Ok();
		}

		private PublicEntitySystem<ItemDto, Item> _itemSystem;
	}
}
