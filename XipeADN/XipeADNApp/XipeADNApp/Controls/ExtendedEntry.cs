using Xamarin.Forms;

namespace XipeADNApp.Controls
{
    public class ExtendedEntry : Entry
    {
        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create("LineColor", typeof(Color), typeof(ExtendedEntry), Color.Default);

        public Color LineColor
        {
            get => (Color)GetValue(LineColorProperty);
            set => SetValue(LineColorProperty, value);
        }
    }
}
