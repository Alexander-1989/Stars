namespace Stars.Source
{
    class Star
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Star() { }

        public Star(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}