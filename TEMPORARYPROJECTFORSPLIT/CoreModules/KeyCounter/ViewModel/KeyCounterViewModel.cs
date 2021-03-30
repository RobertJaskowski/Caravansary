using System.Diagnostics;
using System.Windows.Input;

namespace Caravansary.CoreModules.KeyCounter.ViewModel
{
    class KeyCounterViewModel : CoreModule
    {
        public override string ModuleName => "KeyCounter";



        private int _keyCounterString = 0;
        public int KeyCounter
        {
            get
            {

                return _keyCounterString;
            }
            set
            {

                _keyCounterString = value;
                OnPropertyChanged(nameof(KeyCounter));
            }
        }

        private bool ZPressed = false;
        private bool CtrlPressed = false;

        private ICommand _keyCounterClicked;
        public ICommand KeyCounterClicked
        {
            get
            {
                if (_keyCounterClicked == null)
                    _keyCounterClicked = new RelayCommand(
                       (object o) =>
                       {

                           Debug.WriteLine("_keyCounterClicked " + KeyCounter);

                       },
                       (object o) =>
                       {
                           return true;
                       });

                return _keyCounterClicked;

            }
        }


        private KeyboardListener _listener;

        public KeyCounterViewModel()
        {

            _listener = KeyboardListener.Instance;//todo move to main window



            _listener.OnKeyPressed += _listener_OnKeyPressed;
            _listener.OnKeyReleased += _listener_OnKeyReleased;
        }

        void _listener_OnKeyReleased(object sender, KeyReleasedArgs e)
        {
            if (e.KeyReleased == System.Windows.Input.Key.LeftCtrl)
            {
                CtrlPressed = false;

            }
            else if (e.KeyReleased == System.Windows.Input.Key.Z)
            {
                ZPressed = false;

            }


        }

        private void CheckForCtrlZCombination()
        {
            if (CtrlPressed && ZPressed)
            {
                KeyCounter++;
                //label1.Content = ctrlZCounter;
            }
        }

        void _listener_OnKeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == System.Windows.Input.Key.LeftCtrl)
            {
                CtrlPressed = true;
                CheckForCtrlZCombination();
            }
            else if (e.KeyPressed == System.Windows.Input.Key.Z)
            {
                ZPressed = true;
                CheckForCtrlZCombination();
            }


        }

        public override void CloseModule()
        {
            // _listener.UnHookKeyboard();
            _listener.OnKeyPressed -= _listener_OnKeyPressed;
            _listener.OnKeyReleased -= _listener_OnKeyReleased;
        }
    }
}
