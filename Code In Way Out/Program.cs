using System;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Xml;
using System.Drawing;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections;

namespace kolpet.MazeSolver
{
    public class Solver
    {
        public static void Main(string[] args)
        {
            string xml = args.Length == 1 ? args[0] : File.ReadAllText("xml.txt");
            Maze maze = new Maze(xml);
            Agency agency = args.Length == 1 ? new Agency(maze) : new Agency(maze, 50);
            Agent best = agency.Solve();
            Console.Write(best.Result());
        }
    }

    public class Agency
    {
        PriorityQueue<Agent, int> agents = new PriorityQueue<Agent, int>();
        Maze maze;
        MazeView mazeView;
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

            mazeView = new MazeView(maze);

            for (int i = 1; i <= 9; i++)
            {
                mazeView.RotateCopy((District)i, Direction.Right);
                mazeView.RotateCopy((District)i, Direction.Left).RotateCopy((District)i, Direction.Left);
            }

            FirstStep = new Step(maze.Start.GetDirection(start!));
            Agent agent = new Agent(this, mazeView, maze.Start.Clone(), FirstStep);
            agents.Enqueue(agent, 0);

            if (maze.Level == 3)
            {
                District district = Extensions.GetDistrict(start.x, start.y);
                Step rotateLeft = new Step(district, Direction.Left);
                agent = new Agent(this, mazeView, maze.Start.Clone(), rotateLeft);
                agent.Step();
                Step rotateRight = new Step(district, Direction.Right);
                agent = new Agent(this, mazeView, maze.Start.Clone(), rotateRight);
                agent.Step();

                Step rotateRightAgain = new Step(rotateRight, district, Direction.Right);
                agent = new Agent(agent, rotateRightAgain);
                agent.Step();

                MazeView optimalMaze = maze.FindBestWeight(start);
                if (optimalMaze != mazeView)
                {
                    int i = maze.Cache.IndexOf(optimalMaze);
                    District optimalDistrict = (District)((i + 1) / 2);
                    if (i > 0 && i < 19 && optimalDistrict != district)
                    {
                        Step rotateOptimal = new Step(optimalDistrict, i % 2 == 1 ? Direction.Right : Direction.Left);
                        agent = new Agent(this, mazeView, maze.Start.Clone(), rotateOptimal);
                        agents.Enqueue(agent, 0);
                    }
                }
            }
        }

        public Agent Solve()
        {
            Agent agent;
            do
            {
                agent = agents.Dequeue();
                int weight = agent.GetWeight();
                agent.Step();
                if (visualize)
                {
                    Visualize();
                    Console.WriteLine();
                    Console.WriteLine($"[weight={weight}] {agent}");
                    //Console.WriteLine($"tracked: [weight={agents.UnorderedItems.Single((x) => x.Element == agent).Priority}] {agent}");
                    Thread.Sleep(delay);
                }
            } while (agent.Position != maze.End);
            return agent;
        }

