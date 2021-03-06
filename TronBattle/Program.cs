﻿using System;

namespace TronBattle
{
	internal static class Program
	{
		private static Position[] GetPlayerPositions(int playerCount)
		{
			var initialPositions = new Position[playerCount];
			for (var i = 0; i < playerCount; i++)
			{
				var positionInputs = Console.ReadLine().Split(' ');
				var xInit = Int32.Parse(positionInputs[0]);
				var yInit = Int32.Parse(positionInputs[1]);
				var xNew = Int32.Parse(positionInputs[2]);
				var yNew = Int32.Parse(positionInputs[3]);
				initialPositions[i] = new Position(xInit, yInit, xNew, yNew);
			}
			return initialPositions;
		}

		private static void Main()
		{
			var inputs = Console.ReadLine().Split(' ');
			var count = Int32.Parse(inputs[0]);
			var myNumber = Int32.Parse(inputs[1]);
			var player = new Player(count, myNumber);
			// game loop
			do
			{
				player.AddLatestPositions(GetPlayerPositions(count));
				Console.WriteLine(player.GetNextDirection().ToString());
				Console.ReadLine(); //Throw away user details, not needed now
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
		public Position(int xInit, int yInit, int xNew, int yNew)
		{
			XInit = xInit;
			YInit = yInit;
			XNew = xNew;
			YNew = yNew;
		}

		public int XInit { get; }
		public int YInit { get; }
		public int XNew { get; }
		public int YNew { get; }

		public Position NextPosition(Direction direction)
		{
			switch (direction)
			{
				case Direction.UP:
					return new Position(XNew, YNew, XNew, YNew - 1);
				case Direction.RIGHT:
					return new Position(XNew, YNew, XNew + 1, YNew);
				case Direction.DOWN:
					return new Position(XNew, YNew, XNew, YNew + 1);
				case Direction.LEFT:
					return new Position(XNew, YNew, XNew - 1, YNew);
				default:
					throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
			}
		}

		public override string ToString() => $"{{XInit:{XInit}, YInit:{YInit}, XNew:{XNew}, YNew:{YNew}}}";
	}

	internal sealed class Player
	{
		private readonly bool[,] _cells;

		private Direction _currentDirection = Direction.UP;
		private bool _start = true;
		private readonly int _numberOfPlayers;
		private readonly int _myNumber;
		private readonly Position[] _positions;
		private readonly int _xSize;
		private readonly int _ySize;

		public Player(int count, int myNumber)
		{
			_xSize = 30;
			_ySize = 20;
			_cells = new bool[_xSize, _ySize];
			_numberOfPlayers = count;
			_myNumber = myNumber;
			_positions = new Position[count];
		}

		private Position CurrentPosition => _positions[_myNumber];

		public void AddLatestPositions(Position[] positions)
		{
			Console.Error.WriteLine("Getting positions...");
			for (var i = 0; i < _numberOfPlayers; i++)
			{
				var thisPos = positions[i];
				_positions[i] = thisPos;
				if (thisPos.XInit >= 0)
				{
					_cells[thisPos.XInit, thisPos.YInit] = true;
				}
				if (thisPos.XNew >= 0)
				{
					_cells[thisPos.XNew, thisPos.YNew] = true;
				}

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
			if (_cells[pos.XNew, pos.YNew])
			{
				Console.Error.WriteLine($"Trail at position {pos}");
				return true;
			}

			return false;
		}

		private bool WallAtPosition(Position pos)
		{
			if (pos.XNew < 0 || pos.YNew < 0 || pos.XNew >= _xSize || pos.YNew >= _ySize)
			{
				Console.Error.WriteLine($"Wall at position {pos}");
				return true;
			}

			return false;
		}

		private Direction SetBestStartDirection(Position position)
		{
			var bestDirection = Direction.RIGHT;
			var right = _xSize - position.XNew;
			var best = right;
			var left = _xSize - right;
			if (left > best)
			{
				best = left;
				bestDirection = Direction.LEFT;
			}
			var down = _ySize - position.YNew;
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