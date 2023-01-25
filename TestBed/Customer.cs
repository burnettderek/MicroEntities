using MicroEntities.Attributes;

namespace TestBed
{
	public class Customer
	{
		[ReadOnly][Identity] 
		public int Id { get; set; }

		[MaxStorageSize(50)]
		public string? FirstName { get; set; }
		
		[MaxStorageSize(50)] 
		public string? LastName { get; set; }

		[ReadOnly]
		[Default(Default.Func.NewGuid)]
		[PrimaryKey]
		public Guid Key { get; set; }

		[ReadOnly]
		[Default(Default.Func.TimeStamp)] 
		public DateTime? CreatedOn { get; set; }
		
		public DateTime? LastUpdatedOn { get; set; }
		
		public decimal? Balance { get; set; }
	}

	public class CustomerDto
	{
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		[ReadOnly]
		public Guid? Key { get; set; }
		[ReadOnly]
		public DateTime? CreatedOn { get; set; }
		public decimal? Balance { get; set; }
	}
}
