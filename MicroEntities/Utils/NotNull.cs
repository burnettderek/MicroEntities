namespace MicroEntities.Utils
{
	public class NotNull
	{
		public static void Check(string name, int? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}

		public static void Check(string name, double? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}

		public static void Check(string name, float? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}

		public static void Check(string name, decimal? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}

		public static void Check(string name, DateTime? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}
		public static void Check(string name, string? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
		}

		public static void Check<T>(string name, T? x)
		{
			if (x == null) throw new ArgumentException($"'{name}' cannot be null");
			var properties = x.GetType().GetProperties();
			foreach(var property in properties)
			{
				var value = property.GetValue(x);
				if (value == null) throw new ArgumentException($"The '{property.Name}' field on '{name}', cannot be null.");
			}
		}
	}
}
