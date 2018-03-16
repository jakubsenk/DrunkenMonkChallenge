using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrunkenMonk
{
	public static class PathFinder
	{
		private static List<List<Tile>> Field;
		private static Queue<Tile> queue;
		private static Queue<int> lengths;

		public static void LoadMap(List<Point> persons)
		{
			Field = new List<List<Tile>>();
			int x = 0, y = 0;
			for (int i = 0; i < Console.WindowWidth; i++)
			{
				x = 0;
				List<Tile> tl = new List<Tile>();
				for (int j = 0; j < Console.WindowHeight - 1; j++)
				{
					Tile t = new Tile();
					t.X = y;
					t.Y = x;
					t.Type = (j == 0 || j == Console.WindowHeight - 2 || i == 0 || i == Console.WindowWidth - 1) ? TileType.Person : TileType.Empty;
					tl.Add(t);
					x++;
				}
				Field.Add(tl);
				y++;
			}

			foreach (Point p in persons)
			{
				foreach (List<Tile> tl in Field)
				{
					bool b = false;
					foreach (Tile t in tl)
					{
						if (t.X == p.X && t.Y == p.Y)
						{
							t.Type = TileType.Person;
							b = true;
							break;
						}
					}
					if (b) break;
				}
			}
		}

		public static List<List<Tile>> FindPath(int x = -1, int y = -1)
		{
			List<List<Tile>> map = new List<List<Tile>>();
			List<List<int>> list = new List<List<int>>();
			int startX = 0, startY = 0;
			foreach (var item in Field)
			{
				List<int> l = new List<int>();
				List<Tile> tl = new List<Tile>();
				foreach (var i in item)
				{
					l.Add(int.MaxValue);
					tl.Add(new Tile() { X = i.X, Y = i.Y, Type = TileType.Person });
				}
				list.Add(l);
				map.Add(tl);
			}
			foreach (List<Tile> row in Field)
			{
				foreach (Tile t in row)
				{
					if (t.X == Console.WindowWidth - 3 && t.Y == Console.WindowHeight - 3)
					{
						queue = new Queue<Tile>();
						lengths = new Queue<int>();
						queue.Enqueue(t);
						lengths.Enqueue(0);
						if (!WaveFill(list))
						{
							throw new Exception("Not found");
						}
					}
					if (t.X == 1 && t.Y == 1)
					{
						startX = t.X;
						startY = t.Y;
					}
				}
			}
			if (x != -1 && y != -1)
			{
				startX = x;
				startY = y;
			}
			FillMap(map, list, startX, startY);
			return map;
		}
		private static void FillMap(List<List<Tile>> map, List<List<int>> data, int startX, int startY)
		{
			int i = data[startX][startY];
			try
			{
				if (data[startX + 1][startY] < i && map[startX + 1][startY].Type == TileType.Person)
				{
					map[startX + 1][startY].Type = TileType.Empty;
					FillMap(map, data, startX + 1, startY);
					return;
				}
			}
			catch { }
			try
			{
				if (data[startX - 1][startY] < i && map[startX - 1][startY].Type == TileType.Person)
				{
					map[startX - 1][startY].Type = TileType.Empty;
					FillMap(map, data, startX - 1, startY);
					return;
				}
			}
			catch { }
			try
			{
				if (data[startX][startY + 1] < i && map[startX][startY + 1].Type == TileType.Person)
				{
					map[startX][startY + 1].Type = TileType.Empty;
					FillMap(map, data, startX, startY + 1);
					return;
				}
			}
			catch { }
			try
			{
				if (data[startX][startY - 1] < i && map[startX][startY - 1].Type == TileType.Person)
				{
					map[startX][startY - 1].Type = TileType.Empty;
					FillMap(map, data, startX, startY - 1);
					return;
				}
			}
			catch { }
		}
		private static bool WaveFill(List<List<int>> data)
		{
			while (queue.Count > 0)
			{
				try
				{
					Tile t = queue.Dequeue();
					if (t.X == 1 && t.Y == 1) return true;
					int len = lengths.Dequeue();
					int clen = data[t.X][t.Y];
					if (data[t.X][t.Y] > len && (t.Type == TileType.Empty))
					{
						data[t.X][t.Y] = len;
						try
						{
							queue.Enqueue(Field[t.X][t.Y + 1]);
							lengths.Enqueue(len + 1);
						}
						catch { }
						try
						{
							queue.Enqueue(Field[t.X][t.Y - 1]);
							lengths.Enqueue(len + 1);
						}
						catch { }
						try
						{
							queue.Enqueue(Field[t.X - 1][t.Y]);
							lengths.Enqueue(len + 1);
						}
						catch { }
						try
						{
							queue.Enqueue(Field[t.X + 1][t.Y]);
							lengths.Enqueue(len + 1);
						}
						catch { }
					}
				}
				catch { }
			}
			return false;
		}

	}
}
