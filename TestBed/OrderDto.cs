using MicroEntities;

namespace TestBed
{
	public class OrderDto
	{
		public DateTime? CreatedOn { get; set; }
		public Guid? Id { get; set; }
		public Guid CustomerKey { get; set; }
		public DateTime? DeliveredOn { get; set; }
	}
}
