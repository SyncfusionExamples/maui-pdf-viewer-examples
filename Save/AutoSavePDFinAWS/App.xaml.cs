namespace AutoSavePDFinAWS
{
    public partial class App : Application
    {
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF1cWWhAYVJ0WmFZfVtgcV9GYlZRRmYuP1ZhSXxWdk1iUH9YdHNQTmlZUEV9XEI=");
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}