using MicroEntities.Attributes;

namespace TestBed
{
	public class User
	{
		[ReadOnly]
		public int Id { get; set; }
		public string? UserName { get; set; }
		public string? Password { get; set; }
		[ReadOnly]
		public Guid? Key { get; set; }
		[ReadOnly]
		public DateTime? CreatedOn { get; set; }
		public DateTime? LastUpdatedOn { get; set; }
		public decimal? Balance { get; set; }
	}

	public class UserDto
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
