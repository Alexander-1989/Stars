namespace Stars.Source
{
    internal class Star
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Star() : this(0, 0, 0) { }

        public Star(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}