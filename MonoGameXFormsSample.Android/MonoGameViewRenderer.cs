using Android.App;
using Android.Content;
using GameLibrary;
using Microsoft.Xna.Framework;
using MonoGameXFormsSample.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(MonoGameXFormsSample.Controls.MonoGameView), typeof(MonoGameViewRenderer))]
namespace MonoGameXFormsSample.Droid
{
    public class MonoGameViewRenderer : ViewRenderer<Controls.MonoGameView, Android.Views.View>
    {
        private TestGame _game;
        private Android.Views.View _gameView;

        public MonoGameViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Controls.MonoGameView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            if (e.OldElement != null)
            {
                // remove handlers
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var activity = Context as Activity;
                    Game.Activity = activity;

                    _game = new TestGame();
                    _gameView = (Android.Views.View)_game.Services.GetService(typeof(Android.Views.View));
                    SetNativeControl(_gameView);
                    _game.Run();
                    AndroidGameActivity.ExternalActivityResumed(this);
                }

                // add handlers
            }
        }
    }
}