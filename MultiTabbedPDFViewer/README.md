# How to use SfPdfViewer inside SfTabView?

This project demonstrates how to initialize a .NET MAUI PdfViewer inside the SfTabView control.

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Steps

### 1.Install Required NuGet Package

To get started, create a new [Maui App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create) and ensure the following package is installed in your .NET MAUI project:

[Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer)

You can install this package using the NuGet Package Manager or the NuGet CLI.

### 2. Initialize the TabView in XAML

Start by adding the Syncfusion TabView control to your XAML file.

**a. Add the Syncfusion namespace in your MainPage.xaml:**

Define the XAML namespace to enable access to the TabView.

**XAML:**

```xaml
    xmlns:tabView="clr-namespace:Syncfusion.Maui.TabView;assembly=Syncfusion.Maui.TabView"
```

**b. Add the TabView control to your layout:**

Initialize the TabView in the XAML file. This will display the TabView with two tab item in your app.

**XAML:**

```xaml
     <Grid>
        <tabView:SfTabView IndicatorBackground="{DynamicResource PrimaryLight}"
                           IndicatorPlacement="Top"
                           TabBarPlacement="Bottom"
                          >
            <tabView:SfTabItem Header="doc 1" x:Name="tab1">
            </tabView:SfTabItem>
            <tabView:SfTabItem Header="doc 2" x:Name="tab2">
            </tabView:SfTabItem>
        </tabView:SfTabView>
     </Grid>
```

### 3. Initialize the PdfViewer.

**a. Add the Syncfusion namespace in your MainPage.xaml:**

Define the XAML namespace to enable access to the PdfViewer.

**XAML:**

```xaml
    xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```
    
**b. Create instance of PdfViewer control in you MainPage.cs:**

**C#:**

```csharp
    // Create two instances of SfPdfViewer for displaying PDF documents
    SfPdfViewer pdfViewer = new SfPdfViewer();
    SfPdfViewer pdfViewer1 = new SfPdfViewer();
```

### 4. Set the ZoomMode for the two PdfViewer instance in the `MainPage.cs` file.

In the MainPage constructor, set the [ZoomMode](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ZoomMode.html#fields) for the two PdfViewer instance as [ZoomMode.FitToWidth](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.ZoomMode.html#Syncfusion_Maui_PdfViewer_ZoomMode_FitToWidth).

**C#:**

```csharp
    // Set the zoom mode of both PDF viewers instance to fit the width of the container
    pdfViewer.ZoomMode = ZoomMode.FitToWidth;
    pdfViewer1.ZoomMode = ZoomMode.FitToWidth;
```

### 5. Loading the pdfViewer in different tabs using the SfTabView Loaded event handler.

In the SfTabView `Loaded` event handler, evaluate the header text of each SfTabItem. Based on the header, assign the appropriate PDF stream to the [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource) property of the corresponding SfPdfViewer instance. Then, set that PdfViewer instance as the content of the respective SfTabItem

**C#:**

```csharp
    async void SfTabView_Loaded(System.Object sender, System.EventArgs e)
    {
        // Ensure the viewModel is not null before proceeding
        if (viewModel != null)
        {
            // Check if the first tab's header matches "doc 1"
            if (tab1.Header.Equals("doc 1"))
            {
                await Task.Delay(80); // Slight delay to ensure UI is ready
                pdfViewer.DocumentSource = viewModel.PDFDocumentSource; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                tab1.Content = pdfViewer; // Set the content of tab1 to the pdfViewer.
            }

            // Check if the first tab's header matches "doc 2"
            if (tab2.Header.Equals("doc 2"))
            {
                await Task.Delay(80); // Slight delay to ensure UI is ready
                pdfViewer1.DocumentSource = viewModel.PDFDocumentSource2; // Assign the stream to the "DocumentSource" property of the PdfViewer control
                tab2.Content = pdfViewer1; // Set the content of tab1 to the pdfViewer1.
            }
        }
    }
```

### 6. Subscription of TabView Loaded event. 

For to load the pdf document in the TabView, wire the Loaded event

**XAML:**

```xaml
    <Grid>
        <tabView:SfTabView IndicatorBackground="{DynamicResource PrimaryLight}"
                           IndicatorPlacement="Top"
                           TabBarPlacement="Bottom"
                           Loaded="SfTabView_Loaded"
                          >
            <tabView:SfTabItem Header="doc 1" x:Name="tab1">
            </tabView:SfTabItem>
            <tabView:SfTabItem Header="doc 2" x:Name="tab2">
            </tabView:SfTabItem>
        </tabView:SfTabView>
    </Grid>
```

## Run the App

1. Build and run the application in all the platforms.
2. Switch the tabs.




