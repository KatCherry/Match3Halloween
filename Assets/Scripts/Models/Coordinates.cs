namespace Match3
{
    public class Coordinates
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator == (Coordinates coordinates1, Coordinates coordinates2)
        {
            return coordinates1.X == coordinates2.X && coordinates1.Y == coordinates2.Y;
        }

        public static bool operator != (Coordinates coordinates1, Coordinates coordinates2)
        {
            return coordinates1.X != coordinates2.X || coordinates1.Y != coordinates2.Y;
        }
    }
}
