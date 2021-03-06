using System.Text;

class Program
{
    private const int ScreenWidth = 180;
    private const int ScreenHeight = 70;

    private const int MapWidth = 32;
    private const int MapHeight = 32;

    private const double Fov = Math.PI / 3;
    private const double Depth = 20;

    private static double _playerX = 20;
    private static double _playerY = 20;
    private static double _playerA = 0;

    public static StringBuilder _map = new StringBuilder();

    private static readonly char[] Screen = new char[ScreenWidth * ScreenHeight];

    static async Task Main(string[] args)
    {

        SetScreen();
        DateTime dataTimeFrom = DateTime.Now;

        while (true)
        {
            SetMap();
            DateTime dataTimeTo = DateTime.Now;
            double elapsedTime = (dataTimeTo - dataTimeFrom).TotalSeconds;
            dataTimeFrom = DateTime.Now;

            if (Console.KeyAvailable)
            {
                ConsoleKey consoleKey = Console.ReadKey(true).Key;
                switch (consoleKey)
                {
                    case ConsoleKey.A:
                        _playerA += 8 * elapsedTime;
                        break;
                    case ConsoleKey.D:
                        _playerA -= 8 * elapsedTime;
                        break;
                    case ConsoleKey.W:
                        {
                            _playerX += Math.Sin(_playerA) * 10 * elapsedTime;
                            _playerY += Math.Cos(_playerA) * 10 * elapsedTime;

                            if (_map[(int)_playerY * MapWidth + (int)_playerX] == '#')
                            {
                                _playerX -= Math.Sin(_playerA) * 10 * elapsedTime;
                                _playerY -= Math.Cos(_playerA) * 10 * elapsedTime;
                            }
                            break;
                        }
                    case ConsoleKey.S:
                        {
                            _playerX -= Math.Sin(_playerA) * 10 * elapsedTime;
                            _playerY -= Math.Cos(_playerA) * 10 * elapsedTime;


                            if (_map[(int)_playerY * MapWidth + (int)_playerX] == '#')
                            {
                                _playerX += Math.Sin(_playerA) * 10 * elapsedTime;
                                _playerY += Math.Cos(_playerA) * 10 * elapsedTime;
                            }

                            break;
                        }

                }
            }

            var rayCastingtasks = new List<Task<Dictionary<int, char>>>();

            for (int x = 0; x < ScreenWidth; x++)
            {
                int x1 = x;
                rayCastingtasks.Add(Task.Run(() => CastRay(x1)));
            }

            var rays = await Task.WhenAll(rayCastingtasks);

            foreach (Dictionary<int, char> dictionary in rays)
            {
                foreach (int key in dictionary.Keys)
                {
                    Screen[key] = dictionary[key];
                }
            }



            char[] stats = $"X: {_playerX}, Y: {_playerY}, A: {_playerA}, FPS: {(int)(1 / elapsedTime)}".ToCharArray();
            stats.CopyTo(Screen, 0);

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    Screen[(y + 1) * ScreenWidth + x] = _map[y * MapWidth + x];
                }
            }

            Screen[(int)(_playerY + 1) * ScreenWidth + (int)_playerX] = 'P';

            Console.SetCursorPosition(0, 0);
            Console.Write(Screen);
        }
    }

    static public void SetScreen()
    {
        Console.SetWindowSize(ScreenWidth, ScreenHeight);
        Console.SetBufferSize(ScreenWidth, ScreenHeight);
        Console.CursorVisible = false;
    }

    static public void SetMap()
    {
        _map.Clear();
        _map.Append("################################");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("##########................#....#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("#........#.....................#");
        _map.Append("##########.................#...#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("#..............................#");
        _map.Append("################################");
    }

    public static Dictionary<int, char> CastRay(int x)
    {
        var result = new Dictionary<int, char>();
        double rayAngle = _playerA + Fov / 2 - x * Fov / ScreenWidth;

        double rayX = Math.Sin(rayAngle);
        double rayY = Math.Cos(rayAngle);

        double distanceToWall = 0;
        bool hitWall = false;
        bool isBound = false;

        while (!hitWall && distanceToWall < Depth)
        {
            distanceToWall += 0.1;

            int testX = (int)(_playerX + rayX * distanceToWall);
            int testY = (int)(_playerY + rayY * distanceToWall);

            if (testX < 0 || testX >= Depth + _playerX || testY < 0 || testY >= Depth + _playerY)
            {
                hitWall = true;
                distanceToWall = Depth;
            }
            else
            {
                char testCell = _map[testY * MapWidth + testX];

                if (testCell == '#')
                {
                    hitWall = true;

                    var boundsVectorList = new List<(double module, double cos)>();

                    for (int tx = 0; tx < 2; tx++)
                    {
                        for (int ty = 0; ty < 2; ty++)
                        {
                            double vx = testX + tx - _playerX;
                            double vy = testY + ty - _playerY;

                            double vectorModule = Math.Sqrt(vx * vx + vy * vy);
                            double cosAngle = rayX * vx / vectorModule + rayY * vy / vectorModule;

                            boundsVectorList.Add((vectorModule, cosAngle));
                        }
                    }

                    boundsVectorList = boundsVectorList.OrderBy(v => v.module).ToList();

                    double boundAngle = 0.03 / distanceToWall;

                    if (Math.Acos(boundsVectorList[0].cos) < boundAngle ||
                       Math.Acos(boundsVectorList[1].cos) < boundAngle)
                    {
                        isBound = true;
                    }
                }
                else
                {
                    _map[testY * MapWidth + testX] = '*';
                }
            }
        }

        int celling = (int)(ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
        int floor = ScreenHeight - celling;

        char wallShade = ' ';
        if (isBound)
            wallShade = '|';
        else if (distanceToWall <= Depth / 4d)
            wallShade = '\u2588';
        else if (distanceToWall <= Depth / 3d)
            wallShade = '\u2593';
        else if (distanceToWall <= Depth / 2d)
            wallShade = '\u2592';
        else if (distanceToWall <= Depth)
            wallShade = '\u2591';

        for (int y = 0; y < ScreenHeight; y++)
        {
            if (y <= celling)
            {
                result[y * ScreenWidth + x] = ' ';
            }
            else if (y > celling && y <= floor)
            {
                result[y * ScreenWidth + x] = wallShade;
            }
            else
            {
                char floorShade;

                double b = 1 - (y - ScreenHeight / 2d) / (ScreenHeight / 2d);

                if (b < 0.25)
                    floorShade = '#';
                else if (b < 0.5)
                    floorShade = 'x';
                else if (b < 0.75)
                    floorShade = '-';
                else if (b < 0.9)
                    floorShade = '.';
                else
                    floorShade = ' ';

                result[y * ScreenWidth + x] = floorShade;
            }
        }

        return result;
    }
}