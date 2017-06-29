using System;

namespace TronBattle
{
	internal enum Direction
	{
		// ReSharper disable InconsistentNaming
		UP, DOWN, LEFT, RIGHT
		// ReSharper restore InconsistentNaming
	}

	internal struct Position
	{
		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public int X { get; }
		public int Y { get; }

		public Position NextPosition(Direction direction)
		{
			switch (direction)
			{
				case Direction.UP:
					return new Position(X, Y - 1);
				case Direction.RIGHT:
					return new Position(X + 1, Y);
				case Direction.DOWN:
					return new Position(X, Y + 1);
				case Direction.LEFT:
					return new Position(X - 1, Y);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}
		public override string ToString() => $"{{X:{X}, Y:{Y}}}";
	}

	class Player
	{
		private static int _numberOfPlayers;
		private static int _myNumber;


		private const int XSize = 30;
		private const int YSize = 20;
		private static readonly bool[,] Cells = new bool[XSize, YSize];

		private static Position _currentPosition = new Position(0, 0);

		private static Direction _currentDirection = Direction.UP;
		private static bool _start = true;
		static void Main()
		{
			// game loop
			while (true)
			{
				var playerInputs = Console.ReadLine().Split(' ');
				_numberOfPlayers = Int32.Parse(playerInputs[0]); // total number of players (2 to 4).
				_myNumber = Int32.Parse(playerInputs[1]); // your player number (0 to 3).
				Console.Error.WriteLine("Getting positions...");
				for (var i = 0; i < _numberOfPlayers; i++)
				{
					var positionInputs = Console.ReadLine().Split(' ');
					var xNew = Int32.Parse(positionInputs[2]); // starting X coordinate of lightcycle (can be the same as X0 if you play before this player)
					var yNew = Int32.Parse(positionInputs[3]); // starting Y coordinate of lightcycle (can be the same as Y0 if you play before this player)
					Cells[xNew, yNew] = true;
					var position = new Position(xNew, yNew);

					if (i == _myNumber)
					{
						_currentPosition = position;
						Console.Error.WriteLine("Me");
					}
					else
					{
						Console.Error.WriteLine($"Player {i}");
					}
					Console.Error.WriteLine(position);
				}
				if (_start)
				{
					_start = false;
					_currentDirection = SetBestStartDirection();
					AnnounceDirection(_currentDirection);
					continue;
				}
				var nextDirection = _currentDirection;
				var ahead = _currentPosition.NextPosition(nextDirection);
				Console.Error.WriteLine($"Ahead {ahead}");
				if (WallAtPosition(ahead) || TrailAtPosition(ahead))
				{
					nextDirection = TurnClockWise(_currentDirection);
					ahead = _currentPosition.NextPosition(nextDirection);
					Console.Error.WriteLine($"Clockwise {ahead}");

					if (WallAtPosition(ahead) || TrailAtPosition(ahead))
					{
						nextDirection = TurnAntiClockWise(_currentDirection);

						Console.Error.WriteLine("Turning Anticlockwise");
						_currentDirection = nextDirection;
					}
					else
					{
						Console.Error.WriteLine("Turning Clockwise");
						_currentDirection = nextDirection;
					}
				}
				else
				{
					Console.Error.WriteLine("Straight ahead!");
					_currentDirection = nextDirection;
				}
				// Write an action using Console.WriteLine()
				// To debug: Console.Error.WriteLine("Debug messages...");

				AnnounceDirection(_currentDirection);
			}
		}

		private static bool TrailAtPosition(Position pos)
		{
			if (!Cells[pos.X, pos.Y])
			{
				return false;
			}

			Console.Error.WriteLine($"Trail at position {pos}");
			return true;
		}

		private static bool WallAtPosition(Position pos)
		{
			if (pos.X >= 0 && pos.Y >= 0 && pos.X < XSize && pos.Y < YSize)
			{
				return false;
			}

			Console.Error.WriteLine($"Wall at position {pos}");
			return true;
		}

		private static Direction SetBestStartDirection()
		{
			var bestDirection = Direction.RIGHT;
			var right = XSize - _currentPosition.X;
			var best = right;
			var left = XSize - right;
			if (left > best)
			{
				best = left;
				bestDirection = Direction.LEFT;
			}
			var down = YSize - _currentPosition.Y;
			if (down > best)
			{
				best = down;
				bestDirection = Direction.DOWN;
			}
			var up = YSize - down;
			if (up > best)
			{
				bestDirection = Direction.UP;
			}
			return bestDirection;
		}

		private static Direction TurnClockWise(Direction lastDirection)
		{
			switch (lastDirection)
			{
				case Direction.UP:
					return Direction.RIGHT;
				case Direction.RIGHT:
					return Direction.DOWN;
				case Direction.DOWN:
					return Direction.LEFT;
				case Direction.LEFT:
					return Direction.UP;
				default:
					throw new ArgumentOutOfRangeException(nameof(lastDirection), lastDirection, null);
			}
		}

		private static Direction TurnAntiClockWise(Direction lastDirection)
		{
			switch (lastDirection)
			{
				case Direction.UP:
					return Direction.LEFT;
				case Direction.RIGHT:
					return Direction.UP;
				case Direction.DOWN:
					return Direction.RIGHT;
				case Direction.LEFT:
					return Direction.DOWN;
				default:
					throw new ArgumentOutOfRangeException(nameof(lastDirection), lastDirection, null);
			}
		}

		private static void AnnounceDirection(Direction direction) => Console.WriteLine(direction.ToString());
	}
}