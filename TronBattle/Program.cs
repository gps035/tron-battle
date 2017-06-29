using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
enum Directions
{
	UP, DOWN, LEFT, RIGHT
}

struct Position
{
	public Position(int x, int y)
	{
		X = x;
		Y = y;
	}

	public int X { get; set; }
	public int Y { get; set; }

	public Position NextPosition(Directions direction)
	{
		switch (direction)
		{
			case Directions.UP:
				return new Position(X, Y - 1);
			case Directions.RIGHT:
				return new Position(X + 1, Y);
			case Directions.DOWN:
				return new Position(X, Y + 1);
			default:
				return new Position(X - 1, Y);
		}
	}
	public override string ToString()
	{
		return $"{{X:{X}, Y:{Y}}}";
	}
}

class Player
{
	static int numberOfPlayers = 0;
	static int myNumber = 0;


	const int XSize = 30;
	const int YSize = 20;
	private static bool[,] cells = new bool[XSize, YSize];

	static Position currentPosition = new Position(0, 0);

	static Directions currentDirection = Directions.UP;
	static bool start = true;
	static void Main(string[] args)
	{
		// game loop
		while (true)
		{
			var playerInputs = Console.ReadLine().Split(' ');
			numberOfPlayers = int.Parse(playerInputs[0]); // total number of players (2 to 4).
			myNumber = int.Parse(playerInputs[1]); // your player number (0 to 3).
			Console.Error.WriteLine("Getting positions...");
			for (int i = 0; i < numberOfPlayers; i++)
			{
				var positionInputs = Console.ReadLine().Split(' ');
				int xNew = int.Parse(positionInputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
				int yNew = int.Parse(positionInputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
				cells[xNew, yNew] = true;
				var position = new Position(xNew, yNew);

				if (i == myNumber)
				{
					currentPosition = position;
					Console.Error.WriteLine("Me");
				}
				else
				{
					Console.Error.WriteLine($"Player {i}");
				}
				Console.Error.WriteLine(position);
			}
			if (start)
			{
				start = false;
				currentDirection = SetBestStartDirection();
				AnnounceDirection(currentDirection);
				continue;
			}
			var nextDirection = currentDirection;
			var ahead = currentPosition.NextPosition(nextDirection);
			Console.Error.WriteLine($"Ahead {ahead}");
			if (WallAtPosition(ahead) || TrailAtPosition(ahead))
			{
				nextDirection = TurnClockWise(currentDirection);
				ahead = currentPosition.NextPosition(nextDirection);
				Console.Error.WriteLine($"Clockwise {ahead}");

				if (WallAtPosition(ahead) || TrailAtPosition(ahead))
				{
					nextDirection = TurnAntiClockWise(currentDirection);

					Console.Error.WriteLine("Turning Anticlockwise");
					currentDirection = nextDirection;
				}
				else
				{
					Console.Error.WriteLine("Turning Clockwise");
					currentDirection = nextDirection;
				}
			}
			else
			{
				Console.Error.WriteLine("Straight ahead!");
				currentDirection = nextDirection;
			}
			// Write an action using Console.WriteLine()
			// To debug: Console.Error.WriteLine("Debug messages...");

			AnnounceDirection(currentDirection);
		}
	}

	private static bool TrailAtPosition(Position pos)
	{
		if (cells[pos.X, pos.Y])
		{
			Console.Error.WriteLine($"Trail at position {pos}");
			return true;
		}
		return false;
	}

	private static bool WallAtPosition(Position pos)
	{
		if (pos.X < 0 || pos.Y < 0 || pos.X >= XSize || pos.Y >= YSize)
		{
			Console.Error.WriteLine($"Wall at position {pos}");
			return true;
		}
		return false;
	}

	static Directions SetBestStartDirection()
	{
		var bestDirection = Directions.RIGHT;
		var right = XSize - currentPosition.X;
		var best = right;
		var left = XSize - right;
		if (left > best)
		{
			best = left;
			bestDirection = Directions.LEFT;
		}
		var down = YSize - currentPosition.Y;
		if (down > best)
		{
			best = down;
			bestDirection = Directions.DOWN;
		}
		var up = YSize - down;
		if (up > best)
		{
			best = up;
			bestDirection = Directions.UP;
		}
		return bestDirection;
	}

	static Directions TurnClockWise(Directions currentDirection)
	{
		switch (currentDirection)
		{
			case Directions.UP:
				return Directions.RIGHT;
			case Directions.RIGHT:
				return Directions.DOWN;
			case Directions.DOWN:
				return Directions.LEFT;
			default:
				return Directions.UP;
		}
	}

	static Directions TurnAntiClockWise(Directions currentDirection)
	{
		switch (currentDirection)
		{
			case Directions.UP:
				return Directions.LEFT;
			case Directions.RIGHT:
				return Directions.UP;
			case Directions.DOWN:
				return Directions.RIGHT;
			default:
				return Directions.DOWN;
		}
	}

	static void AnnounceDirection(Directions direction)
	{
		Console.WriteLine(direction.ToString()); // A single line with UP, DOWN, LEFT or RIGHT
	}
}