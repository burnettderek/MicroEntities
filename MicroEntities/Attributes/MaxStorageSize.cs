namespace MicroEntities.Attributes
{
	public class MaxStorageSize : Attribute
	{
		public MaxStorageSize(int sizeInBytes)
		{
			SizeInBytes = sizeInBytes;
		}

		public int SizeInBytes { get; protected set; }
	}
}
