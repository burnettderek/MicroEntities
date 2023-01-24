using MicroEntities.Attributes;

namespace TestBed
{
	public class Customer
	{
		[ReadOnly][Identity(1, 1)] public int Id { get; set; }
		public string? UserName { get; set; }
		public string? Password { get; set; }
		[ReadOnly][Default(Default.Func.NewGuid)][PrimaryKey] public Guid Key { get; set; }
		[ReadOnly][Default(Default.Func.TimeStamp)] public DateTime? CreatedOn { get; set; }
		public DateTime? LastUpdatedOn { get; set; }
		public decimal? Balance { get; set; }
	}

	public class CustomerDto
	{
		public string? UserName { get; set; }
		public string? Password { get; set; }
		[ReadOnly]
		public Guid? Key { get; set; }
		[ReadOnly]
		public DateTime? CreatedOn { get; set; }
		public decimal? Balance { get; set; }
	}
}
