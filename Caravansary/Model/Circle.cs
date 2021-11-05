namespace Caravansary
{
    public class Circle : Shape
    {
        private int _radius;

        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                OnPropertyChanged(nameof(Radius));
            }
        }
    }
}