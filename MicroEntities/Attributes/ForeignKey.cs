namespace MicroEntities.Attributes
{
	public class ForeignKey : Attribute
	{
		public ForeignKey(string tableName, string fieldName)
		{
			TableName = tableName;
			FieldName = fieldName;	
		}

		public string TableName { get; protected set; }
		public string FieldName { get; protected set; }
	}
}
