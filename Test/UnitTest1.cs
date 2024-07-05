using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;

namespace kolpet.MazeSolver
{
    [TestClass]
    public class UnitTest1
    {
        public static Dictionary<string, string> xmls = new Dictionary<string, string>();
        public static Dictionary<string, string> maps = new Dictionary<string, string>();
        public static Dictionary<string, string> results = new Dictionary<string, string>();

        [ClassInitialize]
        public static void FixtureSetUp(TestContext context)
        {
            foreach (string path in Directory.EnumerateFiles(Path.Combine("Maze", "xml")))
            {
                xmls.Add(Path.GetFileNameWithoutExtension(path), File.ReadAllText(path));
            }
            foreach (string path in Directory.EnumerateFiles(Path.Combine("Maze", "map")))
            {
                maps.Add(Path.GetFileNameWithoutExtension(path), File.ReadAllText(path));
            }
            foreach (string path in Directory.EnumerateFiles(Path.Combine("Maze", "result")))
            {
                results.Add(Path.GetFileNameWithoutExtension(path), File.ReadAllText(path));
            }
        }

        [TestMethod]
        public void TestLevel1()
        {
            Agent result = RunTest(xmls["Level1Example"]);
            EvaluateResult(results["Level1Example"], result);
        }

        [TestMethod]
        public void TestLevel2()
        {
            Agent result = RunTest(xmls["Level2Example"]);
            EvaluateResult(results["Level2Example"], result);
        }

        [TestMethod]
        public void TestLevel3()
        {
            Agent result = RunTest(xmls["Level3Example"]);
            EvaluateResult(results["Level3Example"], result);
        }

        [TestMethod]
        public void TestCustomLevel()
        {
            TestMaze testMaze = new TestMaze(3, new Point(2, 1), new Point(2, 17))
                .DrawWalls(2, 3, 15, 3)
                .DrawWalls(16, 5, 3, 5)
                .DrawWalls(2, 7, 15, 7)
                .DrawWalls(16, 9, 3, 9)
                .DrawWalls(2, 11, 15, 11)
                .DrawWalls(16, 13, 3, 13)
                .DrawWalls(2, 15, 15, 15)
                .AddTrap(8, 8);

            Agent result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result);
        }

