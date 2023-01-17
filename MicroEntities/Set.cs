namespace MicroEntities
{
	public class Set
	{
		public string Property { get; protected set; }
		public object PropertyValue { get; protected set; }
		public Set SubSet { get; set; }

		public Set(string property, object value)
		{
			Property = property;
			PropertyValue = value;
		}


		public static Set Value(string property, object value)
		{
			return new Set(property, value);
		}

		public Set And(string property, object value)
		{
			Set? finalSet = this;
			if (finalSet != null)
			{
				while (finalSet.SubSet != null)
				{
					finalSet = finalSet.SubSet;
				}
			}
			finalSet.SubSet = new Set(property, value);
			return this;
		}


	}
}
