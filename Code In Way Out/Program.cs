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
using System.Diagnostics;

namespace kolpet.MazeSolver
{
    public class Solver
    {
        public static void Main(string[] args)
        {
            string xml = args.Length == 1 ? args[0] : File.ReadAllText("xml.txt");
#if DEBUG
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            Maze maze = new Maze(xml);
            Agency agency = args.Length == 1 ? new Agency(maze) : new Agency(maze, 50);
            Agent best = agency.Solve();
            Console.Write(best.Result());
#if DEBUG
            sw.Stop();
            Console.WriteLine($"Time for execution: [ElapsedMilliseconds={sw.ElapsedMilliseconds}]");
#endif
        }
    }

    public class Agency
    {
        PriorityQueue<Agent, int> agents = new PriorityQueue<Agent, int>();
        Maze maze;
        MazeView mazeView;
        bool visualize;
        int delay;

#if DEBUG
        List<MazeView> views = new List<MazeView>();
#endif

        public Step FirstStep { get; }

        public Agent? Best { get; set; }

        public Agency(Maze maze, int delay)
            : this(maze)
        {
            visualize = true;
            this.delay = delay;
        }

        public Agency(Maze maze)
        {
            this.maze = maze;
            Best = null;

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
#if DEBUG
            views.Add(mazeView);
#endif

            FirstStep = new Step(maze.Start.GetDirection(start!));
            Agent agent = new Agent(this, mazeView, maze.Start.Clone(), FirstStep);
            if (maze.Level == 3)
            {
                for (int i = 1; i <= 9; i++)
                {
                    MazeView mazeRight = mazeView.RotateCopy((District)i, Direction.Right);
                    mazeRight.Floodfill(maze.Start, 6, 11);
                    MazeView mazeLeft = mazeView.RotateCopy((District)i, Direction.Left);
                    mazeLeft.Floodfill(maze.Start, 6, 11);
                    MazeView mazeDown = mazeRight.RotateCopy((District)i, Direction.Right);
                    mazeDown.Floodfill(maze.Start, 11, 21);

#if DEBUG
                    views.Add(mazeRight);
                    views.Add(mazeDown);
                    views.Add(mazeLeft);
#endif
                }

                District district = Extensions.GetDistrict(start.x, start.y);
                Step rotateLeft = new Step(district, Direction.Left);
                agent = new Agent(this, mazeView, maze.Start.Clone(), rotateLeft);

                Step rotateRight = new Step(district, Direction.Right);
                agent = new Agent(this, mazeView, maze.Start.Clone(), rotateRight);

                Step rotateRightAgain = new Step(rotateRight, district, Direction.Right);
                agent = new Agent(agent);
                agent.ExecuteStep(rotateRightAgain);

                //MazeView optimalMaze = maze.FindBestWeight(start);
                //if (optimalMaze != mazeView)
                //{
                //    int i = 1;
                //    while (optimalMaze.Rotations[i] == 0) { i++; }
                //    District optimalDistrict = (District)((i + 1) / 2);
                //    if (i > 0 && i < 19 && optimalDistrict != district)
                //    {
                //        Step rotateOptimal = new Step(optimalDistrict, i % 2 == 1 ? Direction.Right : Direction.Left);
                //        agent = new Agent(this, mazeView, maze.Start.Clone(), rotateOptimal);
                //        agents.Enqueue(agent, 0);
                //    }
                //}
            }
        }

        public Agent Solve()
        {
            Agent agent;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            do
            {
                agent = agents.Dequeue();
                agent.PlanAndExecuteSteps();
#if DEBUG
                if (visualize)
                {
                    Visualize();
                    Console.WriteLine();
                    Console.WriteLine($"[agents.Count={agents.Count}]; [weight={agent.GetWeight()}] {agent}");
                    //Console.WriteLine($"tracked: [weight={agents.UnorderedItems.Single((x) => x.Element == agent).Priority}] {agent}");
                    Thread.Sleep(delay);

                    if (Best != null)
                    {
                        Console.WriteLine($"Best: {Best}");
                    }
                }
#endif
            } while (agents.Count > 0 && (Best == null || sw.ElapsedMilliseconds < 500));
            return Best;
        }

