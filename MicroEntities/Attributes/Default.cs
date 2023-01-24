namespace MicroEntities.Attributes
{
	public class Default : Attribute
	{
		public Default(object defaultValue)
		{
			Value = defaultValue;
		}

		public object Value { get; set; }

		public enum Func
		{
			TimeStamp,
			TimeStampUtc,
			NewGuid
		}
	}
}
