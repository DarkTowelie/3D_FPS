using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3D_FPS
{
    public class Game
    {
        private const int ScreenWidth = 100;
        private const int ScreenHeight = 50;

        private const int MapWidht = 32;
        private const int MapHeight = 32;

        private const double Fov = Math.PI / 3;
        private const double Depth = 16;

        private static double _playerX = 5;
        private static double _playerY = 5;
        private static double _playerA = 0;

        public static string _map = "";

        private static readonly char[] Screen = new char[ScreenWidth * ScreenHeight];

        public void SetScreen()
        {
            Console.SetWindowSize(ScreenWidth, ScreenHeight);
            Console.SetBufferSize(ScreenWidth, ScreenHeight);
            Console.CursorVisible = false;
        }

        public void SetMap()
        {
            _map += "################################";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "#..............................#";
            _map += "################################";
        }

        public void Logic()
        {
            SetMap();
            while (true)
            {
                _playerA += 0.003;
                for(int x = 0; x < ScreenWidth; x++)
                {
                    double rayAngle = _playerA + Fov / 2 - x * Fov / ScreenWidth;

                    double rayX = Math.Sin(rayAngle);
                    double rayY = Math.Cos(rayAngle);

                    double distanceToWall = 0;
                    bool hitWall = false;

                    while(!hitWall && distanceToWall < Depth)
                    {
                        distanceToWall += 0.1;

                        int testX = (int)(_playerX + rayX * distanceToWall);
                        int testY = (int)(_playerY + rayY * distanceToWall);

                        if(testX < 0 || testX >= Depth + _playerX || testY < 0 || testY >= Depth + _playerY)
                        {
                            hitWall = true;
                            distanceToWall = Depth;
                        }
                        else
                        {
                            char testCell = _map[testY * MapWidht + testX];

                            if(testCell == '#')
                            {
                                hitWall = true;
                            }
                        }
                    }

                    int celling = (int) (ScreenHeight / 2d - ScreenHeight * Fov / distanceToWall);
                    int floor = ScreenHeight - celling;

                    for(int y = 0; y < ScreenHeight; y++)
                    {
                        if(y <= celling)
                        {
                            Screen[y * ScreenWidth + x] = ' ';
                        }
                        else if(y > celling && y <= floor)
                        {
                            Screen[y * ScreenWidth + x] = '#';
                        }
                        else
                        {
                            Screen[y * ScreenWidth + x] = '.';
                        }
                    }

                }

                Console.SetCursorPosition(0, 0);
                Console.Write(Screen);
            }
        }
    }
}