        public void Enqueue(Agent agent)
        {
            agents.Enqueue(agent, agent.GetWeight());
        }

#if DEBUG
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
                    text[i].Append(ch);
                }
            }
            foreach (var items in agents.UnorderedItems)
            {
                text[items.Element.Position.x][items.Element.Position.y] = 'A';
            }
            Console.Clear();
            int viewCount = views.Count;
            for (int i = 0; i < 28; i++)
            {
                Console.WriteLine(string.Format("{0}{1}",
                    i < 17 ? text[i] : "                 ",
                    i < viewCount ? $"  {i}: {views[i]}" : string.Empty));
            }
            //MazeView winner = views[17];
            //for (int i = 0; i < winner.Agents.Count && i < 5; i++)
            //{
            //    Console.WriteLine($"                   {winner.Agents[i]}");
            //}
        }
#endif
    }

    public class Step
    {

#if DEBUG
        public static int counter = 0;
        public int id;
        public bool trigger;
#endif

        public Step? root;

        public District? district;
        public Direction direction;
        public int length;

        private Step()
        {
#if DEBUG
            id = counter++;
#endif
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

        public override string ToString()
        {
            return (district == null ? $"move {direction!}" : $"rotate {district} {direction}") + $"; id {id}" + (trigger ? " trigger" : string.Empty);
        }
#endif
    }

    public class Agent
    {
        static int counter = 0;
        int id;
        Agency agency;
        int rotationLimit = 10;
        int stepsExecuted;
        int lowestWeight = int.MaxValue;

        public MazeView Maze { get; private set; }

        public Point Position { get; private set; }

        public Step? LastStep { get; private set; }

        public int Score { get; private set; }

        public int PathLength { get; private set; }

        public HashSet<int> Triggered { get; private set; }

        public Agent(Agency agency, MazeView maze, Point position, Step step)
        {
            this.agency = agency;
            this.Maze = maze;
            Position = position;
            Score = 0;
            PathLength = 0;
            id = counter++;
            Triggered = new HashSet<int>();
            stepsExecuted = 0;
#if DEBUG
            Maze.Agents.Add(this);
#endif
            ExecuteStep(step);
        }

        public Agent(Agent copy)
        {
            agency = copy.agency;
            Maze = copy.Maze;
            Position = copy.Position.Clone();
            Score = copy.Score;
            PathLength = copy.PathLength;
            LastStep = copy.LastStep;
            rotationLimit = copy.rotationLimit;
            id = counter++;
            Triggered = copy.Triggered.ToHashSet();
            stepsExecuted = copy.stepsExecuted;
#if DEBUG
            Maze.Agents.Add(this);
#endif
        }

        // TODO: get A* weight working with tests
        public int GetWeight()
        {
            int weight = Position.GetWalkDistance(Maze.End);
            if (Maze.Base.GetWeight(Maze.End) != int.MaxValue)
            {
                weight = Maze.Base.GetWeight(Maze.End) - Score;
                if (weight < lowestWeight)
                    lowestWeight = weight;
                else
                    weight = lowestWeight;
            }

            if (Maze.GetWeight(Position) != int.MaxValue)
            {
                weight += Maze.GetWeight(Position) - Maze.Base.GetWeight(Position);
            }
            else
            {
                weight = (int)Math.Pow(weight, 2);
            }

            weight += Score;
            return weight;
        }

#if DEBUG
        public void DebugStep()
        {
            // level3: down 1;right 7;down 7;left 4;down 2;break;rotate bottomright right;break
            // rotatopotato: down 1;left 2;down 3;rotate topleft left;rotate topleft left;left 2;down 1
            // repeatrotations:  down 1;left 3;rotate top left;right 1;down 4;right 2;down 1
            string steps = @"down 9;up 1"; //
            string[] search = steps.Split(';', StringSplitOptions.RemoveEmptyEntries);
            string[] history = LastStep.History.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            int hLine = 0; int sLine = 0;
            bool canBreak = false;
            for (; sLine < search.Length; sLine++)
            {
                if (hLine >= history.Length) break;
                if (search[sLine] == "break")
                {
                    canBreak = !canBreak;
                    continue;
                }

                string[] parts = search[sLine].Split(' ');
                if (parts[0] == "rotate")
                {
                    if (hLine >= history.Length) break;
                    if (!history[hLine].Contains(search[sLine], StringComparison.InvariantCultureIgnoreCase))
                        return;
                    hLine++;
                }
                else
                {
                    int times = int.Parse(parts[1]);
                    for (int j = 0; j < times; j++)
                    {
                        if (hLine >= history.Length) break;
                        if (!history[hLine].Contains("move " + parts[0], StringComparison.InvariantCultureIgnoreCase))
                            return;
                        hLine++;
                    }
                }
            }

            if (canBreak || (sLine == search.Length && hLine == history.Length))
               Debugger.Break();
        }
#endif

        public void ExecuteStep(Step step)
        {
            if (LastStep != null) 
                step.root = LastStep;
            LastStep = step;
#if DEBUG
            DebugStep();
#endif
            if (agency.Best != null && Score + Position.GetWalkDistance(Maze.End) > agency.Best.Score)
            {
                return;
            }

            if (LastStep.district != null)
            {
                //if (Extensions.GetDistrict(Position.x, Position.y) == plannedStep.district) return;
#if DEBUG
                Maze.Agents.Remove(this);
#endif
                Maze = Maze.RotateCopy((District)LastStep.district, LastStep.direction);
#if DEBUG
                Maze.Agents.Add(this);
#endif
                Score += 5;
                PathLength += 10;
                stepsExecuted++;
                rotationLimit--;

                if (Score < Maze.GetWeight(Position))
                {
                    Maze.Floodfill(Position, Score, PathLength);
                }
            }
            else
            {
                Direction direction = LastStep.direction;
                Position.Move(direction);
                Score++;
                PathLength++;
                stepsExecuted++;
            }

            if (Position == Maze.End)
            {
                if (agency.Best == null || Score < agency.Best.Score)
                    agency.Best = this;
                return;
            }
            if (Maze[Position] == Tile.Trap &&
                Maze.Level >= 2 &&
                !Triggered.Contains(Maze.GetTrapId(Position)))
            {
                Triggered.Add(Maze.GetTrapId(Position));
                //Position = Maze.Start.Clone();
                Score += PathLength;
#if DEBUG
                LastStep.trigger = true;
#endif

                // duplicate path with correction for rotations
                Step current = LastStep;
                for (int i = 1; i <= 9; i++)
                {
                    if (Maze.Rotations[i] == (int)CodeRotation.Down)
                    {
                        Step rotate = new Step(LastStep, (District)i, Direction.Right);
                        LastStep = rotate;
                        rotate = new Step(LastStep, (District)i, Direction.Right);
                        LastStep = rotate;
                    }
                    if (Maze.Rotations[i] % 2 == 1) // Righ-Left
                    {
                        Direction rotation = ((CodeRotation)Maze.Rotations[i]).Convert().Rotate(Direction.Down);
                        Step rotate = new Step(LastStep, (District)i, rotation);
                        LastStep = rotate;
                    }
                }

                Stack<Step> repeat = new Stack<Step>();
                for(int i = 0; i < stepsExecuted; i++)
                {
                    repeat.Push(current);
                    current = current.root;
                }
                for (int i = 0; i < stepsExecuted; i++)
                {
                    current = repeat.Pop().Clone();
                    current.root = LastStep;
                    LastStep = current;
                }
            }

            if (Maze.GetWalked(Position) && Maze.GetWeight(Position) <= Score) return;
            Maze.SetWalked(Position.x, Position.y);
            if (Score > Maze.Base.GetWeight(Maze.End)) return;

            agency.Enqueue(this);
        }

        public void PlanAndExecuteSteps()
        {
            if (Position == Maze.Start)
            {
                if (!Maze[Position.GetNeighbour(agency.FirstStep.direction)].IsValid()) return;
                ExecuteStep(new Step(agency.FirstStep.direction));
                return;
            }

            Queue<Step> choices = new Queue<Step>();
            District district = Extensions.GetDistrict(Position.x, Position.y);
            Direction desire = Position.GetDirection(Maze.End);
            for (int i = 0; i < 4; i++)
            {
                Point neighbour = Position.GetNeighbour(desire);
                if (Maze[neighbour].IsValid() && 
                    Score <= Maze.GetWeight(neighbour))
                {
                    choices.Enqueue(new Step(desire));
                }

                if (Maze.Level == 3 && rotationLimit > 0)
                {
                    District neighbourDistrict = Extensions.GetDistrict(neighbour.x, neighbour.y);
                    if (neighbourDistrict != district &&
                        neighbourDistrict != District.Outside)
                    {
                        bool turned = false;
                        MazeView preview = Maze.RotateCopy(neighbourDistrict, Direction.Right);
                        if (preview[neighbour].IsValid() &&
                            Score <= preview.GetWeight(Position))
                        {
                            choices.Enqueue(new Step(neighbourDistrict, Direction.Right));
                            turned = true;
                        }

                        preview = Maze.RotateCopy(neighbourDistrict, Direction.Left);
                        if (preview[neighbour].IsValid() &&
                            Score <= preview.GetWeight(Position))
                        {
                            choices.Enqueue(new Step(neighbourDistrict, Direction.Left));
                            turned = true;
                        }

                        preview = preview.RotateCopy(neighbourDistrict, Direction.Left);
                        if (preview[neighbour].IsValid() &&
                            Score <= preview.GetWeight(Position) &&
                            !turned)
                        {
                            Step preStep = new Step(neighbourDistrict, Direction.Left);
                            Agent agent = new Agent(this);
                            agent.ExecuteStep(preStep);
                        }
                    }
                }
                desire = desire.Rotate(Direction.Left);
            }

            if (choices.Count > 0)
            {
                Step topStep = choices.Dequeue();
                while (choices.Count > 0)
                {
                    Agent agent = new Agent(this);
                    agent.ExecuteStep(choices.Dequeue());
                }

                ExecuteStep(topStep);
            }
        }

        public string Result()
        {
            Step current = LastStep;

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

#if DEBUG
        public override string ToString()
        {
            return $"Agent {id} [Position={Position}]; [Score={Score}]; [plannedStep={LastStep}]";
        }

        public string Visualize
        {
            get
            {
                string map = Maze.Print(false);
                int moves = 0;
                int pos = 0;
                Point oldPos = Maze.Start.Clone();
                Stack<Step> stack = new Stack<Step>();
                stack.Push(LastStep);
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

#if DEBUG
        public override string ToString()
        {
            return $"[x={x+1}]; [y={y+1}]";
        }
#endif

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
        int[,] traps = new int[17, 17];

        public int Level { get; private set; }

        public Tile this[int row, int column] { get => tiles[row, column]; }

        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start { get; private set; } = new Point(0, 0);

        public Point End { get; private set; } = new Point(0, 0);

        public Dictionary<long, MazeView> Cache { get; } = new Dictionary<long, MazeView>();

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
            int trapCount = 1;
            foreach (XmlNode item in items.ChildNodes)
            {
                Tile tile = item.Name == "Wall" ? Tile.Wall : Tile.Trap;
                int x = int.Parse(item.SelectSingleNode("Row")!.InnerText!) - 1;
                int y = int.Parse(item.SelectSingleNode("Column")!.InnerText!) - 1;
                tiles[x, y] = tile;

                if (tile == Tile.Trap)
                {
                    traps[x, y] = trapCount++;
                }
            }

            for (int i = 0; i < 17; i++)
            {
                tiles[i, 0] = Tile.Wall;
                tiles[i, 16] = Tile.Wall;
                tiles[0, i] = Tile.Wall;
                tiles[16, i] = Tile.Wall;

                for (int j = 0; j < 17; j++)
                    weight[i, j] = int.MaxValue;
            }

            tiles[Start.x, Start.y] = Tile.Start;
            tiles[End.x, End.y] = Tile.End;
        }

        public int GetWeight(Point point) => weight[point.x, point.y];

        public int GetWeight(int x, int y) => weight[x, y];

        public void SetWeight(int x, int y, int weight) => this.weight[x, y] = weight;

        public int GetTrapId(Point point) => GetTrapId(point.x, point.y);

        public int GetTrapId(int x, int y) => traps[x, y];

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
                    if (this[i, j].IsValid() && weights)
                    {
                        str = (GetWeight(i, j) % 100 < 10 ? " " : string.Empty) + (GetWeight(i, j) % 100);
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
        int[,] weight = new int[17, 17];
        bool[,] walked = new bool[17, 17];
        long hash;

        public int Level => Base.Level;

        public Maze Base { get; }

        public int[] Rotations { get; } // 0: base, 1: right, 2: down, 3: left

#if DEBUG
        public List<Agent> Agents { get; set; } = new List<Agent>();
#endif

        public MazeView(Maze maze)
        {
            Base = maze;
            Rotations = new int[10];
            hash = 10000000000;
            maze.Cache.Add(hash, this);
            for (int i = 0; i < 17; i++)
                for (int j = 0; j < 17; j++)
                    weight[i, j] = int.MaxValue;
            Floodfill(Start);
        }

        public MazeView(MazeView copy)
        {
            Base = copy.Base;
            Rotations = copy.Rotations.ToArray();
            for (int i = 0; i < 17; i++)
                for (int j = 0; j < 17; j++)
                    weight[i, j] = int.MaxValue;
        }

        public MazeView(MazeView copy, District district, Direction rotation)
            : this(copy)
        {
            Rotate(district, rotation);
            long hash = 10000000000;
            for (int i = 0; i < 9; i++)
            {
                hash += Rotations[i + 1] * (int)Math.Pow(10, i);
            }
            Base.Cache.Add(hash, this);
        }

        public Tile this[int row, int column]
        {
            get
            {
                AdjustPoint(ref row, ref column);
                return Base[row, column];
            }
        }

        public Tile this[Point point] { get => this[point.x, point.y]; }

        public Point Start => Base.Start;

        public Point End => Base.End;
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

        public int GetWeight(Point point) => GetWeight(point.x, point.y);

        public int GetWeight(int x, int y)
        {
            AdjustPoint(ref x, ref y);
            return weight[x, y];
        }

        public void SetWeight(int x, int y, int w)
        {
            if (w < Base.GetWeight(x, y))
                Base.SetWeight(x, y, w);

            AdjustPoint(ref x, ref y);
            weight[x, y] = w;
        }

        public bool GetWalked(Point point) => GetWalked(point.x, point.y);

        public bool GetWalked(int x, int y)
        {
            AdjustPoint(ref x, ref y);
            return walked[x, y];
        }

        public void SetWalked(int x, int y)
        {
            AdjustPoint(ref x, ref y);
            walked[x, y] = true;
        }

        public int GetTrapId(Point point) => GetTrapId(point.x, point.y);

        public int GetTrapId(int x, int y)
        {
            AdjustPoint(ref x, ref y);
            return Base.GetTrapId(x, y);
        }

        public void AdjustPoint(ref int x, ref int y)
        {
            District district = Extensions.GetDistrict(x, y);
            if (district == District.Outside) return;

            Extensions.AdjustPoint((CodeRotation)Rotations[(int)district], ref x, ref y);
        }

        public bool PreviewRotationEqual(MazeView view, District district, Direction direction)
        {
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

        public void Floodfill(Point start, int baseWeight = 1, int basePath = 1)
        {
            Queue<Tuple<Point, int, int>> points = new Queue<Tuple<Point, int, int>>();

            if (start == Start)
            {
                weight[Start.x, Start.y] = 0;
                start = new(
                    Start.x == 0 ? 1 : (Start.x == 16 ? 15 : Start.x),
                    Start.y == 0 ? 1 : (Start.y == 16 ? 15 : Start.y));
                if (!this[start].IsValid()) return;
                if (this[start] == Tile.Trap)
                    baseWeight = 2;
            }

            weight[start.x, start.y] = baseWeight;
            points.Enqueue(Tuple.Create(start, baseWeight, basePath));
            while (points.TryDequeue(out Tuple<Point, int, int> current)) // point, weight, pathLength
            {
                Point pos = current.Item1;

                Direction desire = pos.GetDirection(End);
                for (int i = 0; i < 4; i++)
                {
                    Point neighbour = pos.GetNeighbour(desire);
                    Tile tile = this[neighbour];
                    if (tile == Tile.Empty || tile == Tile.Trap)
                    {
                        int pathLength = current.Item3 + 1;
                        int w = current.Item2 + 1 + (this[neighbour] == Tile.Trap ? pathLength : 0);
                        if (w < GetWeight(neighbour.x, neighbour.y))
                        {
                            SetWeight(neighbour.x, neighbour.y, w);
                            points.Enqueue(Tuple.Create(neighbour, w, pathLength));
                        }
                    }
                    if (tile == Tile.End)
                    {
                        SetWeight(neighbour.x, neighbour.y, current.Item2 + 1);
                    }

                    desire = desire.Rotate(Direction.Left);
                }
            }
            //Print(true)
        }

#if DEBUG
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
                    if (this[i, j].IsValid() && weights)
                    {
                        str = (GetWeight(i, j) % 100 < 10 ? " " : string.Empty) + (GetWeight(i, j) % 100);
                    }
                    text.Append(str);
                }
                text.AppendLine();
            }
            return text.ToString();
        }

        public string Visualize => Print(true);

        public override string ToString()
        {
            return $"[Rotations={string.Join(',', Rotations)}]; [Agents={Agents.Count}]";
        }
#endif
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

        public static double GetAirDistance(this Point orig, Point dest)
        {
            return Math.Sqrt(Math.Pow(orig.x - dest.x, 2) * Math.Pow(orig.y - dest.y, 2));
        }

        public static int GetWalkDistance(this Point orig, Point dest)
        {
            return Math.Abs(orig.x - dest.x) + Math.Abs(orig.y - dest.y);
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
            long hash = 10000000000;
            for (int i = 0; i < 9; i++)
            {
                hash += maze.Rotations[i + 1] * (int)Math.Pow(10, i);
            }
            int pos = (int)district - 1;
            int pow = (int)Math.Pow(10, pos);
            long dx = (hash / pow) % 10;
            hash -= dx * pow;
            dx = (dx + (int)direction.Convert()) % 4;
            hash += dx * pow;

            if (maze.Base.Cache.TryGetValue(hash, out MazeView cached))
            {
                return cached;
            }

            return new MazeView(maze, district, direction);
        }

        public static MazeView FindBestWeight(this Maze maze, Point position)
        {
            MazeView? result = null;
            int best = int.MaxValue;
            foreach (MazeView cache in maze.Cache.Values)
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