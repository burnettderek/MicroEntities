using FluentValidation.Results;
using MicroEntities.Attributes;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Collections.Generic;

namespace MicroEntities.Data.Caching
{
	public class SimpleCachingLayer<TEntity> : IEntitySystemLayer<TEntity> where TEntity: new()
	{
		public IEntitySystemLayer<TEntity> AddLayer(IEntitySystemLayer<TEntity> systemLayer)
		{
			_subLayer = systemLayer;
			return _subLayer;
		}

		public async Task<CreationResult> Create(TEntity entity)
		{
			CreationResult result = null;
			if(_subLayer != null)
			{
				result = await _subLayer.Create(entity);
				if(result.Validation != null && !result.Validation.IsValid)
				{
					return result;
				}
			}
			_cache.Add(entity);
			if(result != null)return result;
			return new CreationResult() { Validation = new ValidationResult(), ResultIdentity = 0 };
		}

		public async Task Delete(Where? where)
		{
			Where clause = where;
			while (clause != null)
			{
				var old = Where.Evaluate(clause, _cache);
				foreach (var item in old)
				{
					var index = _cache.IndexOf(item);
					_cache.RemoveAt(index);
				}
				clause = clause.AndClause;
			}
			if(_subLayer != null)
			 await _subLayer.Delete(where);
		}

		public async Task<IEnumerable<TEntity>> Select(Where? clause = null, Sort? sort = null, Page? pagination = null)
		{
			var where = clause;
			IEnumerable<TEntity> results = _cache;
			while (where != null)
			{
				results = Where.Evaluate(clause, results).ToList();
				where = clause.AndClause;
			}
			if (sort != null)
				results = Sort.SortCollection(results, sort);
			if(pagination != null)
				results = Page.Paginate(results, pagination);
			return results;
		}

		public async Task<ValidationResult> Update(TEntity entity, Where where)
		{
			Where clause = where;
			while (clause != null)
			{
				var old = Where.Evaluate(clause, _cache);
				foreach (var item in old)
				{
					var index = _cache.IndexOf(item);
					_cache.RemoveAt(index);
					if (index > _cache.Count)
						_cache.Insert(index, entity);
					else _cache.Add(item);
				}
				clause = clause.AndClause;
			}
			if(_subLayer != null) return await _subLayer.Update(entity, where);
			return new ValidationResult();
		}

		public async Task<ValidationResult> Update(Set set, Where where)
		{
			Where clause = where;
			while (clause != null)
			{
				var old = Where.Evaluate(clause, _cache);
				foreach (var item in old)
				{
					Set s = set;
					while(s != null)
					{
						Set.Apply(item, s);
						s = s.SubSet;
					}
				}
				clause = clause.AndClause;
			}
			if (_subLayer != null) return await _subLayer.Update(set, where);
			return new ValidationResult();
		}

		public async Task Load()
		{
			_cache = (await _subLayer.Select()).ToList();
		}

		protected IEntitySystemLayer<TEntity> _subLayer;
		protected List<TEntity> _cache;
		protected Dictionary<string, List<TEntity>> _queryCache = new Dictionary<string, List<TEntity>>();
	}
}
