using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace kolpet.MazeSolver
{
    public class Solver
    {
        public static void Main(string[] args)
        {
            string xml = args.Length == 1 ? args[0] : File.ReadAllText("xml.txt");
            Maze maze = new Maze(xml);
            Agency agency = args.Length == 1 ? new Agency(maze) : new Agency(maze, 75);
            Agent best = agency.Solve();
            Console.Write(best.Result());
        }
    }

    public class Agency
    {
        PriorityQueue<Agent, double> agents = new PriorityQueue<Agent, double>();
        Maze maze;
        bool visualize;
        int delay;

        public Step FirstStep { get; }

        public Agency(Maze maze, int delay)
            : this(maze)
        {
            visualize = true;
            this.delay = delay;
        }

        public Agency(Maze maze)
        {
            this.maze = maze;

            Point? start = null;
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    int value = int.MaxValue;
                    switch (maze[i, j])
                    {
                        case Tile.Wall:
                            value = -1;
                            break;
                        case Tile.Start:
                            value = 0;
                            start = new(
                                i == 0 ? 1 : (i == 16 ? 15 : i),
                                j == 0 ? 1 : (j == 16 ? 15 : j));
                            break;
                        case Tile.End:
                            value = 0;
                            break;
                    }
                }
            }

            FirstStep = new Step(maze.Start.GetDirection(start!));
            Agent agent = new Agent(this, maze, maze.Start.Clone(), FirstStep);
            Enqueue(agent);
        }

        public Agent Solve()
        {
            Agent agent;
            do
            {
                agent = agents.Dequeue();
                agent.Step();
                if (visualize)
                {
                    Visualize();
                }
            } while (agent.Position != maze.End);
            return agent;
        }

        public void Enqueue(Agent agent)
        {
            double weight = agent.Steps + agent.Position.GetDistance(maze.End);
            agents.Enqueue(agent, weight);
        }

        public void Visualize()
        {
            StringBuilder[] text = new StringBuilder[17];
            for (int i = 0; i < 17; i++)
            {
                text[i] = new StringBuilder();
                for (int j = 0; j < 17; j++)
                {
                    char ch = ' ';
                    switch (maze[i, j])
                    {
                        case Tile.Wall:
                            ch = 'X';
                            break;
                        case Tile.Start:
                            ch = 'S';
                            break;
                        case Tile.End:
                            ch = 'E';
                            break;
                        case Tile.Trap:
                            ch = 'H';
                            break;
                    }
                    if (maze.IsWalked(i, j))
                    {
                        ch = '*';
                    }
                    text[i].Append(ch);
                }
            }
            foreach (var items in agents.UnorderedItems)
            {
                text[items.Element.Position.x][items.Element.Position.y] = 'A';
            }
            Console.Clear();
            for (int i = 0; i < 17; i++)
            {
                Console.WriteLine(text[i]);
            }
            Thread.Sleep(delay);
        }
    }

    public class Step
    {
        public Step? root;

        public District? rotation;
        public Direction direction;
        public int length;
        public int id;

        public Step(Direction direction)
        {
            this.direction = direction;
            rotation = null;
            length = 1;
        }

        public Step(Step root, Direction direction)
            : this(direction)
        {
            this.root = root;
        }

        private Step(Step clone)
        {
            root = clone.root;
            rotation = clone.rotation;
            direction = clone.direction;
            length = clone.length;
        }

        public Step Clone()
        {
            return new Step(this);
        }

#if DEBUG
        public string History => root?.History + $"\n{this}";
#endif

        public override string ToString()
        {
            return (rotation == null ? $"move {direction!}" : $"rotate {rotation} {direction}") + $"; id {id}";
        }
    }

    public class Agent
    {
        static int counter = 0;
        int id;
        Agency agency;
        IMaze maze;
        Step plannedStep;

        public Point Position { get; private set; }

        public int Steps { get; private set; }

        public Agent(Agency agency, IMaze maze, Point position, Step step)
        {
            this.agency = agency;
            this.maze = maze;
            Position = position;
            plannedStep = step;
            Steps = 1;
            id = counter++;
        }

        public Agent(Agent copy, Step step)
        {
            agency = copy.agency;
            maze = copy.maze;
            Position = copy.Position.Clone();
            Steps = copy.Steps;
            plannedStep = step;
            id = counter++;
        }

        public void Step()
        {
            if (Steps > 200) return;

            Direction direction = plannedStep.direction;
            Position.Move(direction);
            Steps++;
            plannedStep.id = id;
            maze.Walk(Position);

            if (Position == maze.End) return;
            if (maze[Position] == Tile.Trap)
            {
                maze = maze.Trigger(Position);
                Position = maze.Start.Clone();
                plannedStep = new Step(plannedStep, agency.FirstStep.direction);
                agency.Enqueue(this);
                return;
            }

            Queue<Step> choices = new Queue<Step>();
            Direction desire = Position.GetDirection(maze.End);
            for (int i = 0; i < 4; i++)
            {
                Point neighbour = Position.GetNeighbour(desire);
                if (maze[neighbour].IsValid() &&
                    !maze.IsWalked(neighbour))
                {
                    choices.Enqueue(new Step(plannedStep, desire));
                }
                desire = desire.Rotate(Direction.Left);
            }

            if (choices.Count > 0)
            {
                agency.Enqueue(this);
                plannedStep = choices.Dequeue();

                while (choices.Count > 0)
                {
                    agency.Enqueue(new Agent(this, choices.Dequeue()));
                }
            }
        }

        public string Result()
        {
            Step current = plannedStep;

            Stack<Step> steps = new Stack<Step>();
            do
            {
                Step step = new Step(current.direction);
                while (current.root != null && current.root.direction == step.direction && step.length < 14) { step.length++; current = current.root; }
                steps.Push(step);
                current = current.root;
            } while (current != null);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<Actions>");
            while (steps.Count > 0)
            {
                current = steps.Pop();
                if (current.rotation == null)
                {
                    builder.AppendLine("\t<Step>");
                    builder.AppendLine($"\t\t<Direction>{(int)current.direction!}</Direction>");
                    builder.AppendLine($"\t\t<CellNumber>{current.length}</CellNumber>");
                    builder.AppendLine("\t</Step>");
                }
                else
                {
                    builder.AppendLine("\t<Rotate>");
                    builder.AppendLine($"\t\t<District>{(int)current.rotation}</District>");
                    builder.AppendLine($"\t\t<Direction>{(int)current.direction!}</Direction>");
                    builder.AppendLine("\t</Rotate>");
                }
            }
            builder.AppendLine("</Actions>");
            return builder.ToString();
        }

        public override string ToString()
        {
            return $"Agent {id} [Position={Position}]; [Steps={Steps}]; [plannedStep={plannedStep}]";
        }
    }

    public enum Tile
    {
        Empty = 0,
        Wall = 1,
        Trap = 2,
        Start = 3,
        End = 4,
    }

    public enum Direction
    {
        Left = 1, //  3    2
        Right = 2,// 1 2  0 1
        Up = 3,   //  4    3
        Down = 4,
    }

    public enum CodeRotation
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }

    public enum District
    {
        Outside = 0,
        TopLeft = 1,
        Top = 2,
        TopRight = 3,
        Left = 4,
        Middle = 5,
        Right = 6,
        BottomLeft = 7,
        Bottom = 8,
        BottomRight = 9,
    }

    public class Point
    {
        public int x;
        public int y;

        public Point(int x, int y) { this.x = x; this.y = y; }

        public Point Clone()
        {
            return new Point(x, y);
        }

        public static bool operator ==(Point obj1, Point obj2)
        {
            return obj1.x == obj2.x && obj1.y == obj2.y;
        }

        public static bool operator !=(Point obj1, Point obj2)
        {
            return obj1.x != obj2.x || obj1.y != obj2.y;
        }

        public override string ToString()
        {
            return $"[x={x+1}]; [y={y+1}]";
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            return this == (Point)obj;
        }
    }

    public interface IMaze
    {
        Tile this[int row, int column] { get; }

        Tile this[Point point] { get; }

        Point Start { get; }

        Point End { get; }

        IMaze Rotate(District district, Direction direction);

        IMaze Trigger(Point trap);

        void Walk(Point point);

        bool IsWalked(Point point);
    }

    public class Maze : IMaze
    {
        Tile[,] tiles = new Tile[17, 17];
        bool[,] walked = new bool[17, 17];

        public int Level { get; private set; }

        public Tile this[int row, int column] { get => tiles[row, column]; }
        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start { get; private set; } = new Point(0, 0);

        public Point End { get; private set; } = new Point(0, 0);

        public Maze(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNode maze = doc.SelectSingleNode("Maze")!;
            Level = int.Parse(maze.SelectSingleNode("Level")!.InnerText!);
            Start.x = int.Parse(maze.SelectSingleNode("StartPoint/Row")!.InnerText!) - 1;
            Start.y = int.Parse(maze.SelectSingleNode("StartPoint/Column")!.InnerText!) - 1;
            End.x = int.Parse(maze.SelectSingleNode("EscapePoint/Row")!.InnerText!) - 1;
            End.y = int.Parse(maze.SelectSingleNode("EscapePoint/Column")!.InnerText!) - 1;

            XmlNode items = maze.SelectSingleNode("InsideItems")!;
            foreach (XmlNode item in items.ChildNodes)
            {
                Tile tile = item.Name == "Wall" ? Tile.Wall : Tile.Trap;
                int x = int.Parse(item.SelectSingleNode("Row")!.InnerText!) - 1;
                int y = int.Parse(item.SelectSingleNode("Column")!.InnerText!) - 1;
                tiles[x, y] = tile;
            }

            for (int i = 0; i < 17; i++)
            {
                tiles[i, 0] = Tile.Wall;
                tiles[i, 16] = Tile.Wall;
                tiles[0, i] = Tile.Wall;
                tiles[16, i] = Tile.Wall;
            }

            tiles[Start.x, Start.y] = Tile.Start;
            tiles[End.x, End.y] = Tile.End;
        }

        public IMaze Rotate(District district, Direction direction)
        {
            return new MazeView(this).Rotate(district, direction);
        }

        public IMaze Trigger(Point trap)
        {
            return new MazeView(this).Trigger(trap);
        }

        public void Walk(Point point) => walked[point.x, point.y] = true;

        public bool IsWalked(Point point) => walked[point.x, point.y];
        public bool IsWalked(int x, int y) => walked[x, y];
    }

    class MazeView : IMaze
    {
        Maze maze;
        List<Point> triggered;
        int[] rotations; // 0: base, 1: right, 2: down, 3: left
        bool[,] walked = new bool[17, 17];

        public MazeView(Maze maze)
        {
            this.maze = maze;
            triggered = new List<Point>();
            rotations = new int[10];
        }

        public MazeView(MazeView copy)
        {
            this.maze = copy.maze;
            triggered = copy.triggered.ToList();
            rotations = copy.rotations.ToArray();
        }

        public Tile this[int row, int column]
        {
            get
            {
                AdjustPoint(ref row, ref column);
                if (maze[row, column] == Tile.Trap && triggered.Contains(new Point(row, column)))
                {
                    return Tile.Empty;
                }

                return maze[row, column];
            }
        }

        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start => maze.Start;

        public Point End => maze.End;

        public IMaze Rotate(District district, Direction direction)
        {
            if (direction == Direction.Right)
            {
                rotations[(int)district] = (rotations[(int)district] + 1) % 4;
            }
            else
            {
                rotations[(int)district] = (rotations[(int)district] + 3) % 4;
            }
            return this;
        }

        public IMaze Trigger(Point trap)
        {
            if (!triggered.Contains(trap))
            {
                triggered.Add(trap.Clone());
            }
            return this;
        }

        public void Walk(Point point) => walked[point.x, point.y] = true;

        public bool IsWalked(Point point) => walked[point.x, point.y];

        public void AdjustPoint(ref int x, ref int y)
        {
            District district = Extensions.GetDistrict(x, y);
            if (district == District.Outside) return;

            Extensions.AdjustPoint((CodeRotation)rotations[(int)district], ref x, ref y);
        }
    }

    public static class Extensions
    {
        public static CodeRotation Convert(this Direction rotation)
        {
            return Enum.Parse<CodeRotation>(rotation.ToString());
        }
        public static Direction Convert(this CodeRotation rotation)
        {
            return Enum.Parse<Direction>(rotation.ToString());
        }

        public static Direction Rotate(this Direction direction, Direction rotation)
        {
            int i = (int)direction.Convert();
            int di = rotation == Direction.Right ? 1 : (rotation == Direction.Left ? 3 : 2);
            CodeRotation result = (CodeRotation)((i + di) % 4);
            return result.Convert();
        }

        public static District GetDistrict(int row, int column)
        {
            if (row == 0 || row == 16 || column == 0 || column == 16) return District.Outside;
            int x = row; int y = column;
            int district = (((x - 1) / 5) * 3) + ((y - 1) / 5) + 1;
            return (District)district;
        }

        public static void AdjustPoint(CodeRotation rotation, ref int row, ref int column)
        {
            if (rotation == CodeRotation.Up) return;

            int drow = row - 1; int dcolumn = column - 1;
            int dx = drow % 5; int dy = dcolumn % 5;
            if (rotation == CodeRotation.Left || rotation == CodeRotation.Right) // rotate district to left, show tile to right
            {
                int temp = 4 - dx;
                dx = dy;
                dy = temp;
            }

            if (rotation == CodeRotation.Right || rotation == CodeRotation.Down) // inverse for Right and Down rotations
            {
                dx = (4 - dx) % 5;
                dy = (4 - dy) % 5;
            }
            row = row - (drow % 5) + dx;
            column = column - (dcolumn % 5) + dy;
        }

        public static Tile PreviewRotation(this IMaze maze, CodeRotation rotation, int x, int y)
        {
            District district = GetDistrict(x, y);
            if (district == District.Outside) return maze[x, y];

            AdjustPoint(rotation, ref x, ref y);
            return maze[x, y];
        }

        public static Point GetNeighbour(this Point point, Direction direction)
        {
            Point clone = point.Clone();
            clone.Move(direction);
            return clone;
        }

        public static void Move(this Point point, Direction direction, int count = 1)
        {
            switch (direction)
            {
                case Direction.Left:
                    point.y -= count;
                    break;
                case Direction.Right:
                    point.y += count;
                    break;
                case Direction.Up:
                    point.x -= count;
                    break;
                case Direction.Down:
                    point.x += count;
                    break;
            }
        }

        public static Direction GetDirection(this Point orig, Point dest)
        {
            int dx = orig.x - dest.x;
            int dy = orig.y - dest.y;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                return dx > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                return dy > 0 ? Direction.Left : Direction.Right;
            }
        }

        public static double GetDistance(this Point orig, Point dest)
        {
            return Math.Sqrt(Math.Pow(orig.x - dest.x, 2) * Math.Pow(orig.y - dest.y, 2));
        }

        public static bool IsDistrictBorder(this Point point)
        {
            return point.x % 5 == 0 || point.x % 5 == 4 || point.y % 5 == 0 || point.y % 5 == 4;
        }

        public static bool IsValid(this Tile tile)
        {
            return tile == Tile.Empty || tile == Tile.Trap || tile == Tile.End;
        }
    }
}