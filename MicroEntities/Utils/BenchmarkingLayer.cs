using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MicroEntities.Utils
{
	public class BenchmarkingLayer<TEntity> : IEntitySystemLayer<TEntity> where TEntity : new()
	{
		public BenchmarkingLayer(ILogger<BenchmarkingLayer<TEntity>> logger)
		{
			_logger = logger;
		}

		public IEntitySystemLayer<TEntity> AddLayer(IEntitySystemLayer<TEntity> systemLayer)
		{
			_layer = systemLayer;
			return _layer;
		}

		public async Task<CreationResult> Create(TEntity entity)
		{
			var timer = new Stopwatch();
			timer.Start();
			var result = await _layer.Create(entity);
			timer.Stop();
			_logger.LogDebug($"Create '{EntityName}' completed in {timer.ElapsedMilliseconds}ms.");
			return result;
		}

		public async Task Delete(Where? where)
		{
			var timer = new Stopwatch();
			timer.Start();
			await _layer.Delete(where);
			timer.Stop();
			_logger.LogDebug($"Delete '{EntityName}' completed in {timer.ElapsedMilliseconds}ms.");
		}

		public async Task<IEnumerable<TEntity>> Select(Where? clause = null, Sort? sort = null, Page? pagination = null)
		{
			var timer = new Stopwatch();
			timer.Start();
			var result = await _layer.Select(clause, sort, pagination);
			timer.Stop();
			_logger.LogDebug($"Select '{EntityName}' completed in {timer.ElapsedMilliseconds}ms, returning {result?.Count()} records.");
			return result;
		}
		

		public async Task<ValidationResult> Update(TEntity entity, Where where)
		{
			var timer = new Stopwatch();
			timer.Start();
			var result = await _layer.Update(entity, where);
			timer.Stop();
			_logger.LogDebug($"Update '{EntityName}' completed in {timer.ElapsedMilliseconds}ms.");
			return result;
		}

		public async Task<ValidationResult> Update(Set set, Where where)
		{
			var timer = new Stopwatch();
			timer.Start();
			var result = await _layer.Update(set, where);
			timer.Stop();
			_logger.LogDebug($"Line item update '{EntityName}' completed in {timer.ElapsedMilliseconds}ms.");
			return result;
		}

		private IEntitySystemLayer<TEntity>? _layer;
		private ILogger<BenchmarkingLayer<TEntity>> _logger;
		private string EntityName = typeof(TEntity).Name;
	}
}
