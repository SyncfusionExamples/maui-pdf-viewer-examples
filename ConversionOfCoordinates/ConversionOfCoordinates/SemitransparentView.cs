using Syncfusion.Maui.Core;
using Syncfusion.Maui.Core.Internals;
namespace ConversionOfCoordinates;

public class SemitransparentView : SfView, ITapGestureListener
{
    internal event EventHandler<TapEventArgs?>? ViewTapped;
    public SemitransparentView()
    {
        this.BackgroundColor = Color.FromRgba(72, 70, 73, 80);
        ScrollView scrollView = new ScrollView();
        scrollView.Orientation = ScrollOrientation.Both;
        this.Children.Add(scrollView);
        this.AddGestureListener(this);
    }

    public void OnTap(TapEventArgs e)
    {
        ViewTapped?.Invoke(this, e);
    }
}