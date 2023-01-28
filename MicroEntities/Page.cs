using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MicroEntities
{
	public class Page
	{
		public Page(int number, int size)
		{
			if (number < 0) throw new ArgumentException("Page numbering cannot be less than 0.");
			Size = size;
			Number = number;
		}

		public int Size { get; set; }
		public int Number { get; set; }
	}
}
