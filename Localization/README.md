# Localizing Texts in Syncfusion PDF Viewer for .NET MAUI

This sample demonstrates how to localize the UI texts of the Syncfusion PDF Viewer (Syncfusion.Maui.PdfViewer) in a .NET MAUI application with multi-language support. Users can select a language from the main page and view the PDF with localized UI text.

## Prerequisites

- .NET MAUI development environment installed.
- Syncfusion.Maui.PdfViewer NuGet package added to the project.
- Basic knowledge of .resx resource files and localization in .NET.

## Implementation Steps

### 1. Resource Files

Three resource files are included to support localization:
- **SfPdfViewer.en-US.resx** - English language strings
- **SfPdfViewer.de.resx** - German language strings
- **SfPdfViewer.ar-AE.resx** - Arabic language strings

Each resource file contains the same keys with translated values for Syncfusion PDF Viewer UI elements.

### 2. App.xaml.cs - Culture Configuration

The `App` class includes a `SetCulture()` method that sets both the default thread culture and UI culture:

```csharp
using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Resources;

namespace Localization
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();       

            SetCulture("en-US"); // Set default culture
        }

        public void SetCulture(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
```

### 3. PdfViewerViewModel.cs - Language Selection

The ViewModel manages available languages and the currently selected language:

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Localization
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private string? selectedLanguage;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PdfViewerViewModel()
        {
            // Populate available languages.
            Languages = new ObservableCollection<string> { "English", "German", "Arabic" };

            // Set default language.
            SelectedLanguage = "English";
        }

        public ObservableCollection<string> Languages { get; }

        public string? SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

### 4. MainPage.xaml.cs - Language Selection Interface

The MainPage displays available languages and opens the PDF Viewer with the selected language:

```csharp
namespace Localization
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OpenPdfViewerPage(object sender, EventArgs e)
        {
            // Use empty string as default if SelectedLanguage is null
            var language = viewModel.SelectedLanguage ?? string.Empty;

            var pdfPage = new SfPdfViewerPage(language);
            await Navigation.PushAsync(pdfPage);
        }
    }
}
```

### 5. SfPdfViewerPage.xaml.cs - Localized PDF Viewer

The SfPdfViewerPage applies the selected language localization and loads the PDF:

```csharp
using Syncfusion.Maui.PdfViewer;
using System.Reflection;
using System.Resources;

namespace Localization;

public partial class SfPdfViewerPage : ContentPage
{
    public SfPdfViewerPage(string selectedLanguage)
    {
        // Map language name to culture code
        string cultureCode = selectedLanguage switch
        {
            "German" => "de-DE",
            "Arabic" => "ar-AE",
            _ => "en-US",
        };

        // Set the culture
        if (Application.Current is App app)
        {
            app.SetCulture(cultureCode);
        }

        // Configure the ResourceManager for Syncfusion PDF Viewer localization
        SfPdfViewerResources.ResourceManager = new ResourceManager(
            "Localization.Resources.SfPdfViewer",
            typeof(App).Assembly
        );

        InitializeComponent();
        LoadPdfWithLocalization();    
    }
    
    protected async override void OnDisappearing()
    {
        base.OnDisappearing();
        await pdfViewer.UnloadDocumentAsync();
    }

    private async void LoadPdfWithLocalization()
    {        
        // Load the embedded PDF document
        var documentStream = typeof(App).GetTypeInfo().Assembly
            .GetManifestResourceStream("Localization.Assets.PDF_Succinctly.pdf");
        await pdfViewer.LoadDocumentAsync(documentStream);
    }
}
```

## How It Works

1. **MainPage** displays a list of available languages (English, German, Arabic).
2. User selects a language from the list.
3. User taps the "Open PDF Viewer" button.
4. **SfPdfViewerPage** is created with the selected language passed as a parameter.
5. The page maps the language name to a culture code (en-US, de-DE, ar-AE).
6. **SetCulture()** is called to apply the culture to the application thread.
7. The ResourceManager is configured to use the corresponding `.resx` resource file.
8. The PDF is loaded from the embedded resources with localized UI text.

## Running the Application

1. Build and run the application.
2. The MainPage opens showing available languages.
3. Select a language from the list.
4. Tap the "Open PDF Viewer" button.
5. The PDF Viewer opens with the UI text localized to the selected language.
6. The toolbar buttons, menu items, and other UI elements display in the selected language.

## Adding a New Language

To add support for a new language:

1. Create a new resource file named `SfPdfViewer.{culture-code}.resx` in the `Resources` folder.
   - Example: `SfPdfViewer.fr-FR.resx` for French.
2. Add the same keys and their French translations.
3. Update the `SfPdfViewerPage.xaml.cs` switch statement to map the language to the culture code.
4. Add the language name to the `Languages` collection in `PdfViewerViewModel.cs`.