using Microsoft.Maui.Platform;

namespace CustomToolbar;

public partial class MainPage : ContentPage
{
    private int targetPageNumber = 0;
    private bool pageNumberChanged = false;
    /When loading password protected files, it is used to wait the current thread's execution, until the user enters the password.
    private ManualResetEvent manualResetEvent = new ManualResetEvent(false);

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
                    MessageBox.Show("Error", "Invalid Page Number", "OK");
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
        if (sender is Button button)
        {
            if (e.PropertyName == "IsEnabled")
            {
                if (!button.IsEnabled)
                {
                    button.Unfocus();
                }
                else
                {
                    Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(1), () =>
                    {
                        VisualStateManager.GoToState(button, "Normal");
                    });
                }
            }
        }
    }

    /// <summary>
    /// Handles password requested event.
    /// </summary>
    private void PdfViewer_PasswordRequested(object sender, Syncfusion.Maui.PdfViewer.PasswordRequestedEventArgs e)
    {
        //Show the password dialog.
        PasswordDialog.Dispatcher.Dispatch(() => PasswordDialog.IsVisible = true);

        //Block the current thread until user enters the password.
        manualResetEvent.WaitOne();
        manualResetEvent.Reset();

        //Pass the user password to PDF Viewer to validate and load the PDF.
        e.Password = PasswordDialog.Password;
        e.Handled = true;
    }

    /// <summary>
    /// Handles when document is failed to load.
    /// </summary>
    private void PdfViewer_DocumentLoadFailed(object sender, Syncfusion.Maui.PdfViewer.DocumentLoadFailedEventArgs e)
    {
        //Show the incorrect password message to the user, when document failed to load if the password is invalid.
        if (e.Message == "Can't open an encrypted document. The password is invalid.")
        {
            //Unloads the current document.
            PdfViewer.DocumentSource = null;
            MainThread.BeginInvokeOnMainThread(() => MessageBox.Show("Incorrect Password", "The password you entered is incorrect. Please try again."));
        }
        else if (e.Message == "Invalid cross reference table.")
        {
            //Show the error message to the user, when corrupted document is loaded.
            MainThread.BeginInvokeOnMainThread(() => MessageBox.Show("Error", "Failed to load the PDF document."));
            if (BindingContext is PdfViewerViewModel context)
            {
                context.PdfDocumentStream = null;
            }
        }
    }

    /// <summary>
    /// Handles when the password is entered.
    /// </summary>
    private void PasswordDialogBox_PasswordEntered(object sender, EventArgs e)
    {
        //Release the current waiting thread to execute.
        manualResetEvent.Set();
    }

    /// <summary>
    /// Handles when the message box's ok button is clicked.
    /// </summary>
    private void MessageBox_OKClicked(object sender, EventArgs e)
    {
        if (BindingContext is PdfViewerViewModel context)
        {
            // Retry loading the document.
            if (context.PdfDocumentStream != null)
                PdfViewer.DocumentSource = context.PdfDocumentStream;
        }
    }
}