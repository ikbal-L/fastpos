﻿using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace FastPosFrontend.Navigation
{

    [AttributeUsage(AttributeTargets.Class)]
    public class NavigationItemAttribute : Attribute, INavigationItem
    {
        public NavigationItemAttribute(string title, Type target, string iconResKey ="",
            NavigationItemLoadingStrategy loadingStrategy = NavigationItemLoadingStrategy.Lazy
            , Type parentNavigationItem = null, string groupName = "", 
            bool keepAlive = false, 
            bool isDefault = false)
        {
            Title = title;
            Target = target;
            LoadingStrategy = loadingStrategy;
            ParentNavigationItem = parentNavigationItem;
            GroupName = groupName;
            KeepAlive = keepAlive;
            IsDefault = isDefault;

            var typeName = target.Name.Replace("ViewModel", "");

            if (string.IsNullOrEmpty(iconResKey))
            {
                IconResKey = $"{typeName}NavigationItemIcon";
            }
            else
            {
                IconResKey = iconResKey;
            }

            

            var formattedTitle = string.Join(" ", Regex.Split(typeName, "([A-Z][a-z]+)"));
            



        }

        public string Title { get; }

        public NavigationItemLoadingStrategy LoadingStrategy { get; }
        public Type ParentNavigationItem { get; }
        public string GroupName { get; }

        public Type Target { get; }

        public bool KeepAlive { get; }
        public bool IsDefault { get; }

        public string IconResKey { get; }
    }

}