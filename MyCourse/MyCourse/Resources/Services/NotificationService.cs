#if ANDROID
using MyCourse.Platforms.Android;
#endif
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;

namespace MyCourse.Resources.Services
{
    public interface INotificationService
    {
        void ScheduleNotification(int id, string title, string message, DateTime notifyTime);
        void CancelNotification(int id);
    }
    public static class Notify
    {
        public static void CreateNotification(int id, string title, string message, DateTime notifyTime)
        {
#if ANDROID
            var service = new NotificationServiceAndroid();
            service.ScheduleNotification(id, title, message, notifyTime);
#endif
        }

        public static void DeleteNotification(int id)
        {
#if ANDROID
            var service = new NotificationServiceAndroid();
            service.CancelNotification(id);
#endif
        }
    }
}
