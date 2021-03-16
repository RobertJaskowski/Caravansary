using Caravansary.Properties;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections.Specialized;

namespace Caravansary.CoreModules.Roadmap.ViewModel
{
    class RoadmapViewModel : CoreModule
    {

        #region Properties
        public override string ModuleName => "Roadmap";



        //private bool _isEmptyLabelVisible;
        //public bool IsEmptyLabelVisible
        //{
        //    get
        //    {


        //        return _isEmptyLabelVisible;
        //    }
        //    set
        //    {

        //        _isEmptyLabelVisible = value;
        //        OnPropertyChanged(nameof(IsEmptyLabelVisible));
        //    }

        //}


        public static RoadmapItems SettingsRoadmapItems
        {
            get
            {
                return Settings.Default.RoadmapItems;
            }

            set
            {
                Settings.Default.RoadmapItems = value;
                Settings.Default.Save();
            }
        }

        private ObservableCollection<RoadmapItem> _roadmapItems;
        public ObservableCollection<RoadmapItem> RoadmapItems
        {
            get
            {
                if (_roadmapItems == null)
                {
                    _roadmapItems = new ObservableCollection<RoadmapItem>(SettingsRoadmapItems);

                    if (_roadmapItems.Count <= 0)
                        _roadmapItems.Add(new RoadmapItem("Click to create roadmap item"));
                }

                //if (_roadmapItems.Count > 0)
                //    IsEmptyLabelVisible = false;
                //else
                //    IsEmptyLabelVisible = true;

                return _roadmapItems;
            }
            set
            {

                _roadmapItems = value;

                OnPropertyChanged(nameof(RoadmapItems));
            }
        }


        #endregion


        #region Commands
        //left and alt add to left 
        //right and alt add to right
        //alt??  remove

        private ICommand _roadmapItemLeftClicked;
        public ICommand RoadmapItemLeftClicked
        {
            get
            {
                if (_roadmapItemLeftClicked == null)
                    _roadmapItemLeftClicked = new RelayCommand(
                       (object o) =>
                       {
                           Debug.WriteLine("left click");

                           AddTextBlock(RoadmapAddingDirection.LEFT, o as RoadmapItem);

                       },
                       (object o) =>
                       {
                           return RoadmapItems.Count > 0;
                       });

                return _roadmapItemLeftClicked;

            }
        }

        private ICommand _RoadmapItemRightClicked;
        public ICommand RoadmapItemRightClicked
        {
            get
            {
                if (_RoadmapItemRightClicked == null)
                    _RoadmapItemRightClicked = new RelayCommand(
                       (object o) =>
                       {


                           Debug.WriteLine("right click");

                           AddTextBlock(RoadmapAddingDirection.RIGHT, o as RoadmapItem);
                       },
                       (object o) =>
                       {
                           return RoadmapItems.Count > 1;
                       });

                return _RoadmapItemRightClicked;

            }
        }



        private ICommand _roadmapItemMiddleClicked;
        public ICommand RoadmapItemMiddleClicked
        {
            get
            {
                if (_roadmapItemMiddleClicked == null)
                    _roadmapItemMiddleClicked = new RelayCommand(
                       (object o) =>
                       {
                           if (o is RoadmapItem)
                           {

                               RoadmapItems.Remove(o as RoadmapItem);
                           }

                           Debug.WriteLine("wheel click");

                       },
                       (object o) =>
                       {
                           return RoadmapItems.Count > 1;
                       });

                return _roadmapItemMiddleClicked;

            }
        }


        #endregion


        private KeyboardListener keyboardListener;
        private bool ctrlIsHeld = false;

        public RoadmapViewModel()
        {
            Settings.Default.PropertyChanged += SettingsChanged;

            keyboardListener = KeyboardListener.Instance;
            keyboardListener.OnKeyPressed += KeyPressed;
            keyboardListener.OnKeyReleased += KeyReleased;


            //RoadmapItems.CollectionChanged += RoadmapItemsCollectionChanged;

            //foreach (var item in RoadmapItems)
            //{

            //    item.PropertyChanged += RoadmapItemsItemChanged;
            //}
        }

        private void RoadmapItemsItemChanged(object sender, PropertyChangedEventArgs e)
        {
            SettingsRoadmapItems = new RoadmapItems(RoadmapItems.ToList());

        }

        private void RoadmapItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsRoadmapItems = new RoadmapItems(RoadmapItems.ToList());
        }

        private void KeyPressed(object sender, KeyPressedArgs e)
        {
            if (e.KeyPressed == Key.LeftCtrl)
            {
                ctrlIsHeld = true;
            }
        }

        private void KeyReleased(object sender, KeyReleasedArgs e)
        {

            if (e.KeyReleased == Key.LeftCtrl)
            {
                ctrlIsHeld = false;
            }
        }


        private void AddTextBlock(RoadmapAddingDirection direction, RoadmapItem from)
        {


            RoadmapItem roadmapItem = new RoadmapItem("empty");

            if (direction == RoadmapAddingDirection.LEFT)
                RoadmapItems.Insert(RoadmapItems.IndexOf(from), roadmapItem);
            if (direction == RoadmapAddingDirection.RIGHT)
                RoadmapItems.Insert(RoadmapItems.IndexOf(from) + 1, roadmapItem);




            //foreach (var item in RoadmapItems)
            //{
            //    if(item == from)
            //    {
            //        RoadmapItems.Add()
            //    }
            //}


        }

        private void SettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            //  IsEmptyVisible = Settings.Default.RoadmapEnabled;
        }

        public override void CloseModule()
        {
            Settings.Default.PropertyChanged -= SettingsChanged;

            SettingsRoadmapItems = new RoadmapItems(RoadmapItems.ToList());
        }
    }



}
