using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

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

		public static IEnumerable<T> Paginate<T>(IEnumerable<T> entities, Page page)
		{
			return entities.Skip(page.Number * page.Size).Take(page.Size);
		}
	}

	
}
