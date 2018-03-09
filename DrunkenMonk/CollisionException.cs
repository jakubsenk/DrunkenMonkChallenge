using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrunkenMonk
{
	public class CollisionException : Exception
	{
		public Point Person { get; set; }
		public CollisionException(Point person)
		{
			Person = person;
		}
	}
}
