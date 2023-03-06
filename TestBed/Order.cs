using MicroEntities.Attributes;

namespace TestBed
{
	public class Order
	{
		[ReadOnly]
		[Default(Default.Func.TimeStamp)]
		public DateTime? CreatedOn { get; set; }
		
		[ReadOnly]
		[Default(Default.Func.NewGuid)]
		[PrimaryKey]
		public Guid Id { get; set; }

		[PrimaryKey]
		public int Sector { get; set; }
		
		[ForeignKey("Customers", "Key")] 
		public Guid CustomerKey { get; set; }
		
		public DateTime? DeliveredOn { get; set; }

	}
}
