namespace CreatePDF
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MDAxQDMyMzgyZTMwMmUzMGFMV081MVlpVnJ5aHJyQzUzMFFBOUJwZ1B0YWkxNC80WnA3SHdvbmdqNzA9");
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
