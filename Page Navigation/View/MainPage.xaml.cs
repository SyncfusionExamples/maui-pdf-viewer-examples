using Microsoft.Maui.Platform;
namespace PageNavigation;

public partial class MainPage : ContentPage
{
    private int targetPageNumber = 0;
    private bool pageNumberChanged = false;

    public MainPage()
	{
		InitializeComponent();
    }

    /// <summary>
    /// Handles automatic page number selection in the entry when the entry is tapped.
    /// </summary>
    private void PageNumberEntry_HandlerChanged(object sender, EventArgs e)
    {
        if (PageNumberEntry.Handler != null)
        {
            var handler = PageNumberEntry.Handler as Microsoft.Maui.Handlers.EntryHandler;
#if ANDROID
        handler.PlatformView.SetSelectAllOnFocus(true);
#elif IOS || MACCATALYST
        handler.PlatformView.EditingDidBegin += (s, e) =>
        {
            handler.PlatformView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
        };
#elif WINDOWS
        handler.PlatformView.GotFocus += (s, e) =>
        {
            handler.PlatformView.SelectAll();
        };
#endif
        }
    }

    /// <summary>
    /// Handles the page number validation when the page number is typed in the entry.
    /// </summary>
    void PageNumberEntry_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        if (PageNumberEntry != null && PageNumberEntry.IsFocused && !string.IsNullOrEmpty(PageNumberEntry.Text))
        {
            bool isPageNumber = int.TryParse(PageNumberEntry.Text, out targetPageNumber);
            if (isPageNumber)
            {
                pageNumberChanged = true;
                PageNumberEntry.Text = targetPageNumber.ToString();
            }
            else
            {
                PageNumberEntry.Text = e.OldTextValue;
            }
        }
    }

    /// <summary>
    /// Handles when the page number entry focused property changes to unfocused.
    /// </summary>
    void PageNumberEntry_Unfocused(System.Object sender, Microsoft.Maui.Controls.FocusEventArgs e)
    {
        PageNumberEntry.Text = PdfViewer?.PageNumber.ToString();
    }

    /// <summary>
    /// Handles when the page number is entered in the entry.
    /// </summary>
    private void PageNumberEntry_Completed(object sender, EventArgs e)
    {
        // When the enter key is pressed, the soft keyboard hides.
#if ANDROID
        if (Platform.CurrentActivity?.CurrentFocus != null)
            Platform.CurrentActivity.HideKeyboard(Platform.CurrentActivity.CurrentFocus);
#endif
        if (pageNumberChanged && targetPageNumber != PdfViewer.PageNumber)
        {
            if (targetPageNumber > 0 && targetPageNumber <= PdfViewer.PageCount)
            {
                PdfViewer.GoToPage(targetPageNumber);
                pageNumberChanged = false;

            }
            else
            {
                // Shows the invalid page number to the user when trying to go to the entered page number when the loaded PDF document does not have a page number.
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MessageBox.Show("Error", "Invalid Page Number","OK");
                });
                PageNumberEntry.Text = PdfViewer.PageNumber.ToString();
            }
            PageNumberEntry.Unfocus();
        }
    }

    /// <summary>
    /// Handles the button enable and disable states when the button property changes.
    /// </summary>
    private void Button_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var button = (ImageButton)sender;
        if (e.PropertyName == "IsEnabled")
        {
            if (!button.IsEnabled)
            {
                // When the image button is disabled, it changes the image.
                button.Source = $"{button.ClassId}_disabled.png";
                button.Unfocus(); 
            }
            else if (button.IsEnabled)
            {
                // When the image button is enabled, it changes the image.
                button.Source = $"{button.ClassId}.png";
            }
        }     
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), () =>
        {
            VisualStateManager.GoToState(button, "Normal");
        });
    }
}

