using System;
using System.IO;
using Stars.Source;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
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
        Random rnd = new Random();
        Size normal_size, full_size;
        Point form_pos, old_mouse_pos;
        Star[] stars = new Star[15000];
        uint time_fly = 0;
        uint time_bias = 50;
        uint interval = 300;
        sbyte speed = 5;
        sbyte show_mouse_time = 0;
        bool is_full_size = false;
        bool is_mute = false;
        bool shake = false;
        Direction way = Direction.None;
        Media.MediaPlayer mPlayer = new Media.MediaPlayer();

        private void VolumeMusicUp()
        {
            int vol = mPlayer.Volume + 100;
            if (vol > 0) vol = 0;
            mPlayer.Volume = vol;
        }

        private void VolumeMusicDown()
        {
            int vol = mPlayer.Volume - 100;
            if (vol < -6000) vol = -6000;
            mPlayer.Volume = vol;
        }

        private void Normal_Volume()
        {
            mPlayer.Volume = 0;
        }

        private void Mute_Volume()
        {
            mPlayer.Volume = -6000;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                VolumeMusicUp();
            }
            else if (e.Delta < 0)
            {
                VolumeMusicDown();
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
            time_fly++;

            if (time_fly + time_bias > interval)
            {
                if (shake || way == Direction.None)
                {
                    way = (Direction)(shake ? rnd.Next(1, 5) : rnd.Next(1, 9));
                }
            }

            if (time_fly > interval)
            {
                way = Direction.None;
                speed = (sbyte)rnd.Next(-1, 20);
                time_fly = 0;
                time_bias = (uint)rnd.Next(20, 60);
                interval = (uint)(100 * rnd.Next(1, 15));
                shake = rnd.Next(100) > 75 ? true : false;
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

        private float Map(float n, float start_1, float stop_1, float start_2, float stop_2)
        {
            return start_2 + ((n - start_1) * (stop_2 - start_2) / (stop_1 - start_1));
        }

        private void DrawStar(Star star)
        {
            if (graphics == null) return;
            int _SIZE = 5;

            float size = Map(star.Z, 0, Width, _SIZE, 0);
            float x = Map(star.X / star.Z, 0, 1, 0, Width)  + Width / 2;
            float y = Map(star.Y / star.Z, 0, 1, 0, Height) + Height / 2;

            byte R = (byte)Map(star.Z, 0, Width, 255, 0);
            byte G = 255;
            byte B = 255;

            using (SolidBrush sb = new SolidBrush(Color.FromArgb(R, G, B)))
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
            const string sound_file = ".\\Music\\music.mp3";
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

        private void ChangeSize()
        {
            if (is_full_size)
            {
                Location = form_pos;
                Size = normal_size;
                is_full_size = false;
                NativeMethods.ShowCursor(true);
            }
            else
            {
                form_pos = Location;
                Location = new Point(0, 0);
                Size = full_size;
                is_full_size = true;
                NativeMethods.ShowCursor(false);
            }

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
                VolumeMusicUp();
                return;
            }

            if (e.Control && e.KeyCode == Keys.Down)
            {
                VolumeMusicDown();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Space:
                    {
                        if (timer1.Enabled)
                        {
                            timer1.Stop();
                            if (mPlayer.PlayState == Media.MPPlayStateConstants.mpPlaying)
                            {
                                mPlayer.Pause();
                            }
                        }
                        else
                        {
                            timer1.Start();
                            if (mPlayer.PlayState != Media.MPPlayStateConstants.mpPlaying)
                            {
                                mPlayer.Play();
                            }
                        }
                    }
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.M:
                    if (is_mute)
                    {
                        Normal_Volume();
                    }
                    else
                    {
                        Mute_Volume();
                    }
                    is_mute = !is_mute;
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
            if (!is_full_size)
            {
                if (e.Button == MouseButtons.Left)
                {
                    old_mouse_pos = e.Location;
                }

                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Show(MousePosition);
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !is_full_size)
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
            if (show_mouse_time > 3)
            {
                timer2.Stop();
                show_mouse_time = 0;
                NativeMethods.ShowCursor(false);
            }
            else
            {
                show_mouse_time++;
            }
        }
    }

    static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        internal static extern int ShowCursor(bool show);
    }
}