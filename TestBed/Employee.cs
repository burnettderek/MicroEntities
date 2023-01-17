using FluentValidation;
using MicroEntities;
using MicroEntities.Attributes;

namespace TestBed
{
	public class Employee
	{
		[ReadOnly]
		public DateTime? CreatedOn { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string? SSN { get; set; }
		[ReadOnly]
		public Guid? Id { get; set; }

	}
}
