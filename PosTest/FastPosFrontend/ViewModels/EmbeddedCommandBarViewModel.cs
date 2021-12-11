﻿using System.Windows;
using FastPosFrontend.Helpers;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels
{
    public class EmbeddedCommandBarViewModel: PropertyChangedBase
    {
        public EmbeddedCommandBarViewModel(object context,string resourseKey)
        {
            Context = context;
            DataTemplate = Application.Current.FindResource(resourseKey) as DataTemplate;
        }

        public object Context { get; private set; }

        public DataTemplate DataTemplate { get; private set; }


    }

   

}