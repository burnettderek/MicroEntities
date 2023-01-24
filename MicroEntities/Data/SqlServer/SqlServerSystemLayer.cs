using FluentValidation.Results;
using MicroEntities.Attributes;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace MicroEntities.Data.SqlServer
{
    public class SqlServerSystemLayer<TEntity> : IEntitySystemLayer<TEntity> where TEntity: new()
    {
        public SqlServerSystemLayer(ILoggerFactory logFactory, string connectionString, SchemaMode mode = SchemaMode.DataFirst, string? tableName = null)
        {
			_log = logFactory.CreateLogger<SqlServerSystemLayer<TEntity>>();
            ConnectionString = connectionString;
            Properties = typeof(TEntity).GetProperties().ToList();
            InputProperties = Properties.Where(prop => !(prop.GetCustomAttributes(typeof(ReadOnly), true).Length > 0)).ToList();
            if (tableName == null)
                TableName = typeof(TEntity).Name;
            else TableName = tableName;
			if (mode == SchemaMode.CodeFirst)
				EnforceSchema(logFactory);
        }

        public async Task<CreationResult> Create(TEntity entity)
        {
            try
            {
				var rowsCreated = 0;
                var sql = $"INSERT INTO {TableName} ({GetInputPropertyString()}) VALUES ({GetInputPropertyArgs()})";
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        foreach (var property in Properties)
                        {
                            var value = property.GetValue(entity);
                            if(value != null)
                                command.Parameters.AddWithValue(property.Name, value);
                            else
                                command.Parameters.AddWithValue(property.Name, DBNull.Value);
                        }
                        rowsCreated = await command.ExecuteNonQueryAsync();
                    }
					if (rowsCreated > 0)
					{
						using(var command = new SqlCommand($"SELECT IDENT_CURRENT('{TableName}')", connection))
						{
							var id = await command.ExecuteScalarAsync();
							if(id != DBNull.Value)
							{
								return new CreationResult() { ResultIdentity = id };
							}
						}
					}
					connection.Close();
                }
            }
            catch(Exception ex)
            {
                throw;
            }
			if (_subLayer == null)
				return new CreationResult();
			else return await _subLayer.Create(entity);
        }

		public async Task Delete(Where? where)
		{
			if (where == null) throw new ArgumentNullException("Please don't call Delete without a where clause.");
			var sql = $"DELETE FROM {TableName} WHERE [{where.Property}] = @{where.Property}";
			using (var connection = new SqlConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue($"@{where.Property}", where.Value);
					await command.ExecuteNonQueryAsync();
				}
				connection.Close();
			}
			if (_subLayer == null)
				return;
			else await _subLayer.Delete(where);
		}


		public async Task<IEnumerable<TEntity>> Select(Where? clause = null, Sort? sort = null)
		{
			var results = new List<TEntity>();
			var sql = $"SELECT {GetPropertyString()} FROM {TableName}"; 
			if(clause != null)
			{
				sql += $" WHERE {ToSql(clause)}";
				Where? andClause = clause.AndClause;
				while(andClause != null)
				{ 
					sql += $" AND {ToSql(andClause)}";
					andClause = andClause.AndClause;
				}
			}
			if(sort != null)
			{
				sql += ToSql(sort);
			}
			using (var connection = new SqlConnection(ConnectionString))
			{
				connection.Open();
				using (var command = new SqlCommand(sql, connection))
				{
					if (clause != null)
					{
						command.Parameters.AddWithValue("@" + clause.Property, clause.Value);
						Where? andClause = clause.AndClause;
						while(andClause != null)
						{
							command.Parameters.AddWithValue("@" + andClause.Property, andClause.Value);
							andClause = andClause.AndClause;
						}
					}
					var reader = await command.ExecuteReaderAsync();
					while (reader.Read())
					{
						var result = ReadEntity(reader);
						results.Add(result);
					}
				}
				connection.Close();
			}
			if (_subLayer == null)
				return results;
			else return results.Union(await _subLayer.Select(clause, sort));
		}

		public async Task<ValidationResult> Update(TEntity entity, Where where)
        {
			try
			{
				
				var sql = $"UPDATE {TableName} SET {GetUpdatePropertyString()} WHERE [{where.Property}] = @{where.Property}";
				using (var connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (var command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue($"@{where.Property}", where.Value);
						foreach (var property in Properties)
						{
							var value = property.GetValue(entity);
							if (value != null)
								command.Parameters.AddWithValue($"@{property.Name}", value);
							else
								command.Parameters.AddWithValue($"@{property.Name}", DBNull.Value);
						}
						await command.ExecuteNonQueryAsync();
					}
					connection.Close();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			if (_subLayer == null)
				return new ValidationResult();
			else return await _subLayer.Update(entity, where);
		}

		public async Task<ValidationResult> Update(Set set, Where where)
		{
			try
			{
				var sql = $"UPDATE {TableName} SET {ToSql(set)} WHERE [{where.Property}] = @{where.Property}";
				using (var connection = new SqlConnection(ConnectionString))
				{
					connection.Open();
					using (var command = new SqlCommand(sql, connection))
					{
						command.Parameters.AddWithValue($"@{where.Property}", where.Value);
						var s = set;
						while (s != null)
						{
							if (s.PropertyValue != null)
								command.Parameters.AddWithValue($"@{s.Property}", s.PropertyValue);
							else
								command.Parameters.AddWithValue($"@{s.Property}", DBNull.Value);
							s = s.SubSet;
						}

						await command.ExecuteNonQueryAsync();
					}
					connection.Close();
				}
			}
			catch (Exception ex)
			{
				throw;
			}
			if (_subLayer == null)
				return new ValidationResult();
			else return await _subLayer.Update(set, where);
		}

		public IEntitySystemLayer<TEntity> AddLayer(IEntitySystemLayer<TEntity> systemLayer)
        {
			_subLayer = systemLayer;
			return _subLayer;
        }


		private string GetPropertyString()
		{
			var builder = new StringBuilder();
			foreach (var property in Properties)
			{
				builder.Append($"[{property.Name}]");
				if (property != Properties.Last()) builder.Append(", ");
			}
			return builder.ToString();
		}

		private string GetInputPropertyString()
        {
            var builder = new StringBuilder();
            foreach (var property in InputProperties)
            {
                builder.Append($"[{property.Name}]");
                if (property != InputProperties.Last()) builder.Append(", ");
            }
            return builder.ToString();
        }

        private string GetInputPropertyArgs()
        {
			var builder = new StringBuilder();
			foreach (var property in InputProperties)
			{
                builder.Append("@" + property.Name);
                if (property != InputProperties.Last()) builder.Append(", ");
			}
			return builder.ToString();
		}

		private string GetUpdatePropertyString()
		{
			var builder = new StringBuilder();
			foreach (var property in InputProperties)
			{
				builder.Append($"[{property.Name}] = @{property.Name}");
				if (property != InputProperties.Last()) builder.Append(", ");
			}
			return builder.ToString();
		}

		private string ToSql(Sort sort)
		{
			var dir = sort.Direction == Sort.SortDirection.Ascending ? "ASC" : "DESC";
			return $" ORDER BY [{sort.Property}] {dir}";
		}

		private string ToSql(Set set)
		{
			var sql = $"[{set.Property}] = @{set.Property}";
			var subset = set.SubSet;
			while(subset != null)
			{
				sql += $", [{subset.Property}] = @{subset.Property}";
				subset = subset.SubSet;
			}
			return sql;
		}

		private string ToSql(Where clause)
		{
			return $"[{clause.Property}] {ToSql(clause.Operand)} @{clause.Property}";
		}

		private static string ToSql(Operator operand)
		{
			switch (operand)
			{
				case Operator.Equals: return "=";
				case Operator.NotEquals: return "!=";
				case Operator.GreaterThan: return ">";
				case Operator.LessThan: return "<";
				case Operator.GreaterThanOrEqual: return ">=";
				case Operator.LessThanOrEqual: return "<=";
				case Operator.Matches: return "LIKE";
			}
			throw new ArgumentException($"'{operand}' is not supported by the Where.Render method.");
		}

		private string ToSql(int x)
        {
            return x.ToString();
        }

		private string ToSql(double x)
		{
			return x.ToString();
		}

		private string ToSql(decimal x)
		{
			return x.ToString();
		}

		private string ToSql<T>(T x)
        {
            return $"'{x}'";
        }

		private TEntity ReadEntity(SqlDataReader reader)
		{
			TEntity result = new();
			foreach (var property in Properties)
			{
				var val = reader[property.Name];
				if (val != DBNull.Value)
					property.SetValue(result, reader[property.Name]);
				else
					property.SetValue(result, null);
			}
			return result;
		}

		private void EnforceSchema(ILoggerFactory logFactory)
		{
			var enforcer = new SqlServerSchemaEnforcer<TEntity>(logFactory, ConnectionString, TableName);
			enforcer.EnforceSchema().GetAwaiter().GetResult();
		}

        protected string ConnectionString { get; set; }
        protected string TableName { get; set; }
        protected List<PropertyInfo> Properties = new List<PropertyInfo>();
        protected List<PropertyInfo> InputProperties = new List<PropertyInfo>();
		protected IEntitySystemLayer<TEntity>? _subLayer;
		protected ILogger<SqlServerSystemLayer<TEntity>> _log;
    }
}
