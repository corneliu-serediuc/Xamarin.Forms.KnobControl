using System;

using Xamarin.Forms;
using System.Diagnostics;

namespace KnobControl
{
    public class App : Application
    {
        public App()
        {
            var rectangle = new Rectangle(0, 0, 240, 240);

            // The root page of your application
            MainPage = new ContentPage
            {
                Padding = 0,
                    BackgroundImage = Device.OnPlatform("view_bg.png", "view_bg.png", "view_bg.png"),
                Content = new StackLayout
                {
                    Padding = 0,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Children =
                    {
                        new Knob(rectangle, 0, 100, 25),
                        new Knob(rectangle, 0, 100, 75, true)
                    }
                }
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}

