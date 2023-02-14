using AutoMapper.Internal;

namespace MicroEntities
{
	public class Sort
	{
		public enum SortDirection
		{
			Ascending,
			Descending
		}
		public Sort(string property, SortDirection direction)
		{
			Property = property;
			Direction = direction;
		}
		public string Property { get; protected set; }
		public SortDirection Direction { get; protected set; }	
		public static Sort Ascending(string property)
		{
			return new Sort(property, SortDirection.Ascending);
		}
		public static Sort Descending(string property)
		{
			return new Sort(property, SortDirection.Descending);
		}

		public static IEnumerable<T> SortCollection<T>(IEnumerable<T> collection, Sort sort)
		{
			var prop = typeof(T).GetProperty(sort.Property);
			var type = prop.PropertyType;
			if(type.IsEnum)
				type = Enum.GetUnderlyingType(type);
			if (sort.Direction == SortDirection.Ascending)
			{
				if (type.IsNullableType())
				{
					type = Nullable.GetUnderlyingType(type);
					if (type == typeof(string))
						return collection.OrderBy(a => (string?)prop.GetValue(a));
					if (type == typeof(int))
						return collection.OrderBy(a => (int?)prop.GetValue(a));
					if (type == typeof(double))
						return collection.OrderBy(a => (double?)prop.GetValue(a));
					if (type == typeof(DateTime))
						return collection.OrderBy(a => (DateTime?)prop.GetValue(a));
					if (type == typeof(bool))
						return collection.OrderBy(a => (bool?)prop.GetValue(a));
					if (type == typeof(decimal))
						return collection.OrderBy(a => (decimal?)prop.GetValue(a));
				}
				if (!type.IsNullableType())
				{
					if (type == typeof(string))
						return collection.OrderBy(a => (string)prop.GetValue(a));
					if (type == typeof(int))
						return collection.OrderBy(a => (int)prop.GetValue(a));
					if (type == typeof(double))
						return collection.OrderBy(a => (double)prop.GetValue(a));
					if (type == typeof(DateTime))
						return collection.OrderBy(a => (DateTime)prop.GetValue(a));
					if (type == typeof(bool))
						return collection.OrderBy(a => (bool)prop.GetValue(a));
					if (type == typeof(decimal))
						return collection.OrderBy(a => (decimal)prop.GetValue(a));
				}
			}
			if(sort.Direction == SortDirection.Descending)
			{
				if (type.IsNullableType())
				{
					type = Nullable.GetUnderlyingType(type);
					if (type == typeof(string))
						return collection.OrderByDescending(a => (string?)prop.GetValue(a));
					if (type == typeof(int))
						return collection.OrderByDescending(a => (int?)prop.GetValue(a));
					if (type == typeof(double))
						return collection.OrderByDescending(a => (double?)prop.GetValue(a));
					if (type == typeof(DateTime))
						return collection.OrderByDescending(a => (DateTime?)prop.GetValue(a));
					if (type == typeof(bool))
						return collection.OrderByDescending(a => (bool?)prop.GetValue(a));
					if (type == typeof(decimal))
						return collection.OrderByDescending(a => (decimal?)prop.GetValue(a));
				}
				if (!type.IsNullableType())
				{
					if (type == typeof(string))
						return collection.OrderByDescending(a => (string)prop.GetValue(a));
					if (type == typeof(int))
						return collection.OrderByDescending(a => (int)prop.GetValue(a));
					if (type == typeof(double))
						return collection.OrderByDescending(a => (double)prop.GetValue(a));
					if (type == typeof(DateTime))
						return collection.OrderByDescending(a => (DateTime)prop.GetValue(a));
					if (type == typeof(bool))
						return collection.OrderByDescending(a => (bool)prop.GetValue(a));
					if (type == typeof(decimal))
						return collection.OrderByDescending(a => (decimal)prop.GetValue(a));
				}
			}

			throw new ArgumentException($"Type {type} is not supported.");
		}

		
	}
}
