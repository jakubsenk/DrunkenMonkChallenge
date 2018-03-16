using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrunkenMonk
{
	public enum Direction
	{
		Left, Right, Up, Down, Unchanged
	}
	public struct Point
	{
		public int X, Y;
	}
	class Program
	{
		static void Main(string[] args)
		{
			int diff;
			int tripChance = 25;
			int personSpawnChance = 30;
			int maxColls = 4;
			int pushChance = 20;

			Drawer.DrawMenu();
			diff = Game.LoadDifficulty();
			if (diff == 6 || diff == 7)
			{
				Game.LoadCustomDifficulty(out tripChance, out personSpawnChance, out maxColls, out pushChance);
			}

			Console.Clear();
			Drawer.DisableResize();
			if (diff % 2 != 0) Drawer.ToFullscreen();
			Drawer.DrawGameBorder();

			Console.CursorVisible = false;
			Console.SetCursorPosition(1, 1);
			Console.WriteLine("Loading...");

			int x = 1, y = 1;
			int collisions = 0;
			List<Point> persons = new List<Point>();
			List<List<Tile>> map = new List<List<Tile>>();

			Game.DeployPersonsToWinnablePositions(out persons, diff, out map, personSpawnChance);

			Console.Clear();
			Drawer.DrawCollision(0);
			Drawer.DrawGameBorder();

			Direction dir = Direction.Up;
			Drawer.DrawCharacter(x, y, dir, 'H', ConsoleColor.White);
			Drawer.DrawAllCharacters(persons);

			Drawer.DrawPathToHome(map);
			Console.BackgroundColor = ConsoleColor.Black;
			while (true)
			{
				while (Console.KeyAvailable)
				{
					Console.ReadKey(true);
				}
				ConsoleKeyInfo c = Console.ReadKey(true);

				if (c.Key == ConsoleKey.Spacebar)
				{
					map = PathFinder.FindPath(x, y);
					Drawer.DrawPathToHome(map, 5);
					continue;
				}

				bool success = false;
				bool personPush = false;
				while (!success)
				{
					try
					{
						if (personPush)
						{
							Game.PersonPushPlayer(ref x, ref y, ref dir, persons);
							personPush = false;
						}
						else
						{
							Game.MakeStep(c, ref x, ref y, ref dir, persons);
							Game.Trip(ref x, ref y, dir, persons, tripChance);
						}
						success = true;
					}
					catch (CollisionException ex)
					{
						int steps = -1;
						if (Game.PersonPush(ex.Person, ref steps, pushChance))
						{
							Drawer.HighLightPerson(ex.Person, ConsoleColor.Red);
							personPush = true;
						}
						else
						{
							success = true;
							dir = Direction.Unchanged;
						}
						Drawer.DrawCollision(++collisions);
					}
				}

				if (collisions > maxColls) break;
				Drawer.DrawCharacter(x, y, dir, 'H', ConsoleColor.White);

				if (Game.DetectWin(x, y)) break;
			}



			Console.SetCursorPosition(1, 1);
			if (collisions > maxColls)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Too many collisions, better luck next time.");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Congratulation, you did it!");
			}
			Thread.Sleep(1000);
			while (Console.KeyAvailable)
			{
				Console.ReadKey(true);
			}
			Console.ReadKey();
			Console.ResetColor();
			Console.Clear();
		}
	}
}
