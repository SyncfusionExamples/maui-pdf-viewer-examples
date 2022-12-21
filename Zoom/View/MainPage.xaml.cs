namespace Magnification;

public partial class MainPage : ContentPage
{
    /// <summary>
    /// Constructor of main page class
    /// </summary>
	public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Occurs when a button's property is changed. It is used in this application to handle the IsEnabled property changes.
    /// </summary>
    private void Button_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var button = (ImageButton)sender;
        if (e.PropertyName == "IsEnabled")
        {
            if (!button.IsEnabled)
            {
                //Set the image to the button when disabled and unfocus.
                button.Unfocus();
                button.Source = $"{button.ClassId}_disable.png";

            }
            else if (button.IsEnabled)
            {
                //Set the image to the button when enabled.
                button.Source = $"{button.ClassId}.png";
            }
        }
        //Restore the button state to normal after few milliseconds.
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), () =>
        {
            VisualStateManager.GoToState(button, "Normal");
        });
    }
}
