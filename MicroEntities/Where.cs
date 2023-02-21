using MicroEntities.Attributes;
using System.Text.RegularExpressions;

namespace MicroEntities
{
	public enum Operator
	{
		Equals,
		NotEquals,
		GreaterThan,
		LessThan,
		GreaterThanOrEqual,
		LessThanOrEqual,
		Matches
	}
	public class Where
	{
		public Where(string property, Operator operand, object value, Where? andClause = null)
		{
			Property = property;
			Operand = operand;
			Value = value;
			AndClause = andClause;
		}

		public string Property { get; protected set; }
		public Operator Operand { get; protected set; }
		public object Value { get; protected set; }
		public Where? AndClause { get; protected set; }

		public Where And(Where andClause)
		{
			Where? finalAnd = this;
			if (finalAnd != null)
			{
				while (finalAnd.AndClause != null)
				{
					finalAnd = finalAnd.AndClause;
				}
			}
			finalAnd.AndClause = andClause;
			return this;
		}

		public static Where Equal(string property, object value)
		{
			return new Where(property, Operator.Equals, value);
		}
		public static Where NotEqual(string property, object value)
		{
			return new Where(property, Operator.NotEquals, value);
		}
		public static Where GreaterThan(string property, object value)
		{
			return new Where(property, Operator.GreaterThan, value);
		}
		public static Where LessThan(string property, object value)
		{
			return new Where(property, Operator.LessThan, value);
		}
		public static Where GreaterThanOrEqual(string property, object value)
		{
			return new Where(property, Operator.GreaterThanOrEqual, value);
		}
		public static Where LessThanOrEqual(string property, object value)
		{
			return new Where(property, Operator.LessThanOrEqual, value);
		}
		public static Where Match(string property, object? value)
		{
			return new Where(property, Operator.Matches, value);
		}

