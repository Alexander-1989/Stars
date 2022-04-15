using Stars.Media;
using Stars.Source;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Stars
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            normalSize = Size;
            fullSize = Screen.PrimaryScreen.Bounds.Size;
            formPosition = Location;
            defPosition = new Point(0, 0);
            MouseWheel += Form1_MouseWheel;
        }

        private enum Direction
        {
            None,
            Up,
            Down,
            Left,
            Right,
            RotationUp,
            RotationDown,
            RotationLeft,
            RotationRight
        }

        private Direction way = Direction.None;
        private Graphics graphics = null;
        private Size normalSize, fullSize;
        private Point formPosition, oldMousePosition, defPosition;
        private int flyTime = 0;
        private int changeWayTime = 50;
        private int interval = 300;
        private int speed = 5;
        private int showMouseTime = 0;
        private bool shake = false;
        private bool isFullSize = false;
        private readonly Star[] stars = new Star[15000];
        private readonly Media_Player player = new Media_Player();
        private readonly Random random = new Random();

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                player.Volume += 100;
            }
            else
            {
                player.Volume -= 100;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            flyTime++;
            graphics?.Clear(Color.Black);

            foreach (Star star in stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            if (flyTime > interval - changeWayTime)
            {
                if (shake || way == Direction.None)
                {
                    way = (Direction)(shake ? random.Next(1, 5) : random.Next(1, 9));
                }
            }

            if (flyTime > interval)
            {
                way = Direction.None;
                speed = random.Next(-1, 20);
                flyTime = 0;
                changeWayTime = random.Next(20, 60);
                interval = 100 * random.Next(1, 15);
                shake = random.Next(100) > 70;
            }

            pictureBox1.Invalidate();
        }

        private float Map(float n, float start1, float stop1, float start2, float stop2)
        {
            return ((n - start1) * (stop2 - start2) / (stop1 - start1)) + start2;
        }

        private void MoveStar(Star star)
        {
            double alpha = 2;
            float step = 10;

            switch (way)
            {
                case Direction.Up:
                    star.Y += step;
                    break;
                case Direction.Down:
                    star.Y -= step;
                    break;
                case Direction.Left:
                    star.X += step;
                    break;
                case Direction.Right:
                    star.X -= step;
                    break;
                case Direction.RotationUp:
                    star.Y = (float)(star.X * Math.Sin(AngleToRadians(alpha)) + star.Y * Math.Cos(AngleToRadians(alpha)));
                    break;
                case Direction.RotationDown:
                    star.Y = (float)(-star.X * Math.Sin(AngleToRadians(alpha)) + star.Y * Math.Cos(AngleToRadians(alpha)));
                    break;
                case Direction.RotationLeft:
                    star.X = (float)(star.X * Math.Cos(AngleToRadians(alpha)) - star.Y * Math.Sin(AngleToRadians(alpha)));
                    break;
                case Direction.RotationRight:
                    star.X = (float)(star.X * Math.Cos(AngleToRadians(alpha)) + star.Y * Math.Sin(AngleToRadians(alpha)));
                    break;
            }

            star.Z -= speed;
            if (star.Z < 0)
            {
                star.X = random.Next(-Width, Width);
                star.Y = random.Next(-Height, Height);
                star.Z = random.Next(0, Width);
            }
        }

        private void DrawStar(Star star)
        {

            float size = Map(star.Z, 0, Width, 5, 0);
            float x = Map(star.X / star.Z, 0, 1, 0, Width) + (Width / 2);
            float y = Map(star.Y / star.Z, 0, 1, 0, Height) + (Height / 2);

            byte R = (byte)Map(star.Z, 0, Width, 255, 0);
            byte G = 255;
            byte B = 255;

            using (SolidBrush solidBrush = new SolidBrush(Color.FromArgb(R, G, B)))
            {
                graphics.FillEllipse(solidBrush, x, y, size, size);
            }
        }

        private double AngleToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i] = new Star()
                {
                    X = random.Next(-Width, Width),
                    Y = random.Next(-Height, Height),
                    Z = random.Next(1, Width),
                };
            }

            ChangeSize();
            timer1.Start();
            player.Open(Path.Combine(Environment.CurrentDirectory, "Music\\music.mp3"));
            player.Play();
        }

        private void SetSize(Point location, Size size, bool showCursor)
        {
            Location = location;
            Size = size;
            NativeMethods.ShowCursor(showCursor);
        }

        private void ChangeSize()
        {
            if (isFullSize)
            {
                SetSize(formPosition, normalSize, true);
            }
            else
            {
                formPosition = Location;
                SetSize(defPosition, fullSize, false);
            }

            isFullSize = !isFullSize;

            graphics?.Dispose();
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyData == Keys.F) || (e.Alt && e.KeyCode == Keys.Enter))
            {
                ChangeSize();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Up)
            {
                player.Volume += 100;
                return;
            }

            if (e.Control && e.KeyCode == Keys.Down)
            {
                player.Volume -= 100;
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (timer1.Enabled)
                    {
                        timer1.Stop();
                        player.Pause();
                    }
                    else
                    {
                        timer1.Start();
                        player.Play();
                    }
                    break;
                case Keys.M:
                    player.Mute();
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.N:
                    NativeMethods.ShowCursor(false);
                    break;
                case Keys.B:
                    NativeMethods.ShowCursor(true);
                    break;
                case Keys.G:
                    way = (Direction)random.Next(1, 5);
                    break;
                case Keys.Up:
                    way = Direction.Up;
                    break;
                case Keys.Down:
                    way = Direction.Down;
                    break;
                case Keys.Left:
                    way = Direction.Left;
                    break;
                case Keys.Right:
                    way = Direction.Right;
                    break;
                case Keys.W:
                    way = Direction.RotationUp;
                    break;
                case Keys.S:
                    way = Direction.RotationDown;
                    break;
                case Keys.A:
                    way = Direction.RotationRight;
                    break;
                case Keys.D:
                    way = Direction.RotationLeft;
                    break;
                case Keys.Oemplus:
                    speed++;
                    break;
                case Keys.OemMinus:
                    speed--;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            way = Direction.None;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            ChangeSize();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isFullSize && e.Button == MouseButtons.Left)
            {
                oldMousePosition = e.Location;
            }
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isFullSize && !timer2.Enabled)
            {
                NativeMethods.ShowCursor(true);
                timer2.Start();
            }

            if (e.Button == MouseButtons.Left && !isFullSize)
            {
                int dx = e.Location.X - oldMousePosition.X;
                int dy = e.Location.Y - oldMousePosition.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (showMouseTime < 5)
            {
                showMouseTime++;
            }
            else
            {
                timer2.Stop();
                showMouseTime = 0;
                NativeMethods.ShowCursor(false);
            }
        }
    }
}