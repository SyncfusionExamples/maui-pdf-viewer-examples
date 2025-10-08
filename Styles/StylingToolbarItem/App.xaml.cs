namespace StylingToolbarItem
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ix0oFS8QJAw9HSQvXkVhQlBad1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3tTfkdnWHdbd3dWRGZUWU91Xg==");
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}