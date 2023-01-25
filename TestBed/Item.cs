using MicroEntities.Attributes;

namespace TestBed
{
	public class Item
	{
		[ReadOnly]
		[Identity]
		public int Id { get; set; }

		public string Name { get; set; }

		[ReadOnly]
		[Default(Default.Func.NewGuid)]
		public Guid Key { get; set; }

		public decimal Price { get; set; }
	}


}
