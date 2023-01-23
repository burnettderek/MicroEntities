using FluentValidation;
using FluentValidation.Results;

namespace MicroEntities.Validation
{
	public class FluentValidationLayer<TEntity> : IEntitySystemLayer<TEntity> where TEntity : new()
	{
		public FluentValidationLayer(AbstractValidator<TEntity> validator)
		{
			_validator = validator;
		}

		public FluentValidationLayer(Action<AbstractValidator<TEntity>> addValidationFunc)
		{
			_validator = new EntityValidator();
			addValidationFunc(_validator);
		}
		public IEntitySystemLayer<TEntity> AddLayer(IEntitySystemLayer<TEntity> systemLayer)
		{
			_layer = systemLayer;
			return _layer;
		}

		public async Task<CreationResult> Create(TEntity entity)
		{
			var result = await _validator.ValidateAsync(entity);
			if(result.IsValid)
				return await _layer.Create(entity);
			return new CreationResult() { Validation = result };
		}

		public Task Delete(Where? where)
		{
			return _layer.Delete(where);
		}

		public Task<IEnumerable<TEntity>> Select(Where? clause = null, Sort? sort = null)
		{
			return _layer.Select(clause, sort);
		}

		
		public async Task<ValidationResult> Update(TEntity entity, Where where)
		{
			var result = await _validator.ValidateAsync(entity);
			if (result.IsValid)
				result = await _layer.Update(entity, where);
			return result;
		}

		public async Task<ValidationResult> Update(Set set, Where where)
		{
			var entity = (await _layer.Select(where)).FirstOrDefault();
			if (entity != null)
			{
				var s = set;
				while (s != null)
				{
					var prop = typeof(TEntity).GetProperty(s.Property);
					if (prop != null)
					{
						prop.SetValue(entity, s.PropertyValue);
					}
					else throw new ArgumentException($"Property '{s.Property}' was not found on {typeof(TEntity).Name}.");
					s = s.SubSet;
				}
				var result = await _validator.ValidateAsync(entity);
				if (result.IsValid)
					result = await _layer.Update(set, where);
				return result;
			}
			throw new ArgumentException($"No instance of '{where.Property}' set to '{where.Value}' was found. Can't update.");
		}

		private class EntityValidator : AbstractValidator<TEntity>
		{

		}

		private IEntitySystemLayer<TEntity>? _layer;
		private AbstractValidator<TEntity>? _validator;
	}
}
