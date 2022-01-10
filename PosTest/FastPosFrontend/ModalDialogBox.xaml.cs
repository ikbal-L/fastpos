using System;
using System.Windows;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels;

namespace FastPosFrontend
{
    /// <summary>
    /// Interaction logic for ModalDialogBox.xaml
    /// </summary>
    public partial class ModalDialogBox : Window
    {
        private ModalDialogBox(bool isTemplated = false)
        {
            IsTemplated = isTemplated;
            InitializeComponent();
        }

        private static ModalDialogBox Instance;

        public bool IsTemplated { get; set; }

        public bool Show()
        {
            var  result = Instance.ShowDialog();
            return result != null && result.Value;
        }

        public static ModalDialogBox YesNo(string message, string title,bool isYesDefault = false,bool isNoDefault = false)
        {
            if (isYesDefault&&isNoDefault)
            {
                throw new ArgumentException($"Only one argument must be set to true {nameof(isYesDefault)} or {nameof(isNoDefault)}");
            }
            var vm = new GenericDialogContentViewModel(message, title,
                new GenericCommand("Yes", o => { Instance.DialogResult = true; Instance.Close(); }),
                new GenericCommand("No", o => { Instance.DialogResult = false; Instance.Close(); }));
            Instance = new ModalDialogBox() { DataContext = vm };
            return Instance;
        }

        public static ModalDialogBox Ok(string message, string title)
        {
            var vm = new GenericDialogContentViewModel(message, title,
                new GenericCommand("Ok", o => { Instance.DialogResult = null; Instance.Close(); }));
            Instance = new ModalDialogBox() { DataContext = vm };
            return Instance;
        }

        public static ModalDialogBox Ok(object content,string template, string title,Predicate<object> predicate = null)
        {
            var dt = Application.Current.FindResource(template) as DataTemplate;
            var vm = new TemplatedDialogContentViewModel(content, dt, title,
                new GenericCommand("Ok", o => {
                    if (predicate != null && !predicate.Invoke(null)) return;
                    
                    Instance.DialogResult = null; Instance.Close();
                }));
            Instance = new ModalDialogBox(isTemplated:true) { DataContext = vm };
            return Instance;
        }

        public static ModalDialogBox OkCancel(object content, string template, string title, Predicate<object> predicate = null)
        {
            var dt = Application.Current.FindResource(template) as DataTemplate;
            var vm = new TemplatedDialogContentViewModel(content, dt, title,
                new GenericCommand("Ok", o => {
                    if (predicate != null && !predicate.Invoke(null)) return;

                    Instance.DialogResult = true; Instance.Close();
                }),
                new GenericCommand("Cancel",o=> { 
                    Instance.DialogResult = false; Instance.Close(); })
                );

            Instance = new ModalDialogBox(isTemplated: true) { DataContext = vm };
            return Instance;
        }

        public static ModalDialogBox Submit(object content, string template, string title, Predicate<object> predicate = null, Func<bool> onSubmit = null)
        {
            var dt = Application.Current.FindResource(template) as DataTemplate;
            var vm = new TemplatedDialogContentViewModel(content, dt, title,
                new GenericCommand("Submit", o => {
                    if (predicate != null && !predicate.Invoke(null)) return;

                    Instance.DialogResult = onSubmit?.Invoke()??false; Instance.Close();
                })
                );

            Instance = new ModalDialogBox(isTemplated: true) { DataContext = vm };
            return Instance;
        }
    }
}