using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Caliburn.Micro;
using static FastPosFrontend.ViewModels.AppDrawerConductor;

namespace FastPosFrontend.ViewModels
{
    public class AppDrawerConductor : PropertyChangedBase
    {
        private bool _isTopDrawerOpen;
        private bool _isBottomDrawerOpen;
        private bool _isLeftDrawerOpen;
        private bool _isRightDrawerOpen;

        private readonly Dictionary<(object owner, DrawerPosition position, object tag), (object template, object content)>
            _drawerContents;

        private readonly Dictionary<object, object> _cache;

        private Drawer _left;
        private Drawer _right;
        private Drawer _top;
        private Drawer _bottom;
        private Drawer _navigationDrawer;

        private AppDrawerConductor()
        {
            _drawerContents =
                new Dictionary<(object owner, DrawerPosition position, object tag), (object template, object content)>();
            _cache = new Dictionary<object, object>();
            this.PropertyChanged += AppDrawerConductor_PropertyChanged;
        }

        private void AppDrawerConductor_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsRightDrawerOpen)|| e.PropertyName == nameof(IsLeftDrawerOpen))
            {
                if (NavigationDrawer.Position == DrawerPosition.Left)
                {
                    IsNavigationDrawerOpen = IsLeftDrawerOpen;
                }
                else
                {
                    IsNavigationDrawerOpen = IsRightDrawerOpen;

                }
            }
        }

        public static AppDrawerConductor Instance { get; } = new AppDrawerConductor();

        public bool IsTopDrawerOpen
        {
            get => _isTopDrawerOpen;
            set
            {
                if (!value)
                {
                    if (Top != null)
                    {
                        Top.Visibility = Visibility.Collapsed;
                    }
                }

                Set(ref _isTopDrawerOpen, value);
            }
        }

        public bool IsBottomDrawerOpen
        {
            get => _isBottomDrawerOpen;
            set => Set(ref _isBottomDrawerOpen, value);
        }

        public bool IsLeftDrawerOpen
        {
            get => _isLeftDrawerOpen;
            set => Set(ref _isLeftDrawerOpen, value);
        }

        public bool IsRightDrawerOpen
        {
            get => _isRightDrawerOpen;
            set => Set(ref _isRightDrawerOpen, value);
        }
        private bool _isNavigationDrawerOpen;

        public bool IsNavigationDrawerOpen
        {
            get { return _isNavigationDrawerOpen; }
            set { Set(ref _isNavigationDrawerOpen, value); }
        }



        public Drawer Left
        {
            get => _left;
            set => Set(ref _left, value);
        }

        public Drawer Right
        {
            get => _right;
            set => Set(ref _right, value);
        }

        public Drawer Top
        {
            get => _top;
            set => Set(ref _top, value);
        }

        public Drawer Bottom
        {
            get => _bottom;
            set => Set(ref _bottom, value);
        }

        public Drawer NavigationDrawer
        {
            get { return _navigationDrawer; }
            set { Set(ref _navigationDrawer, value); }
        }


        public enum DrawerPosition
        {
            Left = 0,
            Top = 1,
            Right = 2,
            Bottom = 3
        }

        public void Init(DrawerPosition position, object owner, object resKey, object content, object tag = null)
        {
            var key = (owner, position, tag);
            var template = Application.Current.FindResource(resKey);
            var value = (template, content);
            _drawerContents.Add(key, value);
        }

        public void InitLeft(object owner, object resKey, object content, object tag = null)
        {
            Init(DrawerPosition.Left, owner, resKey, content, tag);
        }

        public void InitRight(object owner, object resKey, object content, object tag = null)
        {
            Init(DrawerPosition.Right, owner, resKey, content, tag);
        }

        public void InitTop(object owner, object resKey, object content, object tag = null)
        {
            Init(DrawerPosition.Top, owner, resKey, content, tag);
        }

        public void InitBottom(object owner, object resKey, object content, object tag = null)
        {
            Init(DrawerPosition.Bottom, owner, resKey, content, tag);
        }

        public void InitNavigationDrawer(object owner, object resKey, object content)
        {
            DrawerPosition position = DrawerPosition.Left;
            if (Thread.CurrentThread.CurrentUICulture.Name.Contains("ar"))
            {
                position = DrawerPosition.Right;
            }
            var template = Application.Current.FindResource(resKey);
            NavigationDrawer = new Drawer() { Content = content, Template = template ,Position = position};
        }

        public bool Open(DrawerPosition position, object owner, object tag = null)
        {
            var key = (owner, position, tag);
            var value = _drawerContents[key];
            Drawer drawer;
            if (!_cache.ContainsKey(key))
            {
                 drawer = new Drawer { Template = value.template, Content = value.content };
                _cache.Add(key,drawer);
            }
            else
            {
                drawer = (Drawer)_cache[key];
                drawer.Visibility = Visibility.Visible;
            }

           
            
            switch (position)
            {
                case DrawerPosition.Left:
                    Left = drawer;
                    IsLeftDrawerOpen = true;
                    break;
                case DrawerPosition.Right:
                    Right = drawer;
                    IsRightDrawerOpen = true;
                    break;
                case DrawerPosition.Top:
                    Top = drawer;
                    IsTopDrawerOpen = true;
                    break;
                case DrawerPosition.Bottom:
                    Bottom = drawer;
                    IsBottomDrawerOpen = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

            return true;
        }

        public bool OpenLeft(object owner, object tag = null)
        {
            return Open(DrawerPosition.Left, owner, tag);
        }

        public bool OpenRight(object owner, object tag = null)
        {
            return Open(DrawerPosition.Right, owner, tag);
        }

        public bool OpenTop(object owner, object tag = null)
        {
            return Open(DrawerPosition.Top, owner, tag);
        }

        public bool OpenBottom(object owner, object tag = null)
        {
            return Open(DrawerPosition.Bottom, owner, tag);
        }

        public void OpenNavigationDrawer()
        {
            if (!IsNavigationDrawerOpen)
            {
                if (NavigationDrawer.Position == DrawerPosition.Left)
                {
                    Left = NavigationDrawer;
                    IsLeftDrawerOpen = true;
                }
                else
                {
                    Right = NavigationDrawer;
                    IsRightDrawerOpen = true;
                }
            }
            else
            {
                if (NavigationDrawer.Position == DrawerPosition.Left)
                {
                    Left = null;
                    IsLeftDrawerOpen = false;
                }
                else
                {
                    Right = null;
                    IsRightDrawerOpen = false;
                }
            }
        }
    }

    public class Drawer : PropertyChangedBase
    {
        private object _template;
        private object _content;
        private Visibility _visibility = Visibility.Visible;

        public object Template
        {
            get => _template;
            set => Set(ref _template, value);
        }

        public object Content
        {
            get => _content;
            set => Set(ref _content, value);
        }

        public Visibility Visibility
        {
            get => _visibility;
            set => Set(ref _visibility, value);
        }

        public DrawerPosition Position { get; set; }
    }
}