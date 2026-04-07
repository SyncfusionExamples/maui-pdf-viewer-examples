# How to add text inside a polygon annotation in .NET MAUI PDF Viewer?

This project demonstrates how to add text inside a polygon annotation in [SfPdfViewer](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html).

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package.

## Steps

### 1. Install Required NuGet Package

Create a new [MAUI App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create) and install the [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package using either.
* NuGet Package Manager
* NuGet CLI

### 2. Initialize and Configure the PDF Viewer

Start by adding the Syncfusion PDF Viewer control to your XAML file.

**a. Add the Syncfusion namespace in `MainPage.xaml`**

This namespace enables access to the PDF Viewer control.

**XAML:**
```xaml
    xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```

**b. Add the PDF Viewer to your layout**

**XAML:**
```xaml
    <Grid>
    <syncfusion:SfPdfViewer x:Name="pdfViewer" ></syncfusion:SfPdfViewer>
    </Grid>
```

**c. Load the PDF in `MainPage.xaml.cs`**

**C#:**
```csharp
    public MainPage()
    {
        InitializeComponent();
            
        // Load a PDF document embedded in the project resources
        Stream? documentStream = this.GetType().Assembly.GetManifestResourceStream("TextPolygonSample.Assets.Annotations.pdf");

        // Load the PDF document into the SfPdfViewer
        PdfViewer.LoadDocument(documentStream);
    }
```

### 3. Define a field to hold the polygon bounds and thickness for positioning FreeText annotations

Declare fields in your `MainPage.xaml.cs` to store the bounds and thickness of the polygon annotation. These will be used to accurately position the free text annotation within the polygon.

**C#:**
```csharp
    RectF polygonBounds;
    float polygonThickness;
```

### 4. Convert the Polygon annotation coordinates to list of points

This method parses a string representation of polygon coordinates into a list of points.

**C#:**
```csharp
    public List<PointF> ConvertCoordinatesToPolygonPoints(string coordinatesData)
    {
        // List to hold the parsed points
        var points = new List<PointF>();

        // Split the coordinates data into commands based on the 'M' (move to) and 'L' (line to) commands
        string[] commands = coordinatesData.Split(new char[] { 'M', 'L' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var command in commands)
        {
            // Separate each command into X and Y coordinates
            string[] coords = command.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (coords.Length >= 2)
            {
                // Parse the coordinates and add to the list as a PointF object
                float x = float.Parse(coords[0], CultureInfo.InvariantCulture);
                float y = float.Parse(coords[1], CultureInfo.InvariantCulture);
                points.Add(new PointF(x, y));
            }
        }
        return points;
    }
```

### 5. Create a polygon annotation using the list of points

This method constructs a `PolygonAnnotation` object from the provided points and configures its appearance and properties.

**C#:**
```csharp
    private PolygonAnnotation CreatePolygonAnnotation(List<PointF> polygonPoints)
    {
        // Create the polygon annotation and set its properties
        PolygonAnnotation polygonAnnotation = new PolygonAnnotation(polygonPoints, 1);
        polygonAnnotation.Color = Colors.Black; // Set annotation color
        polygonAnnotation.Opacity = 25; // Set annotation opacity

        // Set the bounds and thickness for use in positioning other annotations
        polygonBounds = polygonAnnotation.Bounds;
        polygonThickness = polygonAnnotation.BorderWidth;

        // Set a solid border style for the polygon
        polygonAnnotation.BorderStyle = BorderStyle.Solid;

        return polygonAnnotation;
    }
```

### 6. Parse the coordinate, create a polygon annotation, and adds it to the PDF viewer

This method serves as a controller to parse the coordinate string, create the polygon annotation, and add it to the PDF viewer.

**C#:**
```csharp
    private void CreateAndAddPolygonAnnotation(string coordinate)
    {
        // Convert coordinate to a list of polygon points
        List<PointF> polygonPoints = ConvertCoordinatesToPolygonPoints(coordinate);

        // Create the polygon annotation from the points
        PolygonAnnotation polygonAnnotation = CreatePolygonAnnotation(polygonPoints);

        // Add the polygon annotation to the PDF document
        PdfViewer.AddAnnotation(polygonAnnotation);
    }
```

### 7. Converts coordinate to polygon points and adds polygon annotations

This method sets up the coordinates and triggers the creation and addition of the polygon annotation.

**C#:**
```csharp
    void ConvertAndAddPolygonAnnotation()
    {
        // Example coordinates for a polygon
        var coordinates = "M50 27L111 27L111 63L50 63";

        // Create and add polygon annotation based on coordinate
        CreateAndAddPolygonAnnotation(coordinates);
    }
```

### 8. Creates and adds a free text annotation inside the polygon annotation

This method creates a text annotation and positions it within the defined polygon annotation's bounds.

**C#:**
```csharp
    private void CreateAndAddFreeTextAnnotation()
    {
        // Text to be displayed within the free text annotation
        string text = "TOMTEST";

        // Calculate position and size of the free text annotation
        float FreeTextX = polygonBounds.X + polygonThickness;
        float FreeTextY = polygonBounds.Y + polygonThickness;
        float FreeTextWidth = polygonBounds.Width - polygonThickness;
        float FreeTextHeight = polygonBounds.Height - polygonThickness;

        // Define bounds for the free text annotation
        RectF freeTextBounds = new RectF(FreeTextX, FreeTextY, FreeTextWidth, FreeTextHeight);

        // Create a free text annotation and add it to the PDF viewer
        FreeTextAnnotation textAnnotation = new FreeTextAnnotation(text, 1, freeTextBounds);
        PdfViewer.AddAnnotation(textAnnotation);
    }
```

### 9. Call the `ConvertAndAddPolygonAnnotation` and `CreateAndAddFreeTextAnnotation` in the `MainPage` constructor.

The `MainPage` constructor initializes the page, loads the PDF document, and then calls the methods to add the polygon and free text annotations.

**C#:**
```csharp
    public MainPage()
    {
        InitializeComponent();
            
        // Load a PDF document embedded in the project resources
        Stream? documentStream = this.GetType().Assembly.GetManifestResourceStream("TextPolygonSample.Assets.Annotations.pdf");

        // Load the PDF document into the SfPdfViewer
        PdfViewer.LoadDocument(documentStream);

        // Call method to convert coordinates and add polygon annotations to the PDF
        ConvertAndAddPolygonAnnotation();

        // Create and add free text annotation within the polygon
        CreateAndAddFreeTextAnnotation();
    }
```

## Run the App

Build and run the application on you desired platform.