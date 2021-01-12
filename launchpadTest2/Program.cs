using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LP;

namespace launchpadTest2
{
    class Program
    {
        private static Interface lp;
        static void Main(string[] args)
        {
            lp = new Interface();

            lp.connect(lp.getConnectedLaunchpads()[0]);

            lp.OnLaunchpadKeyPressed += new Interface.LaunchpadKeyEventHandler(btnPressEvent);
            lp.OnLaunchpadCCKeyPressed += new Interface.LaunchpadCCKeyEventHandler(ctrPressEvent);
            Console.WriteLine("游玩贪吃蛇请输入tcs 退出请输入quit 清空光效请输入clear");

            while (true)
            {
                string str = Console.ReadLine();
                if (str.Equals("quit"))
                {
                    break;
                }
                else if (str.Equals("tcs"))
                {
                    clearLight();
                    tcs t = new tcs(lp);
                }
                else if (str.StartsWith("test"))
                {
                    test(int.Parse(str.Substring(4)));
                }
                else if (str.Equals("clear"))
                {
                    clearLight();
                }
                else
                {
                    string[] strs = str.Split(' ');
                    show(int.Parse(strs[0]), int.Parse(strs[1]), int.Parse(strs[2]));
                }
            }
            clearLight();
            lp.disconnect(lp.getConnectedLaunchpads()[0]);
        }
        private static void clearLight()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    show(j, i, 0);
                }
            }
        }
        private static void test(int start)
        {
            int cnt = start;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    show(j, i, cnt);
                    cnt++;
                }
            }
        }

        public static void ctrPressEvent(object sender, Interface.LaunchpadCCKeyEventArgs e)
        {
            //Console.WriteLine("Control: " + e.GetVal().ToString() + " " + (e.isPressing() ? "in" : "out"));
        }

        public static void btnPressEvent(object sender, Interface.LaunchpadKeyEventArgs e)
        {
            //Console.WriteLine("X: " + e.GetY() + " ,Y: " + e.GetX() + " " + (e.isPressing() ? "in" : "out"));
        }
        private static void show(int x, int y, int vol)
        {
            lp.setLED(y, x, vol);
        }
    }
    class tcs
    {
        class Pos
        {
            public int x;
            public int y;
            public Pos(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
            public Pos padd(int _x,int _y)
            {
                x += _x;
                y += _y;
                return this;
            }
            public Pos set(Pos p)
            {
                x = p.x;
                y = p.y;
                return this;
            }
            public Pos set(int _x,int _y)
            {
                x = _x;
                y = _y;
                return this;
            }
            public Pos clone()
            {
                return new Pos(x, y);
            }
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj.GetType().Equals(this.GetType()) == false) return false;
                Pos p = (Pos)obj;
                if (p.x == this.x && p.y == this.y) return true;
                return false;
            }
            public override int GetHashCode()
            {
                return x * 10 + y;
            }
        }
        private int[,] dir = new int[8, 8]
        {
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 2, 0, 1, 1, 1, 1, 0, 3 },
            { 2, 2, 0, 1, 1, 0, 3, 3 },
            { 2, 2, 2, 0, 0, 3, 3, 3 },
            { 2, 2, 2, 0, 0, 3, 3, 3 },
            { 2, 2, 0, 4, 4, 0, 3, 3 },
            { 2, 0, 4, 4, 4, 4, 0, 3 },
            { 0, 4, 4, 4, 4, 4, 4, 0 }
        };
        private enum Direction
        {
            UP, DOWN, LEFT, RIGHT, NONE
        }
        private Direction getDirection(Pos pos)
        {
            switch (dir[pos.x, pos.y])
            {
                case 1:
                    return Direction.UP;
                case 2:
                    return Direction.LEFT;
                case 3:
                    return Direction.RIGHT;
                case 4:
                    return Direction.DOWN;
                default:
                    return Direction.UP;
            }
        }

        private Interface lp;

        private void show(int x,int y,int vol)
        {
            lp.setLED(y, x, vol);
        }
        private void show(Pos pos,int vol)
        {
            lp.setLED(pos.y, pos.x, vol);
        }
        private Random rd;
        private Pos scr;
        private Direction Sdir;
        private Direction Ldir;
        private Pos head;
        private Pos getRandom()
        {
            return new Pos(rd.Next(0, 8), rd.Next(0, 8));
        }
        private void init()
        {
            loc = false;
            show(3, 4, 3);
            show(3, 3, 3);
            q = new Queue<Pos>();
            q.Enqueue(new Pos(3, 4));
            q.Enqueue(new Pos(3, 3));
            rd = new Random();
            scr = getRandom();
            while (q.Contains(scr))
            {
                scr.set(getRandom());
            }
            show(scr, 19);
            Sdir = Direction.UP;
            Ldir = Direction.UP;
            head = new Pos(3, 3);

        }
        private bool loc;
        private Queue<Pos> q;
        private int score = 0;
        private Pos getNear(Pos pos,Direction _d)
        {
            //Console.WriteLine("getNextOf (" + pos.x + "," + pos.y + ")");
            switch (_d)
            {
                case Direction.UP:
                    return pos.padd(0, -1);
                case Direction.DOWN:
                    return pos.padd(0, 1);
                case Direction.LEFT:
                    return pos.padd(-1, 0);
                case Direction.RIGHT:
                    return pos.padd(1, 0);
                default:
                    return pos.padd(0, -1);
            }
        }
        public void onChangeDir(object sender,Interface.LaunchpadKeyEventArgs e)
        {
            if (loc) return;
            Pos pos = new Pos(e.GetX(), e.GetY());
            if ((Ldir == Direction.UP && getDirection(pos) == Direction.DOWN) || (Ldir == Direction.DOWN && getDirection(pos) == Direction.UP) || (Ldir == Direction.LEFT && getDirection(pos) == Direction.RIGHT) || (Ldir == Direction.RIGHT && getDirection(pos) == Direction.LEFT)) return;
            Sdir = getDirection(pos);
        }
        private void run()
        {
            int dur = 1000;
            int startTime = Environment.TickCount;
            while (true)
            {
                if (Math.Abs(Environment.TickCount - startTime) >= dur)
                {
                    loc = true;
                    Pos next = getNear(head.clone(), Sdir);
                    Ldir = Sdir;
                    //Console.WriteLine("(" + head.x + "," + head.y + ") (" + next.x + "," + next.y + ") (" + scr.x + "," + scr.y + ")");
                    if (q.Contains(next)) return;
                    if (next.x < 0 || next.x > 7 || next.y < 0 || next.y > 7) return;
                    if (!next.Equals(scr))
                    {
                        show(q.Dequeue(), 0);
                    }
                    else
                    {
                        score++;
                        dur -= 20;

                        scr.set(getRandom());
                        while (q.Contains(scr))
                        {
                            scr.set(getRandom());
                        }
                        show(scr, 19);
                    }
                    show(next, 3);
                    q.Enqueue(next);
                    head.set(next);

                    startTime = Environment.TickCount;
                    loc = false;
                }
            }
        }



        public tcs(Interface _lp)
        {
            lp = _lp;
            
            init();
            lp.OnLaunchpadKeyPressed += new Interface.LaunchpadKeyEventHandler(onChangeDir);
            run();
            while (q.Count > 0)
            {
                show(q.Dequeue(), 1);
            }
            Console.WriteLine("score: " + score);
            Console.WriteLine("再次游玩请输入\"tcs\"");
        }
    }
}
