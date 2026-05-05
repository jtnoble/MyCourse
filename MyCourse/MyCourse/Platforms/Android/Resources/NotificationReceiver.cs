using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using AndroidX.Core.App;
using System.Diagnostics;

namespace MyCourse.Platforms.Android
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class NotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            string title = intent.GetStringExtra("title");
            string message = intent.GetStringExtra("message");

            var builder = new NotificationCompat.Builder(context, "course_channel")
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Microsoft.Maui.Resource.Mipmap.appicon)
                .SetAutoCancel(true);

            NotificationManager manager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            manager.Notify(new Random().Next(), builder.Build());
        }
    }
}
