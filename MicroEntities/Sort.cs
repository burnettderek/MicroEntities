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
	}
}
