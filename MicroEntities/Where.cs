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
		public static Where Match(string property, object value)
		{
			return new Where(property, Operator.Matches, value);
		}
	}

	
}