		public static IEnumerable<TEntity> Evaluate<TEntity>(Where clause, IEnumerable<TEntity> collection)
		{
			var operand = clause.Operand;
			switch(operand)
			{
				case Operator.Equals:
					return WhereEqual(collection, clause.Property, clause.Value);
				case Operator.NotEquals:
					return WhereNotEqual(collection, clause.Property, clause.Value);
				case Operator.GreaterThan:
					return GreaterThan(collection, clause.Property, clause.Value);
				case Operator.LessThan:
					return LessThan(collection, clause.Property, clause.Value);
				case Operator.GreaterThanOrEqual:
					return GreaterThanOrEqual(collection, clause.Property, clause.Value);
				case Operator.LessThanOrEqual:
					return LessThanOrEqual(collection, clause.Property, clause.Value);
				case Operator.Matches:
					return Matches(collection, clause.Property, clause.Value);
				default:
					throw new InvalidOperationException($"Operand '{operand}' not supported.");
			}
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return WhereEqual(collection, property, (int)value);
				case nameof(Guid):
					return WhereEqual(collection, property, (Guid)value);
				case nameof(DateTime):
					return WhereEqual(collection, property, (DateTime)value);
				case nameof(Double):
					return WhereEqual(collection, property, (double)value);
				case nameof(Boolean):
					return WhereEqual(collection, property, (bool)value);
				case nameof(Int64):
					return WhereEqual(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) == value);
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => (Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity) == value);
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => ((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)).Equals(value));
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) == value);
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => (bool)typeof(TEntity).GetProperty(property)?.GetValue(entity) == value);
		}

		public static IEnumerable<TEntity> WhereEqual<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) == value);
		}



		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return WhereNotEqual(collection, property, (int)value);
				case nameof(Guid):
					return WhereNotEqual(collection, property, (Guid)value);
				case nameof(DateTime):
					return WhereNotEqual(collection, property, (DateTime)value);
				case nameof(Double):
					return WhereNotEqual(collection, property, (double)value);
				case nameof(Boolean):
					return WhereNotEqual(collection, property, (bool)value);
				case nameof(Int64):
					return WhereNotEqual(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) != value);
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => (Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity) != value);
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => !((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)).Equals(value));
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) != value);
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => (bool)typeof(TEntity).GetProperty(property)?.GetValue(entity) != value);
		}

		public static IEnumerable<TEntity> WhereNotEqual<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) != value);
		}




		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return GreaterThan(collection, property, (int)value);
				case nameof(Guid):
					return GreaterThan(collection, property, (Guid)value);
				case nameof(DateTime):
					return GreaterThan(collection, property, (DateTime)value);
				case nameof(Double):
					return GreaterThan(collection, property, (double)value);
				case nameof(Boolean):
					return GreaterThan(collection, property, (bool)value);
				case nameof(Int64):
					return GreaterThan(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) > value);
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => ((Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) > 0);
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => ((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)) > (value));
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) > value);
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => ((bool)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) > 0);
		}

		public static IEnumerable<TEntity> GreaterThan<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) > value);
		}





		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return LessThan(collection, property, (int)value);
				case nameof(Guid):
					return LessThan(collection, property, (Guid)value);
				case nameof(DateTime):
					return LessThan(collection, property, (DateTime)value);
				case nameof(Double):
					return LessThan(collection, property, (double)value);
				case nameof(Boolean):
					return LessThan(collection, property, (bool)value);
				case nameof(Int64):
					return LessThan(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) < value);
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => ((Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) < 0);
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => ((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)) < (value));
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) < value);
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => ((bool)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) < 0);
		}

		public static IEnumerable<TEntity> LessThan<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) < value);
		}








		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return GreaterThanOrEqual(collection, property, (int)value);
				case nameof(Guid):
					return GreaterThanOrEqual(collection, property, (Guid)value);
				case nameof(DateTime):
					return GreaterThanOrEqual(collection, property, (DateTime)value);
				case nameof(Double):
					return GreaterThanOrEqual(collection, property, (double)value);
				case nameof(Boolean):
					return GreaterThanOrEqual(collection, property, (bool)value);
				case nameof(Int64):
					return GreaterThanOrEqual(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity, T>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) >= value);
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => ((Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) >= 0);
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => ((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)) >= (value));
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) >= value);
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => ((bool)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) >= 0);
		}

		public static IEnumerable<TEntity> GreaterThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) >= value);
		}




		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			var typeName = typeof(TEntity)?.GetProperty(property)?.PropertyType.Name;
			switch (typeName)
			{
				case nameof(Int32):
					return LessThanOrEqual(collection, property, (int)value);
				case nameof(Guid):
					return LessThanOrEqual(collection, property, (Guid)value);
				case nameof(DateTime):
					return LessThanOrEqual(collection, property, (DateTime)value);
				case nameof(Double):
					return LessThanOrEqual(collection, property, (double)value);
				case nameof(Boolean):
					return LessThanOrEqual(collection, property, (bool)value);
				case nameof(Int64):
					return LessThanOrEqual(collection, property, (long)value);
				default:
					throw new InvalidOperationException($"No support for type {typeName}.");
			}
			throw new ArgumentException($"No equality operator for {typeof(TEntity).GetProperty(property).PropertyType}.");
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, int value)
		{
			return collection.Where(entity => (int)typeof(TEntity).GetProperty(property)?.GetValue(entity) <= value);
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, Guid value)
		{
			return collection.Where(entity => ((Guid)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) <= 0);
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, DateTime value)
		{
			return collection.Where(entity => ((DateTime)typeof(TEntity).GetProperty(property)?.GetValue(entity)) <= (value));
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, double value)
		{
			return collection.Where(entity => (double)typeof(TEntity).GetProperty(property)?.GetValue(entity) <= value);
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, bool value)
		{
			return collection.Where(entity => ((bool)typeof(TEntity).GetProperty(property)?.GetValue(entity)).CompareTo(value) <= 0);
		}

		public static IEnumerable<TEntity> LessThanOrEqual<TEntity>(IEnumerable<TEntity> collection, string property, long value)
		{
			return collection.Where(entity => (long)typeof(TEntity).GetProperty(property)?.GetValue(entity) <= value);
		}




		public static IEnumerable<TEntity> Matches<TEntity>(IEnumerable<TEntity> collection, string property, object value)
		{
			return collection.Where(entity => 
			{
				var current = typeof(TEntity).GetProperty(property)?.GetValue(entity);
				if (current != null && value != null)
				{
					var match = Regex.IsMatch(current.ToString(), value.ToString());
					return match;
				}
				return value == current; //current was null, so value must be null as well.
			});
		}
	}

	
}
