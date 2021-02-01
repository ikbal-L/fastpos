using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace PosTest.Helpers
{
    static class ToastNotification
    {
        // https://github.com/rafallopatka/ToastNotifications
        private static readonly bool IsRunningFromXUnit =
            AppDomain.CurrentDomain.GetAssemblies().Any(
                a => a.FullName.StartsWith("XUnitTesting"));

        private static Notifier _notifier;

        private static Notifier Instance
        {
            get => _notifier ??
                   (_notifier = new Notifier(cfg =>
                   {
                       cfg.PositionProvider = new WindowPositionProvider(
                           parentWindow: Application.Current.MainWindow,
                           corner: Corner.BottomRight,
                           offsetX: 10,
                           offsetY: 10);

                       cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                           notificationLifetime: TimeSpan.FromSeconds(1.5),
                           maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                       cfg.Dispatcher = Application.Current.Dispatcher;
                   }));
        }

        public static void Notify(string message, NotificationType type = NotificationType.Error)
        {
            Notifier notifier = Instance;
            if (!IsRunningFromXUnit)
            {
                if (type == NotificationType.Error)
                {
                    notifier.ShowError(message);
                }
                else if (type == NotificationType.Information)
                {
                    notifier.ShowInformation(message);
                }
                else if (type == NotificationType.Warning)
                {
                    notifier.ShowWarning(message);
                }
                else if (type == NotificationType.Success)
                {
                    notifier.ShowSuccess(message);
                }
            }
            else
            {
                 throw new NotificationException(message);
            }
        }

        public static void ErrorNotification(int respStatusCode)
        {
            switch (respStatusCode)
            {
                case 401:
                    ToastNotification.Notify("Database insertion error " + respStatusCode.ToString());
                    break;
                case 402:
                    ToastNotification.Notify("Id exists in the database " + respStatusCode.ToString());
                    break;
                case 403:
                    ToastNotification.Notify("Database access error " + respStatusCode.ToString());
                    break;
                case 404:
                    ToastNotification.Notify("Bad request " + respStatusCode.ToString());
                    break;
                case 405:
                    ToastNotification.Notify("Authentification error " + respStatusCode.ToString());
                    break;
                case 406:
                    ToastNotification.Notify("Authentification error " + respStatusCode.ToString());
                    break;
                case 407:
                    ToastNotification.Notify("Authentification error " + respStatusCode.ToString());
                    break;
                case 408:
                    ToastNotification.Notify("Database error in Annex_id " + respStatusCode.ToString());
                    break;
                case 409:
                    ToastNotification.Notify("Config Database error " + respStatusCode.ToString());
                    break;
                case 410:
                    ToastNotification.Notify(" User or password error" + respStatusCode.ToString());
                    break;
            }
        }
    }

    public class NotificationException : Exception
    {
        public NotificationException(string message) : base(message)
        {
        }
    }
}