        public void Enqueue(Agent agent)
        {
            agents.Enqueue(agent, agent.GetWeight());
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
                    if (mazeView.GetTrail(i, j) == Trail.Cold)
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
                Console.WriteLine($"{text[i]}  {i}: {maze.Cache[i]}");
            }
            Console.WriteLine($"                   {17}: {maze.Cache[17]}");
            Console.WriteLine($"                   {18}: {maze.Cache[18]}");
            MazeView winner = maze.Cache[17];
            for (int i = 0; i < winner.Agents.Count && i < 5; i++)
            {
                Console.WriteLine($"                   {winner.Agents[i]}");
            }
        }
    }

    public class Step
    {
        public static int counter = 0;

        public Step? root;

        public District? district;
        public Direction direction;
        public int length;
        public int id;

        private Step()
        {
            id = counter++;
        }

        public Step(Direction direction)
            :this()
        {
            this.direction = direction;
            district = null;
            length = 1;
        }

        public Step(Step root, Direction direction)
            : this(direction)
        {
            this.root = root;
        }

        public Step(District district, Direction rotation)
            : this()
        {
            this.district = district;
            direction = rotation;
        }

        public Step(Step root, District district, Direction rotation)
            :this(district, rotation)
        {
            this.root = root;
        }

        private Step(Step clone)
            : this()
        {
            root = clone.root;
            district = clone.district;
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
            return (district == null ? $"move {direction!}" : $"rotate {district} {direction}") + $"; id {id}";
        }
    }

    public class Agent
    {
        static int counter = 0;
        int id;
        Agency agency;
        Step plannedStep;
        int rotationLimit = 3;
        Point goal;

        public MazeView Maze { get; private set; }

        public Point Position { get; private set; }

        public int Steps { get; private set; }

        public Agent(Agency agency, MazeView maze, Point position, Step step)
        {
            this.agency = agency;
            this.Maze = maze;
            Position = position;
            plannedStep = step;
            Steps = 0;
            id = counter++;
            goal = maze.End;
            Maze.Agents.Add(this);
        }

        public Agent(Agent copy, Step step)
            :this(copy)
        {
            plannedStep = step;
        }

        public Agent(Agent copy, Point goal)
            :this(copy)
        {
            this.goal = goal;
        }

        public Agent(Agent copy)
        {
            agency = copy.agency;
            Maze = copy.Maze;
            Position = copy.Position.Clone();
            Steps = copy.Steps;
            goal = copy.goal;
            plannedStep = copy.plannedStep.Clone();
            rotationLimit = copy.rotationLimit;
            id = counter++;
            Maze.Agents.Add(this);
        }

        // TODO: get A* weight working with tests
        public int GetWeight()
        {
            return Steps;
        }

        public void Step()
        {
            if (Steps > 200) return;

            if (plannedStep.district != null)
            {
                //if (Extensions.GetDistrict(Position.x, Position.y) == plannedStep.district) return;
                Maze.SetTrail(Position, Trail.Cold);
                Maze.Agents.Remove(this);
                Maze = Maze.RotateCopy((District)plannedStep.district, plannedStep.direction);
                Maze.Agents.Add(this);
                Steps += 5;
                rotationLimit--;
                Maze.SetTrail(Position, Trail.Hot);
            }
            else
            {
                Maze.SetTrail(Position, Trail.Cold);
                Direction direction = plannedStep.direction;
                Position.Move(direction);
                Steps++;
                Maze.SetTrail(Position, Trail.Hot);
            }

            if (Position == Maze.End) return;
            if (Position == Maze.Start)
            {
                if (!Maze[Position.GetNeighbour(agency.FirstStep.direction)].IsValid()) return;
                plannedStep = new Step(plannedStep, agency.FirstStep.direction);
                agency.Enqueue(this);
                return;
            }
            if (Maze[Position] == Tile.Trap)
            {
                Maze.Agents.Remove(this);
                Maze = Maze.TriggerCopy(Position);
                Maze.Agents.Add(this);
                Position = Maze.Start.Clone();
                plannedStep = new Step(plannedStep, agency.FirstStep.direction);
                agency.Enqueue(this);
                return;
            }
            if (Position == goal)
            {
                goal = Maze.End;
            }
            if (Maze.GetTrail(Position) == Trail.Cold) return;
            //if (Maze.GetWeight(Position) == 99 && rotationLimit == 0) return;
            PlanStep();
        }

        public void PlanStep()
        {
            Queue<Step> choices = new Queue<Step>();
            District district = Extensions.GetDistrict(Position.x, Position.y);
            Direction desire = Position.GetDirection(goal);
            for (int i = 0; i < 4; i++)
            {
                Point neighbour = Position.GetNeighbour(desire);
                if (Maze[neighbour].IsValid() &&
                    Maze.GetTrail(neighbour) != Trail.Cold)
                {
                    choices.Enqueue(new Step(plannedStep, desire));
                }

                if (Maze.Level == 3 && rotationLimit > 0)
                {
                    District neighbourDistrict = Extensions.GetDistrict(neighbour.x, neighbour.y);
                    if (neighbourDistrict != district &&
                        neighbourDistrict != District.Outside)
                    {
                        MazeView preview = Maze.RotateCopy(neighbourDistrict, Direction.Right);
                        if (preview[neighbour.x, neighbour.y].IsValid() &&
                            preview.GetTrail(neighbour) != Trail.Cold)
                        {
                            choices.Enqueue(new Step(plannedStep, neighbourDistrict, Direction.Right));
                        }

                        preview = Maze.RotateCopy(neighbourDistrict, Direction.Left);
                        if (preview[neighbour.x, neighbour.y].IsValid() &&
                            preview.GetTrail(neighbour) != Trail.Cold)
                        {
                            choices.Enqueue(new Step(plannedStep, neighbourDistrict, Direction.Left));
                        }
                    }
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
                Step step = current.Clone();
                if (step.district == null)
                {
                    while (current.root != null &&
                        current.root.district == null &&
                        current.root.direction == step.direction &&
                        step.length < 14)
                    {
                        step.length++;
                        current = current.root;
                    }
                }
                steps.Push(step);
                current = current.root;
            } while (current != null);

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("<Actions>");
            while (steps.Count > 0)
            {
                current = steps.Pop();
                if (current.district == null)
                {
                    builder.AppendLine("\t<Step>");
                    builder.AppendLine($"\t\t<Direction>{(int)current.direction!}</Direction>");
                    builder.AppendLine($"\t\t<CellNumber>{current.length}</CellNumber>");
                    builder.AppendLine("\t</Step>");
                }
                else
                {
                    builder.AppendLine("\t<Rotate>");
                    builder.AppendLine($"\t\t<District>{(int)current.district}</District>");
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

#if DEBUG
        public string Visualize
        {
            get
            {
                string map = Maze.Print(false);
                int moves = 0;
                int pos = 0;
                Point oldPos = Maze.Start.Clone();
                Stack<Step> stack = new Stack<Step>();
                stack.Push(plannedStep);
                while (stack.Peek().root != null) stack.Push(stack.Peek().root!);
                while (stack.Count > 0)
                {
                    Step step = stack.Pop();
                    if (step.district == null)
                    {
                        moves++;
                        oldPos.Move(step.direction);
                        pos = oldPos.x * 36 + oldPos.y * 2;
                        map = map.Remove(pos, 2).Insert(pos, (moves < 10 ? " " : string.Empty) + moves);
                    }
                }
                pos = Position.x * 36 + Position.y * 2;
                map = map.Remove(pos, 2).Insert(pos, "AA");
                return map;
            }
        }
#endif
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

    public enum Trail
    {
        None,
        Hot,
        Cold
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
        int Level { get; }

        Tile this[int row, int column] { get; }

        Tile this[Point point] { get; }

        Point Start { get; }

        Point End { get; }

        int GetWeight(int x, int y);

        void SetWeight(int x, int y, int weight);
    }

    public class Maze : IMaze
    {
        Tile[,] tiles = new Tile[17, 17];
        int[,] weight = new int[17, 17];

        public int Level { get; private set; }

        public Tile this[int row, int column] { get => tiles[row, column]; }

        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start { get; private set; } = new Point(0, 0);

        public Point End { get; private set; } = new Point(0, 0);

        public List<MazeView> Cache { get; } = new List<MazeView>();

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

                for (int j = 0; j < 17; j++)
                    weight[i, j] = 99;
            }

            tiles[Start.x, Start.y] = Tile.Start;
            tiles[End.x, End.y] = Tile.End;
        }

        public int GetWeight(Point point) => weight[point.x, point.y];

        public int GetWeight(int x, int y) => weight[x, y];

        public void SetWeight(int x, int y, int weight) => this.weight[x, y] = weight;

        public string Print(bool weights)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    string str = "  ";
                    switch (this[i, j])
                    {
                        case Tile.Wall:
                            str = "XX";
                            break;
                        case Tile.Start:
                            str = "SS";
                            break;
                        case Tile.End:
                            str = "EE";
                            break;
                        case Tile.Trap:
                            str = "HH";
                            break;
                    }
                    if (weights)
                    {
                        str = (GetWeight(i, j) < 10 ? " " : string.Empty) + GetWeight(i, j).ToString();
                    }
                    text.Append(str);
                }
                text.AppendLine();
            }
            return text.ToString();
        }

#if DEBUG
        public string Visualize => Print(true);
#endif
    }

    public class MazeView : IMaze
    {
        Trail[,] walked = new Trail[17, 17];
        int[,] weight = new int[17, 17];

        public int Level => Maze.Level;

        public Maze Maze { get; }

        public List<Point> Triggered { get; }

        public int[] Rotations { get; } // 0: base, 1: right, 2: down, 3: left

        public List<Agent> Agents { get; set; } = new List<Agent>();

        public MazeView(Maze maze)
        {
            Maze = maze;
            Triggered = new List<Point>();
            Rotations = new int[10];
            maze.Cache.Add(this);
            for (int i = 0; i < 17; i++)
                for (int j = 0; j < 17; j++)
                    weight[i, j] = 99;
            Floodfill();
        }

        public MazeView(MazeView copy)
        {
            Maze = copy.Maze;
            Triggered = copy.Triggered.ToList();
            Rotations = copy.Rotations.ToArray();
            Maze.Cache.Add(this);
            for (int i = 0; i < 17; i++)
                for (int j = 0; j < 17; j++)
                    weight[i, j] = 99;
        }

        public MazeView(MazeView copy, District district, Direction rotation)
            : this(copy)
        {
            Rotate(district, rotation);
            Floodfill();
        }

        public MazeView(MazeView copy, Point trap)
            : this(copy)
        {
            Trigger(trap);
            Floodfill();
        }

        public Tile this[int row, int column]
        {
            get
            {
                AdjustPoint(ref row, ref column);
                if (Maze[row, column] == Tile.Trap && Triggered.Contains(new Point(row, column)))
                {
                    return Tile.Empty;
                }

                return Maze[row, column];
            }
        }

        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start => Maze.Start;

        public Point End => Maze.End;
        public MazeView Rotate(District district, Direction direction)
        {
            if (direction == Direction.Right)
            {
                Rotations[(int)district] = (Rotations[(int)district] + 1) % 4;
            }
            else
            {
                Rotations[(int)district] = (Rotations[(int)district] + 3) % 4;
            }
            return this;
        }

        public MazeView Trigger(Point trap)
        {
            if (!Triggered.Contains(trap))
            {
                Triggered.Add(trap.Clone());
            }
            return this;
        }

        public void SetTrail(Point point, Trail trail) => walked[point.x, point.y] = trail;

        public Trail GetTrail(Point point) => walked[point.x, point.y];

        public Trail GetTrail(int x, int y) => walked[x, y];

        public int GetWeight(Point point) => GetWeight(point.x, point.y);

        public int GetWeight(int x, int y)
        {
            AdjustPoint(ref x, ref y);
            return weight[x, y];
        }

        public void SetWeight(int x, int y, int w)
        {
            if (Maze.GetWeight(x, y) > w)
                Maze.SetWeight(x, y, w);

            AdjustPoint(ref x, ref y);
            weight[x, y] = w;
        }

        public void AdjustPoint(ref int x, ref int y)
        {
            District district = Extensions.GetDistrict(x, y);
            if (district == District.Outside) return;

            Extensions.AdjustPoint((CodeRotation)Rotations[(int)district], ref x, ref y);
        }

        public bool PreviewRotationEqual(MazeView view, District district, Direction direction)
        {
            if (!Enumerable.SequenceEqual(Triggered, view.Triggered))
                return false;

            int[] rotationsCopy = Rotations.ToArray();
            if (direction == Direction.Right)
            {
                rotationsCopy[(int)district] = (rotationsCopy[(int)district] + 1) % 4;
            }
            else
            {
                rotationsCopy[(int)district] = (rotationsCopy[(int)district] + 3) % 4;
            }

            return Enumerable.SequenceEqual(rotationsCopy, view.Rotations);
        }

        public bool PreviewTriggerEqual(MazeView view, Point trigger)
        {
            if (!Enumerable.SequenceEqual(Rotations, view.Rotations))
                return false;

            List<Point> triggeredCopy = Triggered.ToList();
            triggeredCopy.Add(trigger);

            return Enumerable.SequenceEqual(triggeredCopy, view.Triggered);
        }

        private void Floodfill()
        {
            Queue<Tuple<Point, int>> points = new Queue<Tuple<Point, int>>();
            Point start = new(
                End.x == 0 ? 1 : (End.x == 16 ? 15 : End.x),
                End.y == 0 ? 1 : (End.y == 16 ? 15 : End.y));
            weight[End.x, End.y] = 0;
            points.Enqueue(Tuple.Create(start, 1));

            while (points.TryDequeue(out Tuple<Point, int> current))
            {
                Point pos = current.Item1;
                SetWeight(pos.x, pos.y, current.Item2);

                Direction desire = pos.GetDirection(Start);
                for (int i = 0; i < 4; i++)
                {
                    Point neighbour = pos.GetNeighbour(desire);
                    if ((this[neighbour] == Tile.Empty || this[neighbour] == Tile.Trap) &&
                        GetWeight(neighbour.x, neighbour.y) > current.Item2 + 1)
                    {
                        int cost = this[neighbour] == Tile.Trap ? 25 : 1;
                        points.Enqueue(Tuple.Create(neighbour, current.Item2 + cost));
                    }
                    if (this[neighbour] == Tile.Start)
                    {
                        SetWeight(neighbour.x, neighbour.y, current.Item2 + 1);
                    }

                    desire = desire.Rotate(Direction.Left);
                }
            }

            //Print(true)
        }

        public string Print(bool weights)
        {
            StringBuilder text = new StringBuilder();
            for (int i = 0; i < 17; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    string str = "  ";
                    switch (this[i, j])
                    {
                        case Tile.Wall:
                            str = "XX";
                            break;
                        case Tile.Start:
                            str = "SS";
                            break;
                        case Tile.End:
                            str = "EE";
                            break;
                        case Tile.Trap:
                            str = "HH";
                            break;
                    }
                    if (str == "  " && weights)
                    {
                        str = (GetWeight(i, j) < 10 ? " " : string.Empty) + GetWeight(i, j).ToString();
                    }
                    text.Append(str);
                }
                text.AppendLine();
            }
            return text.ToString();
        }

