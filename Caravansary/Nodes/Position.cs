namespace Caravansary
{
    public class Position : ObservableObject
    {
        private float _x;

        public float X
        {
            get { return _x; }
            set { _x = value; OnPropertyChanged(nameof(X)); }
        }

        private float _y;

        public float Y
        {
            get { return _y; }
            set { _y = value; OnPropertyChanged(nameof(Y)); }
        }

        public Position(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}