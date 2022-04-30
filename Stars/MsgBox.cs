using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stars
{
    internal class MsgBox : Form
    {
        private readonly Timer _timer;
        private readonly StringFormat _stringFormat;
        private readonly SolidBrush _fontBrush;
        private readonly Font _font;
        private const int maxHeight = 60;
        private string _message;
        private int _duration;
        public int Duration { get; set; }

        public new string Text
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                Invalidate();
            }
        }

        public MsgBox(IWin32Window owner) : this(owner, 80) { }

        public MsgBox(IWin32Window owner, int duration) : this(owner, duration, "") { }

        public MsgBox(IWin32Window owner, int duration, string text)
        {
            Owner = owner as Form;
            _font = new Font
                (
                base.Font.FontFamily,
                12,
                base.Font.Style,
                base.Font.Unit,
                base.Font.GdiCharSet,
                base.Font.GdiVerticalFont
                );
            _stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            _fontBrush = new SolidBrush(Color.White);
            Location = new Point(Owner.Location.X, Owner.Location.Y);
            Size = new Size(Owner.Width, maxHeight);
            BackColor = Color.FromArgb(30, 30, 30);
            FormBorderStyle = FormBorderStyle.None;
            Opacity = 0;
            Visible = true;
            ShowIcon = false;
            ShowInTaskbar = false;
            _message = text;
            _duration = Duration = duration;
            _timer = new Timer();
            _timer.Tick += new EventHandler(Tick);
            _timer.Interval = 1;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void Tick(object sender, EventArgs e)
        {
            if (_duration > 0)
            {
                _duration--;
            }
            else if (Opacity > 0)
            {
                Opacity -= 0.02;
                Location = new Point(Location.X, Location.Y - 1);
            }
            else
            {
                _timer.Stop();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Rectangle rect = new Rectangle(new Point(), Size);
            e.Graphics.DrawString(_message, _font, _fontBrush, rect, _stringFormat);
        }

        public void Show(string message)
        {
            Width = Owner.Width;
            Location = new Point(Owner.Location.X, Owner.Location.Y);
            Text = message;
            Opacity = 0.99;
            _duration = Duration;
            _timer.Start();
        }

        public new void Dispose()
        {
            _stringFormat.Dispose();
            _font.Dispose();
            _fontBrush.Dispose();
            _timer.Dispose();
            base.Dispose();
        }
    }
}