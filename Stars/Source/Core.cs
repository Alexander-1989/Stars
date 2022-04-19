namespace Stars
{
    internal enum Direction
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

    internal struct Line
    {
        public float Start { get; set; }
        public float End { get; set; }

        public Line(float start, float end)
        {
            Start = start;
            End = end;
        }

        public float Length => End - Start;
    }
}