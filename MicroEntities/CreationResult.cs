using FluentValidation.Results;

namespace MicroEntities
{
	public class CreationResult
	{
		public ValidationResult? Validation { get; set; }
		public object? ResultIdentity { get; set; }
	}
}
