﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using Caliburn.Micro;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace FastPosFrontend.SL.Controls
{
    internal class ColorToolViewModel : PropertyChangedBase
    {
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();

        private ColorScheme _activeScheme;
        public ColorScheme ActiveScheme
        {
            get => _activeScheme;
            set
            {
                if (_activeScheme != value)
                {
                    Set(ref _activeScheme, value);
                }
            }
        }

        private Color? _selectedColor;
        public Color? SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (_selectedColor != value)
                {
                    Set(ref _selectedColor, value);

                    // if we are triggering a change internally its a hue change and the colors will match
                    // so we don't want to trigger a custom color change.
                    var currentSchemeColor = ActiveScheme switch
                    {
                        ColorScheme.Primary => _primaryColor,
                        ColorScheme.Secondary => _secondaryColor,
                        ColorScheme.PrimaryForeground => _primaryForegroundColor,
                        ColorScheme.SecondaryForeground => _secondaryForegroundColor,
                        _ => throw new NotSupportedException($"{ActiveScheme} is not a handled ColorScheme.. Ye daft programmer!")
                    };

                    if (_selectedColor != currentSchemeColor && value is Color color)
                    {
                        ChangeCustomColor(color);
                    }
                }
            }
        }

        private bool _isDarkTheme;
        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set
            {
                if (Set(ref _isDarkTheme, value))
                {
                    ApplyBase(value);
                }
            }
        }

        public IEnumerable<ISwatch> Swatches { get; } = SwatchHelper.Swatches;

        public ICommand ChangeCustomHueCommand { get; }

        public ICommand ChangeHueCommand { get; }
        public ICommand ChangeToPrimaryCommand { get; }
        public ICommand ChangeToSecondaryCommand { get; }
        public ICommand ChangeToPrimaryForegroundCommand { get; }
        public ICommand ChangeToSecondaryForegroundCommand { get; }

        public ICommand ToggleBaseCommand { get; }

        private void ApplyBase(bool isDark)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }

        public ColorToolViewModel()
        {
            ToggleBaseCommand = new DelegateCommandBase(o => ApplyBase((bool)o));
            ChangeHueCommand = new DelegateCommandBase(ChangeHue);
            ChangeCustomHueCommand = new DelegateCommandBase(ChangeCustomColor);
            ChangeToPrimaryCommand = new DelegateCommandBase(o => ChangeScheme(ColorScheme.Primary));
            ChangeToSecondaryCommand = new DelegateCommandBase(o => ChangeScheme(ColorScheme.Secondary));
            ChangeToPrimaryForegroundCommand = new DelegateCommandBase(o => ChangeScheme(ColorScheme.PrimaryForeground));
            ChangeToSecondaryForegroundCommand = new DelegateCommandBase(o => ChangeScheme(ColorScheme.SecondaryForeground));


            ITheme theme = _paletteHelper.GetTheme();

            _primaryColor = theme.PrimaryMid.Color;
            _secondaryColor = theme.SecondaryMid.Color;

            SelectedColor = _primaryColor;

            //IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;

            //if (_paletteHelper.GetThemeManager() is { } themeManager)
            //{
            //    themeManager.ThemeChanged += (_, e) =>
            //    {
            //        IsDarkTheme = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
            //    };
            //}
        }

        private void ChangeCustomColor(object obj)
        {
            var color = (Color)obj;

            if (ActiveScheme == ColorScheme.Primary)
            {
                //_paletteHelper.ChangePrimaryColor(color);
                _primaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                //_paletteHelper.ChangeSecondaryColor(color);
                _secondaryColor = color;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(color);
                _primaryForegroundColor = color;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(color);
                _secondaryForegroundColor = color;
            }
        }

        private void ChangeScheme(ColorScheme scheme)
        {
            ActiveScheme = scheme;
            if (ActiveScheme == ColorScheme.Primary)
            {
                SelectedColor = _primaryColor;
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                SelectedColor = _secondaryColor;
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SelectedColor = _primaryForegroundColor;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SelectedColor = _secondaryForegroundColor;
            }
        }

        private Color? _primaryColor;

        private Color? _secondaryColor;

        private Color? _primaryForegroundColor;

        private Color? _secondaryForegroundColor;

        private void ChangeHue(object obj)
        {
            var hue = (Color)obj;

            SelectedColor = hue;
            if (ActiveScheme == ColorScheme.Primary)
            {
                //_paletteHelper.ChangePrimaryColor(hue);
                _primaryColor = hue;
                _primaryForegroundColor = _paletteHelper.GetTheme().PrimaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.Secondary)
            {
                //_paletteHelper.ChangeSecondaryColor(hue);
                _secondaryColor = hue;
                _secondaryForegroundColor = _paletteHelper.GetTheme().SecondaryMid.GetForegroundColor();
            }
            else if (ActiveScheme == ColorScheme.PrimaryForeground)
            {
                SetPrimaryForegroundToSingleColor(hue);
                _primaryForegroundColor = hue;
            }
            else if (ActiveScheme == ColorScheme.SecondaryForeground)
            {
                SetSecondaryForegroundToSingleColor(hue);
                _secondaryForegroundColor = hue;
            }
        }

        private void SetPrimaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.PrimaryLight = new ColorPair(theme.PrimaryLight.Color, color);
            theme.PrimaryMid = new ColorPair(theme.PrimaryMid.Color, color);
            theme.PrimaryDark = new ColorPair(theme.PrimaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }

        private void SetSecondaryForegroundToSingleColor(Color color)
        {
            ITheme theme = _paletteHelper.GetTheme();

            theme.SecondaryLight = new ColorPair(theme.SecondaryLight.Color, color);
            theme.SecondaryMid = new ColorPair(theme.SecondaryMid.Color, color);
            theme.SecondaryDark = new ColorPair(theme.SecondaryDark.Color, color);

            _paletteHelper.SetTheme(theme);
        }
    }
}