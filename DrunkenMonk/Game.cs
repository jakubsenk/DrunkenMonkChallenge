using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
	public static class Game
	{
		public static bool Bounced(int x, int y, Direction dir)
		{
			switch (dir)
			{
				case Direction.Left:
					if (x == 1)
						return true;
					break;
				case Direction.Right:
					if (x == Console.WindowWidth - 2)
						return true;
					break;
				case Direction.Up:
					if (y == 1)
						return true;
					break;
				case Direction.Down:
					if (y == Console.WindowHeight - 3)
						return true;
					break;
			}
			return false;
		}

		public static bool DetectWin(int x, int y)
		{
			if (y == Console.WindowHeight - 3 && x == Console.WindowWidth - 2) return true;
			return false;
		}
		private static List<List<Tile>> Field;
		private static void LoadMap(List<Point> persons)
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
		private static Queue<Tile> queue;
		private static Queue<int> lengths;
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

		public static int LoadDifficulty()
		{
			int diff;
			while (true)
			{
				try
				{
					diff = int.Parse(Console.ReadLine());
					if (diff < 0 || diff > 7) throw new Exception();
					break;
				}
				catch
				{
					Console.WriteLine("Enter diffuculty in range 0 - 7");
				}
			}
			return diff;
		}

		public static void LoadCustomDifficulty(out int tripChance, out int personSpawnChance, out int maxColls, out int pushChance)
		{
			Console.Clear();
			Console.Write("Enter trip chance [0;100]: ");
			tripChance = int.Parse(Console.ReadLine());
			if (tripChance < 0 || tripChance > 100)
			{
				tripChance = 25;
				Console.WriteLine("Invalid trip chance... set to default.");
			}
			Console.WriteLine("Enter person spawn chance.");
			Console.Write("Please note that too high chance may result in unwinnable game and long loading [0;80]: ");
			personSpawnChance = int.Parse(Console.ReadLine());
			if (personSpawnChance < 0 || personSpawnChance > 80)
			{
				personSpawnChance = 30;
				Console.WriteLine("Invalid value... set to default");
			}
			Console.Write("Enter maximum collisions: ");
			maxColls = int.Parse(Console.ReadLine());
			if (maxColls < 1)
			{
				maxColls = 4;
				Console.WriteLine("Invalid value... set to default");
			}
			Console.WriteLine("Enter chance that person will push you back on collision.");
			Console.Write("Please note that too high chance may result that persons will play ping-pong with you [0;95]: ");
			pushChance = int.Parse(Console.ReadLine());
			if (pushChance < 0 || pushChance > 95)
			{
				pushChance = 20;
				Console.WriteLine("Invalid value... set to default");
			}
		}

		public static void DeployPersonsToWinnablePositions(out List<Point> persons, int diff, out List<List<Tile>> map, int personSpawnChance)
		{
			bool s = false;
			map = new List<List<Tile>>();
			persons = new List<Point>();
			while (!s)
			{
				try
				{
					persons = GeneratePersons(diff, out map, personSpawnChance);
					s = true;
				}
				catch
				{ }
			}
		}
		public static List<Point> GeneratePersons(int diff, out List<List<Tile>> map, int spawnChance = 0)
		{
			List<Point> persons = new List<Point>();
			Random r = new Random();
			int personGenerated = r.Next(0, 101);
			int maxPersons = (Console.WindowHeight - 4) * (Console.WindowWidth - 3);
			int randAmount;
			switch (diff)
			{
				case 0:
				case 1:
					randAmount = r.Next(25, 36);
					break;
				case 2:
				case 3:
					randAmount = r.Next(36, 46);
					break;
				case 4:
				case 5:
					randAmount = r.Next(46, 55);
					break;
				default:
					randAmount = spawnChance;
					break;
			}
			while (maxPersons > 0)
			{
				maxPersons--;
				if (personGenerated < randAmount)
				{
					Point p = new Point();
					p.X = r.Next(1, Console.WindowWidth - 2);
					p.Y = r.Next(1, Console.WindowHeight - 2);
					if ((p.X == 1 || p.X == 2 || p.X == 3) && (p.Y == 1 || p.Y == 2 || p.Y == 3) || (p.X == Console.WindowWidth - 2 || p.X == Console.WindowWidth - 3 || p.X == Console.WindowWidth - 4) && (p.Y == Console.WindowHeight - 2 || p.Y == Console.WindowHeight - 3 || p.Y == Console.WindowHeight - 4))
						continue;
					persons.Add(p);
				}
				personGenerated = r.Next(0, 101);
			}
			LoadMap(persons);
			map = FindPath();
			return persons;
		}
		public static void Trip(ref int x, ref int y, Direction dir, List<Point> persons, int trip)
		{
			Random r = new Random();
			int tripChance = r.Next(0, 101);
			if (tripChance <= trip)
			{
				int steps = r.Next(1, 4);
				while (steps > 0)
				{
					Drawer.DrawCharacter(x, y, dir, 'H', ConsoleColor.DarkRed);
					if (!FlashIfBounced(x, y, dir, ref dir))
					{
						switch (dir)
						{
							case Direction.Left:
								if (!CheckCollision(x, y, dir, persons))
								{
									if (!FlashIfBounced(x, y, Direction.Left, ref dir))
										x--;
								}
								else
								{
									Drawer.HighLightPerson(new Point() { X = x - 1, Y = y });
									throw new CollisionException(new Point() { X = x - 1, Y = y });
								}
								break;
							case Direction.Right:
								if (!CheckCollision(x, y, dir, persons))
								{
									if (!FlashIfBounced(x, y, Direction.Right, ref dir))
										x++;
								}
								else
								{
									Drawer.HighLightPerson(new Point() { X = x + 1, Y = y });
									throw new CollisionException(new Point() { X = x + 1, Y = y });
								}
								break;
							case Direction.Up:
								if (!CheckCollision(x, y, dir, persons))
								{
									if (!FlashIfBounced(x, y, Direction.Up, ref dir))
										y--;
								}
								else
								{
									Drawer.HighLightPerson(new Point() { X = x, Y = y - 1 });
									throw new CollisionException(new Point() { X = x, Y = y - 1 });
								}
								break;
							case Direction.Down:
								if (!CheckCollision(x, y, dir, persons))
								{
									if (!FlashIfBounced(x, y, Direction.Down, ref dir))
										y++;
								}
								else
								{
									Drawer.HighLightPerson(new Point() { X = x, Y = y + 1 });
									throw new CollisionException(new Point() { X = x, Y = y + 1 });
								}
								break;
							default:
								break;
						}
						Thread.Sleep(200);
						steps--;
					}
				}
			}
		}

		public static void PersonPushPlayer(ref int x, ref int y, ref Direction dir, List<Point> persons)
		{
			ConsoleKeyInfo c = new ConsoleKeyInfo();
			switch (dir)
			{
				case Direction.Left:
					c = new ConsoleKeyInfo('a', ConsoleKey.RightArrow, false, false, false);
					break;
				case Direction.Right:
					c = new ConsoleKeyInfo('a', ConsoleKey.LeftArrow, false, false, false);
					break;
				case Direction.Up:
					c = new ConsoleKeyInfo('a', ConsoleKey.DownArrow, false, false, false);
					break;
				case Direction.Down:
					c = new ConsoleKeyInfo('a', ConsoleKey.UpArrow, false, false, false);
					break;
			}
			Random r = new Random();
			int steps = r.Next(2, 4);
			while (steps > 0)
			{
				steps--;
				MakeStep(c, ref x, ref y, ref dir, persons);
				Drawer.DrawCharacter(x, y, dir);
				Thread.Sleep(200);
			}
		}

		public static bool CheckCollision(int x, int y, Direction dir, List<Point> persons)
		{
			switch (dir)
			{
				case Direction.Left:
					if (persons.Contains(new Point() { X = x - 1, Y = y }))
						return true;
					break;
				case Direction.Right:
					if (persons.Contains(new Point() { X = x + 1, Y = y }))
						return true;
					break;
				case Direction.Up:
					if (persons.Contains(new Point() { X = x, Y = y - 1 }))
						return true;
					break;
				case Direction.Down:
					if (persons.Contains(new Point() { X = x, Y = y + 1 }))
						return true;
					break;
			}
			return false;
		}

		public static bool PersonPush(Point p, ref int steps, int pushChance)
		{
			Random r = new Random();
			if (r.Next(0, 101) < pushChance)
			{
				steps = r.Next(2, 4);
				return true;
			}
			return false;
		}


		public static void MakeStep(ConsoleKeyInfo c, ref int x, ref int y, ref Direction dir, List<Point> persons)
		{
			switch (c.Key)
			{
				case ConsoleKey.LeftArrow:
					if (!FlashIfBounced(x, y, Direction.Left, ref dir))
					{
						if (!CheckCollision(x, y, dir, persons))
							x--;
						else
						{
							Drawer.HighLightPerson(new Point() { X = x - 1, Y = y });
							throw new CollisionException(new Point() { X = x - 1, Y = y });
						}
					}
					break;
				case ConsoleKey.RightArrow:
					if (!FlashIfBounced(x, y, Direction.Right, ref dir))
					{
						if (!CheckCollision(x, y, dir, persons))
							x++;
						else
						{
							Drawer.HighLightPerson(new Point() { X = x + 1, Y = y });
							throw new CollisionException(new Point() { X = x + 1, Y = y });
						}
					}
					break;
				case ConsoleKey.DownArrow:
					if (!FlashIfBounced(x, y, Direction.Down, ref dir))
					{
						if (!CheckCollision(x, y, dir, persons))
							y++;
						else
						{
							Drawer.HighLightPerson(new Point() { X = x, Y = y + 1 });
							throw new CollisionException(new Point() { X = x, Y = y + 1 });
						}
					}
					break;
				case ConsoleKey.UpArrow:
					if (!FlashIfBounced(x, y, Direction.Up, ref dir))
					{
						if (!CheckCollision(x, y, dir, persons))
							y--;
						else
						{
							Drawer.HighLightPerson(new Point() { X = x, Y = y - 1 });
							throw new CollisionException(new Point() { X = x, Y = y - 1 });
						}
					}
					break;
				default:
					break;
			}
		}

		public static bool FlashIfBounced(int x, int y, Direction dir, ref Direction setDir)
		{
			if (Bounced(x, y, dir))
			{
				Drawer.FlashBorder();
				setDir = Direction.Unchanged;
				return true;
			}
			setDir = dir;
			return false;
		}
	}
}
