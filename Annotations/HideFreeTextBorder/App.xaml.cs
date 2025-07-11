namespace HideFreeTextBorder
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Add license key");
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
