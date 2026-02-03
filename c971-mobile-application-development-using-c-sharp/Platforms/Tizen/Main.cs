using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace c971_mobile_application_development_using_c_sharp
{
    internal class Program : MauiApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        static void Main(string[] args)
        {
            var app = new Program();
            app.Run(args);
        }
    }
}
