using MicroEntities.Attributes;
using System.Text.Json.Serialization;

namespace TestBed
{
	public class Item
	{
		public enum ItemType
		{
			Deoderant,
			Douche,
			PotatoChips,
			CandyBar,
			MakeUp
		}

		public ItemType Type { get; set; }

		[ReadOnly]
		[Identity]
		public int Id { get; set; }

		public string Name { get; set; }

		[ReadOnly]
		[Default(Default.Func.NewGuid)]
		public Guid Key { get; set; }

		public decimal Price { get; set; }


	}

	public class ItemDto
	{
		public enum ItemType
		{
			Deoderant,
			Douche,
			PotatoChips,
			CandyBar,
			MakeUp
		}
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public ItemType Type { get; set; }
		public string Name { get; set; }
		public Guid Key { get; set; }
		public decimal Price { get; set; }
	}

}
