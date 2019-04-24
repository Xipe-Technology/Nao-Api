using System;
using Xamarin.Forms;
using Xamarin.Forms.Material.iOS;
using Xamarin.Forms.Platform.iOS;
using XipeADNApp.iOS.Renderers;

[assembly: ExportRenderer(typeof(Button), typeof(CustomMaterialButtonRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace XipeADNApp.iOS.Renderers
{
    public class CustomMaterialButtonRenderer : MaterialButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {

        }
    }
}
