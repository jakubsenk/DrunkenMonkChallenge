using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DrunkenMonk
{
	public static class Drawer
	{
		private const int MF_BYCOMMAND = 0x00000000;
		private const int SC_CLOSE = 0xF060;
		private const int SC_MINIMIZE = 0xF020;
		private const int SC_MAXIMIZE = 0xF030;
		private const int SC_SIZE = 0xF000;

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
		[DllImport("user32.dll")]
		private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

		[DllImport("user32.dll")]
		private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GetConsoleWindow();

		public static void DisableResize()
		{
			Console.BufferWidth = Console.WindowWidth;
			Console.BufferHeight = Console.WindowHeight;
			IntPtr handle = GetConsoleWindow();
			IntPtr sysMenu = GetSystemMenu(handle, false);


			if (handle != IntPtr.Zero)
			{
				DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
				DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
			}
		}
		internal static class DllImports
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct COORD
			{

				public short X;
				public short Y;
				public COORD(short x, short y)
				{
					this.X = x;
					this.Y = y;
				}
			}
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetStdHandle(int handle);
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool SetConsoleDisplayMode(
					IntPtr ConsoleOutput
					, uint Flags
					, out COORD NewScreenBufferDimensions
					);
		}
		public static void ToFullscreen()
		{
			IntPtr hConsole = DllImports.GetStdHandle(-11);   // get console handle
			DllImports.COORD xy = new DllImports.COORD(100, 100);
			DllImports.SetConsoleDisplayMode(hConsole, 1, out xy);
		}

		public static void DrawAllCharacters(List<Point> persons)
		{
			foreach (Point p in persons)
			{
				DrawCharacter(p.X, p.Y, Direction.Unchanged, 'B', ConsoleColor.Blue);
			}
		}

		public static void DrawMenu()
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Clear();
			Console.WriteLine("Welcome to Drunken Monk");
			Console.WriteLine("Select difficulty:");
			Console.WriteLine("0 - Let me have some drink, sir");
			Console.WriteLine("1 - Let me have some drink, sir (fullscreen)");
			Console.WriteLine("2 - Alcoholic (professional)");
			Console.WriteLine("3 - Alcoholic (professional) (fullscreen)");
			Console.WriteLine("4 - I alcohol therefor I am");
			Console.WriteLine("5 - I alcohol therefor I am (fullscreen)");
			Console.WriteLine("6 - Let me define my own difficulty");
			Console.WriteLine("7 - Let me define my own difficulty (fullscreen)");
		}

		public static void HighLightPerson(Point person, ConsoleColor color = ConsoleColor.Yellow, ConsoleColor originalColor = ConsoleColor.Blue, char displayChar = 'B')
		{
			Console.SetCursorPosition(person.X, person.Y);
			Console.ForegroundColor = color;
			Console.Write(displayChar);
			Thread.Sleep(350);
			Console.CursorLeft--;
			Console.ForegroundColor = originalColor;
			Console.Write(displayChar);
		}

		public static void DrawPathToHome(List<List<Tile>> map)
		{
			int goalX = Console.WindowWidth - 2;
			int goalY = Console.WindowHeight - 3;
			foreach (List<Tile> tl in map)
			{
				foreach (Tile t in tl)
				{
					if (t.X == goalX && t.Y == goalY)
					{
						Console.SetCursorPosition(t.X, t.Y);
						Console.BackgroundColor = ConsoleColor.Green;
						Console.Write(" ");
					}
					if (t.Type == TileType.Empty)
					{
						Thread.Sleep(15);
						Console.SetCursorPosition(t.X, t.Y);
						Console.BackgroundColor = ConsoleColor.DarkGreen;
						Console.Write(" ");
					}
				}
			}
			foreach (List<Tile> tl in map)
			{
				foreach (Tile t in tl)
				{
					if (t.Type == TileType.Empty)
					{
						Thread.Sleep(15);
						Console.SetCursorPosition(t.X, t.Y);
						Console.BackgroundColor = ConsoleColor.Black;
						Console.Write(" ");
					}
				}
			}
		}
		public static void DrawGameBorder(bool setDefaultColor = true)
		{
			if (setDefaultColor)
				Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.SetCursorPosition(0, 0);
			Console.Write("╔");
			while (Console.CursorLeft < Console.WindowWidth - 15)
			{
				Console.Write("═");
			}
			Console.CursorTop = 1;
			while (Console.CursorTop < Console.WindowHeight - 2)
			{
				Console.CursorLeft = 0;
				Console.Write("║");
				Console.CursorLeft = Console.WindowWidth - 1;
				Console.Write("║");
			}
			Console.CursorLeft = 0;
			Console.Write("╚");
			while (Console.CursorLeft < Console.WindowWidth - 1)
			{
				Console.Write("═");
			}
			Console.Write("╝");
			DrawCollision();
		}

		public static void FlashBorder()
		{
			Console.ForegroundColor = ConsoleColor.Red;
			DrawGameBorder(false);
			Thread.Sleep(100);
			DrawGameBorder();
		}

		private static int lastCol;
		public static void DrawCollision(int col = -1)
		{
			if (col == -1) col = lastCol;
			Console.SetCursorPosition(Console.WindowWidth - 14, 0);
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write("Collisions: " + col);
			lastCol = col;
		}

		public static void DrawCharacter(int x, int y, Direction dir, char c = 'H', ConsoleColor charColor = ConsoleColor.White)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = charColor;
			Console.Write(c);
			Console.ForegroundColor = ConsoleColor.Black;
			switch (dir)
			{
				case Direction.Left:
					Console.SetCursorPosition(x + 1, y);
					Console.Write(" ");
					break;
				case Direction.Right:
					Console.SetCursorPosition(x - 1, y);
					Console.Write(" ");
					break;
				case Direction.Up:
					Console.SetCursorPosition(x, y + 1);
					Console.Write(" ");
					break;
				case Direction.Down:
					Console.SetCursorPosition(x, y - 1);
					Console.Write(" ");
					break;
				default:
					break;
			}
		}
	}
}
