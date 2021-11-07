namespace Caravansary
{
    public class Shape : ObservableObject, IShape
    {
        private int _width;

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        private int _height;

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        private int borderThickness;

        public int BorderThickness
        {
            get { return borderThickness; }
            set { borderThickness = value; OnPropertyChanged(nameof(BorderThickness)); }
        }
    }
}