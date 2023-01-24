namespace MicroEntities.Attributes
{
	public class Identity : Attribute
	{
		public Identity(int increment = 1, int seed = 1)
		{
			Increment = increment;
			Seed = seed;	
		}

		public int Increment { get; protected set; }
		public int Seed { get; protected set; }
	}
}
