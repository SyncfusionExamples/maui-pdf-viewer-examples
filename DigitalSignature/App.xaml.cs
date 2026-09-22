using Microsoft.Extensions.DependencyInjection;

namespace DigitalSignature
{
    public partial class App : Application
    {
        public App()
        {
            //Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ix0oFS8QJAw9HSQvXkVhQlBad1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3tTfkdnWHZecXBSRWVVU091Wg==");
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}