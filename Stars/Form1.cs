using System;
using System.IO;
using Stars.Source;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

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
            player.PlayCount = int.MaxValue; // Повторять 2 147 483 647 раз 
        }

        Size normal_size, full_size;
        Point form_pos, old_mouse_pos;

        Graphics graphics = null;
        Random rnd = new Random();
        MediaPlayer.MediaPlayer player = new MediaPlayer.MediaPlayer();
        Star[] stars = new Star[15000];

        uint interval = 300;
        uint time_fly = 0;
        uint time_bias = 50;
        sbyte speed = 5;
        sbyte show_mouse_time = 0;
        bool is_full_size = false;
        bool is_mute = false;
        bool shake = false;
        string bias = string.Empty;
        string[] ways =
        {
            "up",
            "down",
            "left",
            "right",
            "rotation_up",
            "rotation_down",
            "rotation_left",
            "rotation_right"
        };

        private void VolumeMusicUp()
        {
            int vol = player.Volume + 100;
            if (vol > 0) vol = 0;
            player.Volume = vol;
        }

        private void VolumeMusicDown()
        {
            int vol = player.Volume - 100;
            if (vol < -6000) vol = -6000;
            player.Volume = vol;
        }

        private void Normal_Volume()
        {
            player.Volume = 0;
        }

        private void Mute_Volume()
        {
            player.Volume = -6000;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) VolumeMusicUp();
            else if (e.Delta < 0) VolumeMusicDown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            graphics?.Clear(Color.Black);

            foreach (Star star in stars)
            {
                DrawStar(star);
                MoveStar(star);
            }

            pictureBox1.Invalidate();

            time_fly += 1;
            if (time_fly + time_bias > interval)
            {
                if (shake || bias == string.Empty)
                {
                    int n = shake ? rnd.Next(4) : ways.Length;
                    bias = ways[rnd.Next(n)];
                }
            }
            if (time_fly > interval)
            {
                bias = string.Empty;
                speed = (sbyte)rnd.Next(-1, 20);
                time_fly = 0;
                time_bias = (uint)rnd.Next(20, 60);
                interval = (uint)(100 * rnd.Next(1, 15));
                shake = rnd.Next(100) > 75 ? true : false;
            }
        }

        private void MoveStar(Star star)
        {
            double fi = 2;
            float step = 10;

            switch (bias)
            {
                case "up":
                    star.Y += step;
                    break;
                case "down":
                    star.Y -= step;
                    break;
                case "left":
                    star.X += step;
                    break;
                case "right":
                    star.X -= step;
                    break;
                case "rotation_up":
                    star.Y = (float)(star.X * Math.Sin(AngleToRadians(fi)) + star.Y * Math.Cos(AngleToRadians(fi)));
                    break;
                case "rotation_down":
                    star.Y = (float)(-star.X * Math.Sin(AngleToRadians(fi)) + star.Y * Math.Cos(AngleToRadians(fi)));
                    break;
                case "rotation_left":
                    star.X = (float)(star.X * Math.Cos(AngleToRadians(fi)) - star.Y * Math.Sin(AngleToRadians(fi)));
                    break;
                case "rotation_right":
                    star.X = (float)(star.X * Math.Cos(AngleToRadians(fi)) + star.Y * Math.Sin(AngleToRadians(fi)));
                    break;
            }

            star.Z -= speed;
            if (star.Z < 0)
            {
                star.X = rnd.Next(-Width, Width);
                star.Y = rnd.Next(-Height, Height);
                star.Z = rnd.Next(1, Width);
            }
        }

        private void DrawStar(Star star)
        {
            if (graphics == null) return;
            int _SIZE = 5;
            float size = Map(star.Z, 0, Width, _SIZE, 0);
            float x = Map(star.X / star.Z, 0, 1, 0, Width) + Width / 2;
            float y = Map(star.Y / star.Z, 0, 1, 0, Height) + Height / 2;
            float color = Map(star.Z, 0, Width, 255, 0); if (color < 0) color = 0;

            SolidBrush sb = new SolidBrush(Color.FromArgb((int)color, 255, 255));
            graphics.FillEllipse(sb, x, y, size, size);
            sb.Dispose();
        }

        private double AngleToRadians(double angle)
        {
            return angle * Math.PI / 180;
        }

        private float Map(float n, float start_1, float stop_1, float start_2, float stop_2)
        {
            return ((n - start_1) * (stop_2 - start_2) / (stop_1 - start_1)) + start_2;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const string sound_file = ".\\Music\\music.mp3";
            if (File.Exists(sound_file))
            {
                player.Open(sound_file);
                player.Play();
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
            show_mouse_time = 0;

            if (is_full_size)
            {
                Location = form_pos;
                Size = normal_size;
                is_full_size = false;
                timer2.Stop();
                NativeMethods.ShowCursor(true);
            }
            else
            {
                form_pos = Location;
                Location = new Point(0, 0);
                Size = full_size;
                is_full_size = true;
                timer2.Start();
            }

            pictureBox1.Image?.Dispose();
            graphics?.Dispose();
            pictureBox1.Image = new Bitmap(Width, Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Enter)
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
                    if (timer1.Enabled)
                    {
                        timer1.Stop();
                        if (player.PlayState == MediaPlayer.MPPlayStateConstants.mpPlaying) player.Pause();
                    }
                    else
                    {
                        timer1.Start();
                        if (player.PlayState != MediaPlayer.MPPlayStateConstants.mpPlaying) player.Play();
                    }
                    break;
                case Keys.Escape:
                    Application.Exit();
                    break;
                case Keys.F:
                    ChangeSize();
                    break;
                case Keys.M:
                    if (is_mute) Normal_Volume();
                    else Mute_Volume();
                    is_mute = !is_mute;
                    break;
                case Keys.N:
                    NativeMethods.ShowCursor(false);
                    break;
                case Keys.B:
                    NativeMethods.ShowCursor(true);
                    break;
                case Keys.G:
                    bias = ways[rnd.Next(4)];
                    break;
                case Keys.Up:
                    bias = "up";
                    break;
                case Keys.Down:
                    bias = "down";
                    break;
                case Keys.Left:
                    bias = "left";
                    break;
                case Keys.Right:
                    bias = "right";
                    break;
                case Keys.W:
                    bias = "rotation_up";
                    break;
                case Keys.S:
                    bias = "rotation_down";
                    break;
                case Keys.A:
                    bias = "rotation_right";
                    break;
                case Keys.D:
                    bias = "rotation_left";
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            bias = string.Empty;
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            ChangeSize();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !is_full_size)
            {
                old_mouse_pos = e.Location;
            }

            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(MousePosition);
            }

            if (is_full_size && !timer2.Enabled)
            {
                show_mouse_time = 0;
                timer2.Start();
                NativeMethods.ShowCursor(true);
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (is_full_size && !timer2.Enabled)
            {
                show_mouse_time = 0;
                timer2.Start();
                NativeMethods.ShowCursor(true);
            }

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
                System.Media.SystemSounds.Beep.Play();
                timer2.Stop();
                show_mouse_time = 0;
                NativeMethods.ShowCursor(false);
            }

            show_mouse_time += 1;
        }
    }

    static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "ShowCursor")]
        internal static extern int ShowCursor(bool show);
    }
}