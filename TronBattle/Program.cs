using System;

namespace TronBattle
{
	internal class Program
	{
		private static Position[] GetPlayerPositions(int playerCount)
		{
			var initialPositions = new Position[playerCount];
			for (var i = 0; i < playerCount; i++)
			{
				var positionInputs = Console.ReadLine().Split(' ');
				var xNew = Int32.Parse(positionInputs[2]);
				var yNew = Int32.Parse(positionInputs[3]);
				initialPositions[i] = new Position(xNew, yNew);
			}
			return initialPositions;
		}

		private static void Main()
		{
			var inputs = Console.ReadLine().Split(' ');
			var count = Int32.Parse(inputs[0]);
			var myNumber = Int32.Parse(inputs[1]);
			var initialPositions = GetPlayerPositions(count);
			var player = new Player(count, myNumber, initialPositions);
			// game loop
			do
			{
				Console.WriteLine(player.GetNextDirection().ToString());
				Console.ReadLine(); //Throw away user details, not needed now
				player.AddLatestPositions(GetPlayerPositions(count));
			} while (true);
		}
	}


	internal enum Direction
	{
		// ReSharper disable InconsistentNaming
		UP,
		RIGHT,
		DOWN,
		LEFT,
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
		private readonly bool[,] _cells;

		private Direction _currentDirection = Direction.UP;
		private bool _start = true;
		private readonly int _numberOfPlayers;
		private readonly int _myNumber;
		private readonly Position[] _positions;
		private readonly int _xSize;
		private readonly int _ySize;

		public Player(int count, int myNumber, Position[] initialPositions)
		{
			_xSize = 30;
			_ySize = 20;
			_cells = new bool[_xSize, _ySize];
			_numberOfPlayers = count;
			_myNumber = myNumber;
			_positions = initialPositions;
			AddLatestPositions(initialPositions);
		}

		private Position CurrentPosition => _positions[_myNumber];

		public void AddLatestPositions(Position[] positions)
		{
			Console.Error.WriteLine("Getting positions...");
			for (var i = 0; i < _numberOfPlayers; i++)
			{
				var thisPos = positions[i];
				_positions[i] = thisPos;
				_cells[thisPos.X, thisPos.Y] = true;

				Console.Error.WriteLine(i == _myNumber ? "Me" : $"Player {i}");
				Console.Error.WriteLine(thisPos);
			}
		}

		public Direction GetNextDirection()
		{
			if (_start)
			{
				_currentDirection = SetBestStartDirection(CurrentPosition);
				_start = false;
				return _currentDirection;
			}

			var nextDirection = _currentDirection;
			var ahead = CurrentPosition.NextPosition(nextDirection);
			Console.Error.WriteLine($"Ahead {ahead}");
			if (WallAtPosition(ahead) || TrailAtPosition(ahead))
			{
				nextDirection = TurnClockWise(_currentDirection);
				ahead = CurrentPosition.NextPosition(nextDirection);
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

			return _currentDirection;
		}

		private bool TrailAtPosition(Position pos)
		{
			if (_cells[pos.X, pos.Y])
			{
				Console.Error.WriteLine($"Trail at position {pos}");
				return true;
			}

			return false;
		}

		private bool WallAtPosition(Position pos)
		{
			if (pos.X < 0 || pos.Y < 0 || pos.X >= _xSize || pos.Y >= _ySize)
			{
				Console.Error.WriteLine($"Wall at position {pos}");
				return true;
			}

			return false;
		}

		private Direction SetBestStartDirection(Position position)
		{
			var bestDirection = Direction.RIGHT;
			var right = _xSize - position.X;
			var best = right;
			var left = _xSize - right;
			if (left > best)
			{
				best = left;
				bestDirection = Direction.LEFT;
			}
			var down = _ySize - position.Y;
			if (down > best)
			{
				best = down;
				bestDirection = Direction.DOWN;
			}
			var up = _ySize - down;
			if (up > best)
			{
				bestDirection = Direction.UP;
				return bestDirection;
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
	}
}