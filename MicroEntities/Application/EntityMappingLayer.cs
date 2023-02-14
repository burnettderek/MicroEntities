using AutoMapper;
using FluentValidation.Results;

namespace MicroEntities.Application
{
	public class EntityMappingLayer<TPublic, TPrivate> : IEntitySystemLayer<TPublic> where TPublic : new() where TPrivate : new()
	{
		public EntityMappingLayer()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<TPublic, TPrivate>();
				cfg.CreateMap<TPrivate, TPublic>();
			});
			_mapper = new Mapper(config);
		}

		public EntityMappingLayer(Mapper mapper)
		{
			_mapper = mapper;
		}
		public async Task<CreationResult> Create(TPublic entity)
		{
			var privateEntity = _mapper.Map<TPrivate>(entity);
			var result = await _layer.Create(privateEntity);
			return result;
		}

		public async Task<IEnumerable<TPublic>> Select(Where? clause = null, Sort? sort = null, Page? pagination = null)
		{
			var privateEntityList = await _layer.Select(clause, sort, pagination);
			var publicEntityList = _mapper.Map<List<TPublic>>(privateEntityList);
			return publicEntityList;
		}
		public async Task<ValidationResult> Update(TPublic entity, Where where)
		{
			var privateEntity = _mapper.Map<TPrivate>(entity);
			var result = await _layer.Update(privateEntity, where);
			return result;
		}

		public async Task<ValidationResult> Update(Set set, Where where)
		{
			return await _layer.Update(set, where);
		}
		public async Task Delete(Where? where)
		{
			await _layer.Delete(where);
		}
		public IEntitySystemLayer<TPrivate> AddLayer(IEntitySystemLayer<TPrivate> layer)
		{
			_layer = layer;
			return layer;
		}

		public IEntitySystemLayer<TPublic> AddLayer(IEntitySystemLayer<TPublic> systemLayer)
		{
			throw new InvalidOperationException("You should only add layers of a private class to mapping layers.");
		}

		protected IEntitySystemLayer<TPrivate>? _layer;
		protected readonly Mapper _mapper;
	}

	
}