#if DEBUG
        public string Visualize => Print(false);
#endif

        public override string ToString()
        {
            return $"[Rotations={string.Join(',', Rotations)}]; [Triggered={string.Join(',', Triggered)}]; [Agents={Agents.Count}]";
        }
    }

    public static class Extensions
    {
        public static CodeRotation Convert(this Direction rotation) => rotation switch
        {
            Direction.Left => CodeRotation.Left,
            Direction.Right => CodeRotation.Right,
            Direction.Up => CodeRotation.Up,
            Direction.Down => CodeRotation.Down,
        };

        public static Direction Convert(this CodeRotation rotation) => rotation switch
        {
            CodeRotation.Left => Direction.Left,
            CodeRotation.Right => Direction.Right,
            CodeRotation.Up => Direction.Up,
            CodeRotation.Down => Direction.Down,
        };

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
            return point.x % 5 == 0 || point.x % 5 == 1 || point.y % 5 == 0 || point.y % 5 == 1; // starts from 1
        }

        public static District GetNeighbourDistrict(this Point point, Direction direction)
        {
            int district = (int)GetDistrict(point.x, point.y);
            if (point.x % 5 == 1 && district > 2 && direction == Direction.Up)
            {
                return (District)(district - 3);
            }
            if (point.x % 5 == 0 && district < 7 && direction == Direction.Down)
            {
                return (District)(district + 3);
            }
            if (point.y % 5 == 1 && district % 3 != 1 && direction == Direction.Left)
            {
                return (District)((district + 2) % 3);
            }
            if (point.y % 5 == 0 && district % 3 != 0 && direction == Direction.Right)
            {
                return (District)((district + 1) % 3);
            }
            return District.Outside;
        }

        public static bool IsValid(this Tile tile)
        {
            return tile == Tile.Empty || tile == Tile.Trap || tile == Tile.End;
        }

        public static MazeView RotateCopy(this MazeView maze, District district, Direction direction)
        {
            foreach(MazeView cache in maze.Maze.Cache)
            {
                if (maze.PreviewRotationEqual(cache, district, direction))
                {
                    return cache;
                }
            }

            return new MazeView(maze, district, direction);
        }

        public static MazeView TriggerCopy(this MazeView maze, Point trap)
        {
            foreach (MazeView cache in maze.Maze.Cache)
            {
                if (maze.PreviewTriggerEqual(cache, trap))
                {
                    return cache;
                }
            }

            return new MazeView(maze, trap);
        }

        public static MazeView FindBestWeight(this Maze maze, Point position)
        {
            MazeView? result = null;
            int best = int.MaxValue;
            foreach (MazeView cache in maze.Cache)
            {
                int weight = cache.GetWeight(position);
                if (weight != 0 && weight < best)
                {
                    result = cache;
                    best = weight;
                }
            }
            return result!;
        }
    }
}