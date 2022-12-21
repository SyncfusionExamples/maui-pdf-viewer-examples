namespace PageNavigation;

public partial class MessageBox : ContentView
{
    /// <summary>
    /// Occurs when the ok button is clicked.
    /// </summary>
    public event EventHandler<EventArgs> OkClicked;

    /// <summary>
    /// Constructor of the MessageBox
    /// </summary>
    public MessageBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles when Ok is clicked.
    /// </summary>
    private void Ok_Clicked(object sender, EventArgs e)
    {
        //Hide the message box.
        this.IsVisible = false;

        //Fire the Ok clicked event.
        OkClicked?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Show the message box with the given title and message.
    /// </summary>
    internal void Show(string title, string message, string closeText = "OK")
    {
        this.IsVisible = true;
        Title.Text = title;
        Message.Text = message;
        OkButton.Text = closeText;
    }
}