        [TestMethod]
        public void TestDeadEndLevel()
        {
            TestMaze testMaze = new TestMaze(3, maps["DeadEnd"]);

            Agent result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result, 25);
        }

        [TestMethod]
        public void TestDoubleRotateStartLevel()
        {
            {
                TestMaze testMaze = new TestMaze(3, maps["DoubleRotateStart"]);

                Agent result = RunTest(testMaze.Print());
                EvaluateResult(string.Empty, result, 30);
            }
        }

        [TestMethod]
        public void TestDoubleRotateLateLevel()
        {
            {
                TestMaze testMaze = new TestMaze(3, maps["DoubleRotateLate"]);

                Agent result = RunTest(testMaze.Print());
                EvaluateResult(string.Empty, result, 30);
            }
        }

        [TestMethod]
        public void TestMultipleTrapsLevel()
        {
            {
                TestMaze testMaze = new TestMaze(3, maps["MultipleTraps"]);

                Agent result = RunTest(testMaze.Print());
                EvaluateResult(string.Empty, result, 43);
            }
        }

        [TestMethod]
        public void TestDoubleDeadEndLevel()
        {
            {
                TestMaze testMaze = new TestMaze(3, maps["DoubleDeadEnd"]);

                Agent result = RunTest(testMaze.Print(), delaySec: 10);
                EvaluateResult(string.Empty, result, 32);
            }
        }

        [TestMethod]
        public void TestTryolKillerLevel()
        {
            Agent result = RunTest(xmls["TryolKiller"], delaySec: 10);
            EvaluateResult(string.Empty, result);
        }

        [TestMethod]
        public void TestTryolKillerModifiedLevel()
        {
            TestMaze testMaze = new TestMaze(3, maps["TryolKillerModified"]);

            Agent result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result, 136);
        }

        [TestMethod]
        public void TestRotatoPotatoLevel()
        {
            TestMaze testMaze = new TestMaze(3, maps["RotatoPotato"]);

            Agent result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result, 59);
        }

        [TestMethod]
        public void TestRepeatRotationsLevel()
        {
            TestMaze testMaze = new TestMaze(3, maps["RepeatRotations"]);

            Agent result = RunTest(testMaze.Print());
            EvaluateResult(string.Empty, result, 50);
        }

        [TestMethod]
        public void TestUltimateLevel()
        {
            Agent result = RunTest(xmls["TheUltimate"], delaySec: 60);
            EvaluateResult(string.Empty, result);
        }

        [TestMethod]
        public void TestExtensions()
        {
            // Rotate
            {
                Direction dir = Direction.Up;

                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Right, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Right);
                Assert.AreEqual(Direction.Up, dir);

                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Right, dir);
                dir = dir.Rotate(Direction.Left);
                Assert.AreEqual(Direction.Up, dir);

                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Down, dir);
                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Up, dir);

                dir = Direction.Right;

                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Left, dir);
                dir = dir.Rotate(Direction.Down);
                Assert.AreEqual(Direction.Right, dir);
            }

            // GetDistrict
            {
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 5));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(0, 16));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(5, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(5, 16));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 0));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 5));
                Assert.AreEqual(District.Outside, Extensions.GetDistrict(16, 16));

                Assert.AreEqual(District.TopLeft, Extensions.GetDistrict(1, 1));
                Assert.AreEqual(District.TopLeft, Extensions.GetDistrict(5, 5));

                Assert.AreEqual(District.Top, Extensions.GetDistrict(1, 6));
                Assert.AreEqual(District.Top, Extensions.GetDistrict(5, 10));

                Assert.AreEqual(District.TopRight, Extensions.GetDistrict(1, 11));
                Assert.AreEqual(District.TopRight, Extensions.GetDistrict(5, 15));

                Assert.AreEqual(District.Left, Extensions.GetDistrict(6, 1));
                Assert.AreEqual(District.Left, Extensions.GetDistrict(10, 5));

                Assert.AreEqual(District.Middle, Extensions.GetDistrict(6, 6));
                Assert.AreEqual(District.Middle, Extensions.GetDistrict(10, 10));

                Assert.AreEqual(District.Right, Extensions.GetDistrict(6, 11));
                Assert.AreEqual(District.Right, Extensions.GetDistrict(10, 15));

                Assert.AreEqual(District.BottomLeft, Extensions.GetDistrict(11, 1));
                Assert.AreEqual(District.BottomLeft, Extensions.GetDistrict(15, 5));

                Assert.AreEqual(District.Bottom, Extensions.GetDistrict(11, 6));
                Assert.AreEqual(District.Bottom, Extensions.GetDistrict(15, 10));

                Assert.AreEqual(District.BottomRight, Extensions.GetDistrict(11, 11));
                Assert.AreEqual(District.BottomRight, Extensions.GetDistrict(15, 15));
            }

            // AdjustPoint
            {
                /* .1...  District.TopLeft, CodeRotation.Right
                 * ....4  1: 1,2
                 * .....  2: 4,1
                 * 2....  3: 5,4
                 * ...3.  4: 2,5
                 */
                int row = 1; int column = 2;
                Extensions.AdjustPoint(CodeRotation.Up, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);

                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(4, row);
                Assert.AreEqual(1, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(5, row);
                Assert.AreEqual(4, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(2, row);
                Assert.AreEqual(5, column);
                Extensions.AdjustPoint(CodeRotation.Right, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);

                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(2, row);
                Assert.AreEqual(5, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(5, row);
                Assert.AreEqual(4, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(4, row);
                Assert.AreEqual(1, column);
                Extensions.AdjustPoint(CodeRotation.Left, ref row, ref column);
                Assert.AreEqual(1, row);
                Assert.AreEqual(2, column);
            }

            // PreviewRotation
            {
                /* .X...  District.TopLeft
                 * HX...
                 * .X...
                 * .X...
                 * .X...
                 */
                TestMaze testMaze = new TestMaze(3, new Point(2, 1), new Point(2, 17))
                    .DrawWalls(2, 3, 6, 3)
                    .AddTrap(3, 2);

                Maze maze = new Maze(testMaze.Print());

                Assert.AreEqual(Tile.Empty, maze.PreviewRotation(CodeRotation.Right, 1, 1));
                Assert.AreEqual(Tile.Trap, maze.PreviewRotation(CodeRotation.Right, 1, 4));
                for (int i = 1; i <= 5; i++)
                {
                    Assert.AreEqual(Tile.Wall, maze.PreviewRotation(CodeRotation.Right, 2, i));
                }

                Assert.AreEqual(Tile.Empty, maze[1, 1]);
                Assert.AreEqual(Tile.Trap, maze[2, 1]);
                for (int i = 1; i <= 5; i++)
                {
                    Assert.AreEqual(Tile.Wall, maze[i, 2]);
                }
            }
        }

        private Agent RunTest(string xml, int delaySec = 10, bool throwAssert = true)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Maze maze = new Maze(xml);
            Agency agency = new Agency(maze);
            Task<Agent> solver = Task.Run(() =>
            {
                Agent best = agency.Solve();
                sw.Stop();
                return best;
            });

            Task timeout = Task.Delay(delaySec * 1000);

            Task.WaitAny(solver, timeout);

            try
            {
                Assert.IsFalse(solver.IsFaulted, "test threw exception, ex: " + solver.Exception?.InnerException);
                Assert.IsTrue(solver.IsCompleted, $"test timeouted after {delaySec} seconds");
            }
            catch
            {
                if (throwAssert)
                    throw;
            }

            Logger.LogMessage($"time of execution: {sw.ElapsedMilliseconds}ms");
            return solver.Result;
        }

        private void EvaluateResult(string expectedResult, Agent result, int expectedSteps = 0)
        {
            string actualResult = result.Result();
            int actualSteps = CountSteps(actualResult);
            if (!string.IsNullOrEmpty(expectedResult))
            {
                if (actualResult != expectedResult)
                {
                    Logger.LogMessage("result deviates from expected result");
                }

                if (expectedSteps == 0)
                    expectedSteps = CountSteps(expectedResult);
            }

            if (expectedSteps != 0)
            {
                Logger.LogMessage($"expected steps: {expectedSteps}; actual steps: {actualSteps}");
            }
            else
            {
                Logger.LogMessage($"steps: {actualSteps}");
            }

#if DEBUG
            Logger.LogMessage(result.LastStep.History);
#else
            Logger.LogMessage(actualResult);
#endif

            if (expectedSteps != 0)
            {
                Assert.IsTrue(actualSteps <= expectedSteps, "test performed worse than previous best");
                Assert.AreEqual(expectedSteps, actualSteps, "test performed better than previous best, validate and readjust test");
            }
        }

        private int CountSteps(string result) 
        {
            MatchCollection matches = Regex.Matches(result, "<Step>\\s+\\S+\\s+\\D+(\\d+)");
            int steps = 0;
            foreach (Match match in matches)
            {
                steps += int.Parse(match.Groups[1].Value);
            }

            matches = Regex.Matches(result, "<Rotate>");
            steps += matches.Count * 5;
            return steps;
        }

        private class TestMaze
        {
            public int Level { get; set; }

            public Point Start { get; private set; } = new Point(0, 0);

            public Point End { get; private set; } = new Point(0, 0);

            public List<Point> Walls { get; private set; } = new List<Point>();

            public List<Point> Traps { get; private set; } = new List<Point>();

            public TestMaze(int level, Point start, Point end)
            {
                Level = level;
                Start = start;
                End = end;
            }

            public TestMaze(int level, string map)
            {
                map = map.Replace(Environment.NewLine, string.Empty).Replace("X\t", "X").Replace("S\t", "S").Replace("E\t", "E").Replace("H\t", "H").Replace("\t", " ");
                Level = level;
                for(int i = 0; i < 17; i++)
                {
                    for (int j = 0; j < 17; j++)
                    {
                        int pos = i * 17 + j;
                        Point point = new Point(i + 1, j + 1);
                        if (i == 0 || i == 16 || j == 0 || j == 16)
                        {
                            if (map[pos] == 'S')
                                Start = point;
                            if (map[pos] == 'E')
                                End = point;
                        }
                        else if (map[pos] == 'X')
                        {
                            Walls.Add(point);
                        }
                        else if (map[pos] == 'H')
                        {
                            Traps.Add(point);
                        }
                    }
                }
            }

            public TestMaze AddTrap(int x, int y)
            {
                Traps.Add(new Point(x, y));
                return this;
            }

            public TestMaze DrawWalls(int x1, int y1, int x2, int y2)
            {
                if (x1 != x2 && y1 != y2) return this;
                int dx = Math.Sign(x2 - x1);
                int dy = Math.Sign(y2 - y1);
                int i = x1; int j = y1;
                while (i != x2 || j != y2)
                {
                    Point p = new Point(i, j);
                    if (!Walls.Contains(p))
                    {
                        Walls.Add(p);
                    }
                    i += dx; j += dy;
                }
                Point p2 = new Point(x2, y2);
                if (!Walls.Contains(p2))
                {
                    Walls.Add(p2);
                }
                return this;
            }

            public string Print()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine( "<Maze>");
                sb.AppendLine($"\t<Level>{Level}</Level>");
                sb.AppendLine( "\t<StartPoint>");
                sb.AppendLine($"\t\t<Row>{Start.x}</Row>");
                sb.AppendLine($"\t\t<Column>{Start.y}</Column>");
                sb.AppendLine( "\t</StartPoint>");
                sb.AppendLine( "\t<EscapePoint>");
                sb.AppendLine($"\t\t<Row>{End.x}</Row>");
                sb.AppendLine($"\t\t<Column>{End.y}</Column>");
                sb.AppendLine( "\t</EscapePoint>");
                sb.AppendLine( "\t<InsideItems>");
                foreach(Point point in Walls)
                {
                    sb.AppendLine("\t\t<Wall>");
                    sb.AppendLine($"\t\t\t<Row>{point.x}</Row>");
                    sb.AppendLine($"\t\t\t<Column>{point.y}</Column>");
                    sb.AppendLine("\t\t</Wall>");
                }
                foreach (Point point in Traps)
                {
                    sb.AppendLine("\t\t<Trap>");
                    sb.AppendLine($"\t\t\t<Row>{point.x}</Row>");
                    sb.AppendLine($"\t\t\t<Column>{point.y}</Column>");
                    sb.AppendLine("\t\t</Trap>");
                }
                sb.AppendLine( "\t</InsideItems>");
                sb.AppendLine( "</Maze>");
                return sb.ToString();
            }
        }
    }
}