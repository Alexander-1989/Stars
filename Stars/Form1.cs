﻿using System;
using System.IO;
using Stars.Source;
using System.Drawing;
using System.Windows.Forms;
using Media = MediaPlayer;


namespace Stars
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            normal_size = Size;
            full_size = Screen.PrimaryScreen.Bounds.Size;
            form_pos = Location;
            MouseWheel += Form1_MouseWheel;
            mPlayer.PlayCount = int.MaxValue; // Повторять 2 147 483 647 раз 
        }

        enum Direction
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

        Graphics graphics = null;
        static Random rnd = new Random();
        Size normal_size, full_size;
        Point form_pos, old_mouse_pos;
        Star[] stars = new Star[15000];
        Direction way = Direction.None;
        uint timeFly = 0;
        uint timeBias = 50;
        uint period = 300;
        sbyte speed = 5;
        sbyte showMouseTime = 0;
        bool shake = false;
        bool isMute = false;
        bool isFullSize = false;
        Media.MediaPlayer mPlayer = new Media.MediaPlayer();

        private void VolumeUp()
        {
            int vol = mPlayer.Volume + 100;
            if (vol > 0) vol = 0;
            mPlayer.Volume = vol;
        }

        private void VolumeDown()
        {
            int vol = mPlayer.Volume - 100;
            if (vol < -6000) vol = -6000;
            mPlayer.Volume = vol;
        }

        private void NormalVolume()
        {
            mPlayer.Volume = 0;
        }

        private void MuteVolume()
        {
            mPlayer.Volume = -6000;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                VolumeUp();
            }
            else if (e.Delta < 0)
            {
                VolumeDown();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (graphics == null) return;
            graphics.Clear(Color.Black);

            foreach (Star star in stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            pictureBox1.Invalidate();
            timeFly++;

            if (timeFly + timeBias > period)
            {
                if (shake || way == Direction.None)
                {
                    way = (Direction)(shake ? rnd.Next(1, 5) : rnd.Next(1, 9));
                }
            }

            if (timeFly > period)
            {
                way = Direction.None;
                speed = (sbyte)rnd.Next(-1, 20);
                timeFly = 0;
                timeBias = (uint)rnd.Next(20, 60);
                period = (uint)(100 * rnd.Next(1, 15));
                shake = rnd.Next(100) > 70 ? true : false;
            }
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
                star.X = rnd.Next(-Width, Width);
                star.Y = rnd.Next(-Height, Height);
                star.Z = rnd.Next(0, Width);
            }
        }

        private float Map(float n, float start1, float stop1, float start2, float stop2)
        {
            return ((n - start1) * (stop2 - start2) / (stop1 - start1)) + start2;
        }

        private void DrawStar(Star star)
        {
            if (graphics == null) return;

            float size = Map(star.Z, 0, Width, 5, 0);
            float x = Map(star.X / star.Z, 0, 1, 0, Width) + Width / 2;
            float y = Map(star.Y / star.Z, 0, 1, 0, Height) + Height / 2;

            byte B = 255, G = 255, R = (byte)Map(star.Z, 0, Width, 255, 0);
            Color color = Color.FromArgb(R, G, B);

            using (SolidBrush sb = new SolidBrush(color))
            {
                graphics.FillEllipse(sb, x, y, size, size);
            }
        }

        private double AngleToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sound_file = ".\\Music\\music.mp3";
            if (File.Exists(sound_file))
            {
                mPlayer.Open(sound_file);
                mPlayer.Play();
            }

            for (int i = 0; i < stars.Length; ++i)
            {
                stars[i] = new Star()
                {
                    X = rnd.Next(-Width, Width),
                    Y = rnd.Next(-Height, Height),
                    Z = rnd.Next(1, Width),
                };
            }

            ChangeSize();
            timer1.Start();
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
                SetSize(form_pos, normal_size, true);
            }
            else
            {
                form_pos = Location;
                SetSize(new Point(0, 0), full_size, false);
            }

            isFullSize = !isFullSize;

            pictureBox1.Image?.Dispose();
            graphics?.Dispose();
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
                VolumeUp();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Down)
            {
                VolumeDown();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Space:
                    if (timer1.Enabled)
                    {
                        timer1.Stop();
                        mPlayer.Pause();
                    }
                    else
                    {
                        timer1.Start();
                        mPlayer.Play();
                    }
                    break;
                case Keys.M:
                    if (isMute)
                    {
                        NormalVolume();
                    }
                    else
                    {
                        MuteVolume();
                    }
                    isMute = !isMute;
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
                    way = (Direction)rnd.Next(1, 5);
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
                old_mouse_pos = e.Location;
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
                int dx = e.Location.X - old_mouse_pos.X;
                int dy = e.Location.Y - old_mouse_pos.Y;
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