using Syncfusion.Maui.Themes;
using System.Reflection;

namespace StylingToolbarItem
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void GotoPdfViewerPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PDFViewerPage());
        }
        private void OnThemeChangeButtonClicked(object sender, EventArgs e)
        {
            if (Application.Current != null)
            {
                ICollection<ResourceDictionary> mergedDictionaries = Application.Current.Resources.MergedDictionaries;
                if (mergedDictionaries != null)
                {
                    var theme = mergedDictionaries.OfType<SyncfusionThemeResourceDictionary>().FirstOrDefault();
                    if (theme != null)
                    {
                        if (theme.VisualTheme == SfVisuals.MaterialLight)
                        {
                            theme.VisualTheme = SfVisuals.MaterialDark;
                            Application.Current.UserAppTheme = AppTheme.Dark;
                        }
                        else
                        {
                            theme.VisualTheme = SfVisuals.MaterialLight;
                            Application.Current.UserAppTheme = AppTheme.Light;
                        }
                    }
                }
            }
        }
    }
}
