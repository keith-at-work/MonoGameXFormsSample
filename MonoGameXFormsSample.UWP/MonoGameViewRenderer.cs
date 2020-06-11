using GameLibrary;
using MonoGameXFormsSample.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(MonoGameXFormsSample.Controls.MonoGameView), typeof(MonoGameViewRenderer))]
namespace MonoGameXFormsSample.UWP
{
    public class MonoGameViewRenderer : ViewRenderer<Controls.MonoGameView, SwapChainPanel>
    {
        private SwapChainPanel _gamePanel;
        private TestGame _game;

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
                    _gamePanel = new SwapChainPanel();
                    SetNativeControl(_gamePanel);

                    _game = MonoGame.Framework.XamlGame<TestGame>.Create("", Window.Current.CoreWindow, _gamePanel);
                }

                // add handlers
            }
        }
    }
}
