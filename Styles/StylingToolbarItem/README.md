# How to match the semantics of custom toolbar item with that of default one in .NET MAUI PDF Viewer?

This project demonstrate how to match the semantics of custom toolbar item with that of already existing toolbar item in the toolbar in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package.

## Steps

### 1. Install Required NuGet Package

Create a new [MAUI App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create) and install the [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package using either.

* NuGet Package Manager
* NuGet CLI

### 2. Add the Syncfusion.Maui.Themes namespace in `App.xaml`

This namespace enables access to the [SyncfusionThemeResourceDictionary](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.Themes.SyncfusionThemeDictionary.html).

**XAML:**

```xaml
     xmlns:syncTheme="clr-namespace:Syncfusion.Maui.Themes;assembly=Syncfusion.Maui.Core"
```

### 3. Merge the `SyncfusionThemeResourceDictionary` to the Application Resource.

To apply themes to your application, merge the [SyncfusionThemeResourceDictionary](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.Themes.SyncfusionThemeDictionary.html) item to the application resource.
   
**XAML:**

```xaml
    <Application  xmlns:syncTheme="clr-namespace:Syncfusion.Maui.Themes;assembly=Syncfusion.Maui.Core"
             ...>
        <Application.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <syncTheme:SyncfusionThemeResourceDictionary />
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
        </Application.Resources>
    </Application>
```

### 4. Create a new folder and add a `ContentPage` (PdfViewerPage) under the created folder

Create a ContentPage which serves as a destination page of the Navigation.

### 5. Create two button in the `MainPage.Xaml`

Create two buttons.

**XAML:**

```xaml
    <Grid RowSpacing="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <Button Text="Change Theme" Grid.Row="0" WidthRequest="150" HorizontalOptions="Center" VerticalOptions="End" HeightRequest="50"/>
        <Button Text="Go To PdfViewerPage" Grid.Row="1" WidthRequest="250" HorizontalOptions="Center" VerticalOptions="Start" HeightRequest="50"/>
    </Grid>
```

### 6. Create clicked event handlers

#### a. Event Handler for Theme Change Button

In this event handler, you can set the theme change code as below using the merged [SyncfusionThemeResourceDictionary](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.Themes.SyncfusionThemeDictionary.html). 

**C#**

```csharp
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
```

#### b. Event Handler for the Navigation button.

In this event handler, you can set the code `PDfViewerPage` navigation using the [Navigation.PushAsync](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.navigationpage.pushasync?view=net-maui-9.0) method.

**C#:**

```csharp
    private async void GotoPdfViewerPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PDFViewerPage());
    }
```

### 7. Wire the clicked event handlers for the respective buttons

**XAML:**

```xaml
    <Grid RowSpacing="10">
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>
        <Button Text="Change Theme" Clicked="OnThemeChangeButtonClicked"  Grid.Row="0" WidthRequest="150" HorizontalOptions="Center" VerticalOptions="End" HeightRequest="50"/>
        <Button Text="Go To PdfViewerPage" Clicked="GotoPdfViewerPage" Grid.Row="1" WidthRequest="250" HorizontalOptions="Center" VerticalOptions="Start" HeightRequest="50"/>
    </Grid>
```

### 8. Initialize and Configure the PDF Viewer

Start by adding the Syncfusion PDF Viewer control to your XAML file.

#### a. Add the Syncfusion.Maui.PdfViewer namespace in `PdfViewerPage.xaml`

This namespace enables access to the PDF Viewer control.

**XAML:**

```xaml
    xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```

#### b. Add the PDF Viewer to your layout

**XAML:**

```xaml
     <Grid>
        <syncfusion:SfPdfViewer x:Name="pdfViewer" ></syncfusion:SfPdfViewer>
     </Grid>
 ```

#### c. Load the PDF in `PdfViewerPage.Xaml.cs`

**C#:**

```csharp
    //Accessing the PDF document that is added as embedded resource as stream.
    Stream? documentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("StylingToolbarItem.Assets.PDF_Succinctly.pdf");

    // Assigning stream to "DocumentSource" property.
    pdfViewer.DocumentSource = DocumentStream;
```

### 9. Matching semantics of the custom toolbar item with default toolbar item

To match the text color of newly added toolbar item with that of default toolbar item in the toolbar, set the color of the [TextColor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.button.textcolor?view=net-maui-9.0) for the button as that of the default toolbar item using the [SetAppThemeColor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.bindableobjectextensions.setappthemecolor?view=net-maui-9.0) method. Refer to the below code example.

**C#:**

``` csharp
    // Create new open button
    fileOpenButton = new Button
    {
        Text = "\ue712", // Set button text
        FontSize = 24, // Set button text font size
        FontFamily = "MauiMaterialAssets", // Set button text font family
        BackgroundColor = Colors.Transparent, // Set background for the button
        BorderColor = Colors.Transparent, // Set border color for the button
        CornerRadius = 5, // Set corner radius of the button
    };

    //Set color based on theme.
    fileOpenButton.SetAppThemeColor(Button.TextColorProperty,
    Color.FromArgb("#49454F"),
    Color.FromArgb("#CAC4D0"));

#if !WINDOWS && !MACCATALYST
    // Inserting open file option button as toolbar item in the top toolbar for the mobile platform.
    PdfViewer?.Toolbars?.GetByName("TopToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "OpenFile"));
#else
    // Inserting open file option button as toolbar item in the primary toolbar for the desktop platform.
    PdfViewer?.Toolbars?.GetByName("PrimaryToolbar")?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "OpenFile"));
#endif
```

To set the tooltip to a newly added toolbar item like the default items in the toolbar, set the tooltip property for the newly added item. For more details about the tooltip, please refer Tooltips in .NET MAUI , refer to below code example.

**C#:**

``` csharp
    // Set the tooltip text
    ToolTipProperties.SetText(fileOpenButton, "OpenFile");
```

## Run the App

1. Build and run the application on all platforms.
2. Switch the theme and check the color and tooltip of the newly added item in the PdfViewer. 





