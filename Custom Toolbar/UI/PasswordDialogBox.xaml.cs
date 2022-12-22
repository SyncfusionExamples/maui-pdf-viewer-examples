namespace CustomToolbar;

public partial class PasswordDialogBox : ContentView
{
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Occurs when the password is entered.
    /// </summary>
    public event EventHandler<EventArgs> PasswordEntered;

    /// <summary>
    /// Constructor for the password dialog box.
    /// </summary>
    public PasswordDialogBox()
    {
        InitializeComponent();
    }
    
    /// <summary>
    /// Handles when the cancel button is clicked.
    /// </summary>
    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        //Reset the values and close the dialog.
        Password = null;
        passwordBlock.Text = "";
        this.IsVisible = false;
    }

    private void OkButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(passwordBlock.Text) == false)
        {
            //Store the password.
            Password = passwordBlock.Text;

            //Reset the UI and close the dialog.
            passwordBlock.Text = "";
            this.IsVisible = false;

            //Fire the password entered event.
            PasswordEntered?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            //Show warning when password is empty.
            helperText.TextColor = Color.FromRgb(255,0,0);
            helperText.Text = "* Password cannot be empty";
        }
    }
    
    /// <summary>
    /// Handles when show password check box is checked or unchecked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkBox)
        {
            // Show password when checked and hide when unchecked.
            if (checkBox.IsChecked)
                passwordBlock.IsPassword = false;
            else
                passwordBlock.IsPassword = true;
        }
    }
    
    /// <summary>
    /// Handles when password text is changed.
    /// </summary>
    private void passwordBlock_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue) == false)
        {
            //Reset the helper text if the current password is not null or empty.
            helperText.Text = "";
        }
    }
}