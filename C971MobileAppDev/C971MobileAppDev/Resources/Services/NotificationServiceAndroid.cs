#if ANDROID
using Android.App;
using Android.Content;
using C971MobileAppDev.Resources.Services;
using System.Diagnostics;
using Application = Android.App.Application;

[assembly: Dependency(typeof(C971MobileAppDev.Platforms.Android.NotificationServiceAndroid))]

namespace C971MobileAppDev.Platforms.Android
{
    public class NotificationServiceAndroid : INotificationService
    {
        public void ScheduleNotification(int id, string title, string message, DateTime notifyTime)
        {
            if (notifyTime <= DateTime.Now)
            {
                return;
            }

            var context = Application.Context;

            Intent intent = new Intent(context, typeof(NotificationReceiver));
            intent.PutExtra("title", title);
            intent.PutExtra("message", message);

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(
                context,
                id,
                intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent
                );

            var triggerTime = new DateTimeOffset(notifyTime).ToUnixTimeMilliseconds();
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerTime, pendingIntent);
        }

        public void CancelNotification(int id) {
            var context = Application.Context;

            Intent intent = new Intent(context, typeof(NotificationReceiver));

            PendingIntent pendingIntent = PendingIntent.GetBroadcast(
                context,
                id,
                intent,
                PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);

            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            alarmManager.Cancel(pendingIntent);
        }


    }
}
#endif