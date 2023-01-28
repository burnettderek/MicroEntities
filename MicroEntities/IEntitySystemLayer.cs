using FluentValidation.Results;

namespace MicroEntities
{
	public interface IEntitySystemLayer<TPublic> where TPublic : new()
	{
		Task<CreationResult> Create(TPublic entity);
		Task<IEnumerable<TPublic>> Select(Where? clause = null, Sort? sort = null, Page? pagination = null);
		Task<ValidationResult> Update(TPublic entity, Where where);
		Task<ValidationResult> Update(Set set, Where where);
		Task Delete(Where? clause);
		IEntitySystemLayer<TPublic> AddLayer(IEntitySystemLayer<TPublic> systemLayer);
	}
}
