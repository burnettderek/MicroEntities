using System.Data.SqlClient;

namespace MicroEntities.Data.SqlServer
{
	public class FreeFormQuery
	{
		private string _connectionString;

		public class Parameter
		{
			public Parameter(string key, object value)
			{
				Key = key;
				Value = value;
			}

			public string Key { get; protected set; }
			public object Value { get; protected set; }
		}

		public FreeFormQuery(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<IEnumerable<T>> SelectObject<T>(string sql, IEnumerable<Parameter>? parameters = null, bool ignoreMissingProperties = false) where T : class, new()
		{
			var results = new List<T>();
			var properties = typeof(T).GetProperties();
			Dictionary<string, int> propertyMap = null;
			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var cmd = new SqlCommand(sql, con))
				{
					if (parameters != null)
					{
						foreach (var param in parameters)
						{
							cmd.Parameters.AddWithValue(param.Key, param.Value);
						}
					}
					var reader = await cmd.ExecuteReaderAsync();
					while (reader.Read())
					{
						if (propertyMap == null)
						{
							propertyMap = new Dictionary<string, int>();
							foreach (var property in properties)
							{
								try
								{
									propertyMap[property.Name] = reader.GetOrdinal(property.Name);
								}
								catch (IndexOutOfRangeException)
								{
									if (!ignoreMissingProperties)
										throw new ArgumentOutOfRangeException($"The class {typeof(T).Name} has a field '{property.Name}' that has no matching column in the sql statement. Either remove that field from your class or set ignoreMissingFields to true.");
								}
							}
						}
						var result = new T();
						foreach (var property in properties)
						{
							int ordinal = propertyMap[property.Name];
							object? value = null;
							if (!reader.IsDBNull(ordinal))
								value = reader[ordinal];
							property.SetValue(result, value);
						}
						results.Add(result);
					}
				}
			}
			return results;
		}

		public async Task<T?> SelectPrimitive<T>(string sql, IEnumerable<Parameter>? parameters = null)
		{
			T? result = default(T);

			using (var con = new SqlConnection(_connectionString))
			{
				con.Open();
				using (var cmd = new SqlCommand(sql, con))
				{
					if (parameters != null)
					{
						foreach (var param in parameters)
						{
							cmd.Parameters.AddWithValue(param.Key, param.Value);
						}
					}
					var reader = await cmd.ExecuteReaderAsync();
					if (reader.Read())
						return (T?)reader[0];

				}
			}
			return result;
		}
	}
}
