using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Stars.Media;
using Stars.Source;

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
            message = new MsgBox(this, 60);
            MouseWheel += Form1_MouseWheel;
        }

        private Graphics graphics = null;
        private Direction way;
        private Size fullSize;
        private Size normalSize;
        private Point formPosition;
        private Point oldMousePosition;
        private const int starsCount = 15000;
        private int flyInterval = 0;
        private int changeWayInterval = 50;
        private int fullInterval = 300;
        private int speed = 5;
        private int showMouseInterval = 0;
        private bool shake = false;
        private bool isFullSize = false;
        private readonly Star[] stars = new Star[starsCount];
        private readonly Media_Player player = new Media_Player();
        private readonly Random random = new Random();
        private readonly MsgBox message;

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                player.Volume -= 2;
            }
            else if (e.Delta > 0)
            {
                player.Volume += 2;
            }
        }

        private void flyTimer_Tick(object sender, System.EventArgs e)
        {
            flyInterval++;
            graphics?.Clear(Color.Black);

            foreach (Star star in stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            if (flyInterval > fullInterval - changeWayInterval)
            {
                if (shake || way == Direction.None)
                {
                    way = (Direction)(shake ? random.Next(1, 5) : random.Next(1, 9));
                }
            }

            if (flyInterval > fullInterval)
            {
                way = Direction.None;
                speed = random.Next(-1, 20);
                if (speed == 0) { speed = -1; }
                flyInterval = 0;
                changeWayInterval = random.Next(20, 60);
                fullInterval = 100 * random.Next(1, 15);
                shake = random.Next(100) > 70;
            }

            pictureBox1.Invalidate();
        }

        //private float Map(float n, float start1, float stop1, float start2, float stop2)
        //{
        //    return ((n - start1) * (stop2 - start2) / (stop1 - start1)) + start2;
        //}

        private float Map(float point, Line from, Line to)
        {
            return ((point - from.Start) * to.Length / from.Length) + to.Start;
        }

        private void MoveStar(Star star)
        {
            const double angle = 2;
            const float step = 10;

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
                    star.Y = (float)(star.X * Math.Sin(DegreesToRadians(angle)) + star.Y * Math.Cos(DegreesToRadians(angle)));
                    break;
                case Direction.RotationDown:
                    star.Y = (float)(-star.X * Math.Sin(DegreesToRadians(angle)) + star.Y * Math.Cos(DegreesToRadians(angle)));
                    break;
                case Direction.RotationLeft:
                    star.X = (float)(star.X * Math.Cos(DegreesToRadians(angle)) - star.Y * Math.Sin(DegreesToRadians(angle)));
                    break;
                case Direction.RotationRight:
                    star.X = (float)(star.X * Math.Cos(DegreesToRadians(angle)) + star.Y * Math.Sin(DegreesToRadians(angle)));
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
            float size = Map(star.Z, new Line(0, Width), new Line(5, 0));
            float x = Map(star.X / star.Z, new Line(0, 1), new Line(0, Width)) + (Width / 2);
            float y = Map(star.Y / star.Z, new Line(0, 1), new Line(0, Height)) + (Height / 2);

            byte R = (byte)Map(star.Z, new Line(0, Width), new Line(255, 0));
            byte G = 255;
            byte B = 255;

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(R, G, B)))
            {
                graphics.FillEllipse(brush, x, y, size, size);
            }
        }

        private double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private void Form1_Load(object sender, System.EventArgs e)
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

            try
            {
                string fileName = Path.Combine(Environment.CurrentDirectory, "Music\\music.mp3");
                player.Open(fileName);
                player.Play();
                player.Notify += new Media_Player.PlayerEventHandler(Player_Notify);
            }
            catch (Exception) { }

            ChangeSize();
            flyTimer.Start();
        }

        private void Player_Notify(object sender, PlayerEventArgs e)
        {
            message.Show(e.Message);
        }

        private void SetSize(Point location, Size size, bool showCursor)
        {
            Size = size;
            Location = location;
            NativeMethods.ShowCursor(showCursor);
        }

        private void ChangeSize()
        {
            if (isFullSize)
            {
                mouseTimer.Stop();
                showMouseInterval = 0;
                SetSize(formPosition, normalSize, true);
            }
            else
            {
                formPosition = Location;
                SetSize(new Point(), fullSize, false);
            }

            isFullSize = !isFullSize;
            graphics?.Dispose();
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.F) || (e.Alt && e.KeyCode == Keys.Enter))
            {
                ChangeSize();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Up)
            {
                player.Volume += 5;
                return;
            }

            if (e.Control && e.KeyCode == Keys.Down)
            {
                player.Volume -= 5;
                return;
            }

            if (e.Shift && e.KeyCode == Keys.Up)
            {
                speed++;
                return;
            }

            if (e.Shift && e.KeyCode == Keys.Down)
            {
                speed--;
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Space:
                    if (flyTimer.Enabled)
                    {
                        flyTimer.Stop();
                        player.Pause();
                    }
                    else
                    {
                        flyTimer.Start();
                        player.Play();
                    }
                    break;
                case Keys.M:
                    player.Mute();
                    break;
                case Keys.N:
                    player.SetMaxVolume();
                    break;
                case Keys.B:
                    NativeMethods.ShowCursor(false);
                    break;
                case Keys.V:
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

        private void pictureBox1_DoubleClick(object sender, System.EventArgs e)
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
            if (isFullSize && !mouseTimer.Enabled)
            {
                NativeMethods.ShowCursor(true);
                mouseTimer.Start();
            }

            if (e.Button == MouseButtons.Left && !isFullSize)
            {
                int dx = e.Location.X - oldMousePosition.X;
                int dy = e.Location.Y - oldMousePosition.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void mouseTimer_Tick(object sender, System.EventArgs e)
        {
            if (showMouseInterval < 5)
            {
                showMouseInterval++;
            }
            else
            {
                mouseTimer.Stop();
                showMouseInterval = 0;
                NativeMethods.ShowCursor(false);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}