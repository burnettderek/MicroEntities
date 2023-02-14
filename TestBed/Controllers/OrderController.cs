using MicroEntities;
using MicroEntities.Application;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TestBed.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrderController : Controller
	{
		public OrderController(EntityMappingLayer<OrderDto, Order> orderSystem)
		{
			_orderSystem = orderSystem;
		}

		[HttpPost]
		public async Task<ActionResult> Create([FromBody] OrderDto input)
		{
			try
			{
				var result = await _orderSystem.Create(input);
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
				var result = await _orderSystem.Select(Where.Equal("Id", id));
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
				var result = await _orderSystem.Select(Where.Equal(property, value));
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
		public async Task<ActionResult> Edit([FromBody] OrderDto order)
		{
			try
			{
				var result = await _orderSystem.Update(order, Where.Equal("Id", order.Id));
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

		[HttpPut("/{id}/{property}/{value}")]
		public async Task<ActionResult> Edit(Guid id, string property, string value)
		{
			try
			{
				var result = await _orderSystem.Update(Set.Value(property, value), Where.Equal("Id", id));
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


		[HttpDelete("{id}")]
		public async Task<ActionResult> Delete(Guid id)
		{
			await _orderSystem.Delete(Where.Equal("id", id));
			return Ok();
		}

		private EntityMappingLayer<OrderDto, Order> _orderSystem;
	}
}
