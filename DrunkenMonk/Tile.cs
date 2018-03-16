using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrunkenMonk
{
	public enum TileType
	{
		Empty, Person
	}
	public class Tile
		{
			public int X { get; set; }
			public int Y { get; set; }
			public TileType Type { get; set; }
		}
	}