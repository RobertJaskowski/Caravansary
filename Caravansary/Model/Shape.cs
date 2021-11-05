namespace Caravansary
{
    public abstract class Shape : ObservableObject, IShape
    {
        private int _top;
        private int _left;

        public int Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged(nameof(Top));
            }
        }

        public int Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged(nameof(Left));
            }
        }
    }
}