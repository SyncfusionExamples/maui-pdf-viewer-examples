namespace ViewOnlyPdf;

public partial class AccessSettingsPage : ContentPage
{
    public AccessSettingsPage(DocumentAccessPolicy policy)
    {
        InitializeComponent();
        BindingContext = policy;
    }
}
