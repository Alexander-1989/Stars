﻿using System;
using Stars.Media;
using Stars.Source;
using System.Drawing;
using System.Windows.Forms;

namespace Stars
{
    public partial class Form1 : Form
    {
        internal enum Direction : byte
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
        private Graphics graphics;
        private Direction way;
        private Size fullSize;
        private Size normalSize;
        private Point formPosition;
        private Point oldMousePosition;
        private const int angle = 2;
        private const int step = 10;
        private const int starsCount = 15000;
        private const int showMouseInterval = 5;
        private const int volumeIncrement = 5;
        private int speed = 0;
        private int flyInterval = 0;
        private int changeWayInterval = 0;
        private int fullInterval = 0;
        private int showMouseSeconds = 0;
        private bool shaking = false;
        private bool isFullSize = false;
        private readonly MsgBox message = null;
        private readonly Random random = new Random();
        private readonly Star[] stars = new Star[starsCount];
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        private readonly string musicFileName = Application.StartupPath + "\\Music\\music.mp3";
        private const string info =
            "F1 - Help\nEscape - Close Application\n" +
            "Space - Start or Stop\nM - Mute\nN - Max Volume\nB - Hide Cursor\nV - Show Cursor\n" +
            "G - Gravity\nUp - Up\nDown - Down\nLeft - Left\nRight - Right\n" +
            "W - Rotation Up\nS - Rotation Down\nA - Rotation Right\nD - Rotation Left\n" +
            "OemPlus - Speed Up\nOemMinus - Speed Down";

        public Form1()
        {
            InitializeComponent();
            normalSize = Size;
            fullSize = Screen.PrimaryScreen.Bounds.Size;
            formPosition = Location;
            message = new MsgBox(this, 60);
            MouseWheel += Form1_MouseWheel;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                audioPlayer.Volume += volumeIncrement;
            }
            else if (e.Delta < 0)
            {
                audioPlayer.Volume -= volumeIncrement;
            }
        }

        private void FlyTimer_Tick(object sender, EventArgs e)
        {
            flyInterval++;
            graphics?.Clear(Color.Black);

            foreach (Star star in stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            if (flyInterval + changeWayInterval > fullInterval)
            {
                if (shaking)
                {
                    way = (Direction)random.Next(1, 5);
                }
                else if (way == Direction.None)
                {
                    way = (Direction)random.Next(1, 9);
                }
            }

            if (flyInterval > fullInterval)
            {
                flyInterval = 0;
                way = Direction.None;
                speed = random.Next(-1, 20);
                changeWayInterval = random.Next(20, 60);
                fullInterval = random.Next(100, 1500);
                shaking = random.Next(100) > 70;
            }

            pictureBox1.Invalidate();
        }

        private float Map(float position, float startA, float endA, float startB, float endB)
        {
            return ((position - startA) * (endB - startB) / (endA - startA)) + startB;
        }

        private void InitStars(Star[] stars)
        {
            for (int i = 0; i < stars.Length; i++)
            {
                int x = random.Next(-Width, Width);
                int y = random.Next(-Height, Height);
                int z = random.Next(1, Width);
                stars[i] = new Star(x, y, z);
            }
        }

        private void MoveStar(Star star)
        {
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
                star.Z = random.Next(1, Width);
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

            Color color = Color.FromArgb(R, G, B);
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics?.FillEllipse(brush, x, y, size, size);
            }
        }

        private double DegreesToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                audioPlayer.Open(musicFileName);
                audioPlayer.Play();
                audioPlayer.Notify += PlayerNotify;
            }
            catch (Exception) { }

            InitStars(stars);
            ChangeSize();
            flyTimer.Start();
        }

        private void PlayerNotify(object sender, AudioPlayerEventArgs e)
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
                showMouseSeconds = 0;
                SetSize(formPosition, normalSize, true);
            }
            else
            {
                formPosition = Location;
                SetSize(new Point(0, 0), fullSize, false);
            }

            isFullSize = !isFullSize;
            graphics?.Dispose();
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            Focus();
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
                audioPlayer.Volume += volumeIncrement;
                return;
            }

            if (e.Control && e.KeyCode == Keys.Down)
            {
                audioPlayer.Volume -= volumeIncrement;
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
                case Keys.F1:
                    MessageBox.Show(info);
                    NativeMethods.ShowCursor(true);
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.Space:
                    if (flyTimer.Enabled)
                    {
                        flyTimer.Stop();
                        audioPlayer.Pause();
                    }
                    else
                    {
                        flyTimer.Start();
                        audioPlayer.Play();
                    }
                    break;
                case Keys.M:
                    audioPlayer.Mute();
                    break;
                case Keys.N:
                    audioPlayer.SetMaxVolume();
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
                case Keys.C:
                    way = Direction.None;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            way = Direction.None;
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            ChangeSize();
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!isFullSize && e.Button == MouseButtons.Left)
            {
                oldMousePosition = e.Location;
            }
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
                NativeMethods.ShowCursor(true);
            }
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isFullSize && !mouseTimer.Enabled)
            {
                mouseTimer.Start();
                NativeMethods.ShowCursor(true);  
            }

            if (e.Button == MouseButtons.Left && !isFullSize)
            {
                int dx = e.Location.X - oldMousePosition.X;
                int dy = e.Location.Y - oldMousePosition.Y;
                Location = new Point(Location.X + dx, Location.Y + dy);
            }
        }

        private void MouseTimer_Tick(object sender, EventArgs e)
        {
            if (showMouseSeconds < showMouseInterval)
            {
                showMouseSeconds++;
            }
            else
            {
                mouseTimer.Stop();
                showMouseSeconds = 0;
                NativeMethods.ShowCursor(false);
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}