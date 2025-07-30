# .NET MAUI PDF Viewer with Thumbnail Preview

This sample demonstrates how to build a cross-platform PDF Viewer with a visual thumbnail navigation strip using Syncfusion's .NET MAUI UI components.

## Features
- Display any PDF document in a high-fidelity, cross-platform viewer.
- Generate and display thumbnail previews of each PDF page, shown in a horizontal scrollable strip.
- Tap/click a thumbnail to instantly navigate the viewer to that page.
- Thumbnail strip can be minimized or maximized for better workspace.
- Visual highlight indicates the currently selected/active page.
- Clean, modern MVVM architecture — easy to extend and maintain.
- Responsive and touch-friendly UI for desktop, tablet, and mobile.

## Technologies Used
- [.NET MAUI](https://dotnet.microsoft.com/en-us/apps/maui)
- [Syncfusion.Maui.PdfViewer](https://www.syncfusion.com/maui-controls/pdf-viewer)
- [Syncfusion.Maui.PdfToImageConverter](https://www.syncfusion.com/maui-controls/pdf-to-image-converter)

## Quick Start

1. **Clone or Download** this repository.
2. **Open in Visual Studio** with the .NET MAUI workload installed.
3. **Restore NuGet Packages** if necessary.
4. **Adjust Embedded PDF:**
    - Place your own PDF file in `Assets/` folder if you want and update the ViewModel resource name if needed.
5. **Run** on Android, iOS, Mac Catalyst, or Windows.

## Key Code Highlights

- ViewModel creates page thumbnails via `Syncfusion.Maui.PdfToImageConverter`, populates `ObservableCollection<PageThumbnail>`.
- UI (XAML) binds thumbnail list to a horizontal scrollable border-strip via `BindableLayout` and `FlexLayout`.
- Compiled bindings (`x:DataType`) used throughout for runtime safety and performance.
- Color, icon, and height converters enable clean MVVM-driven UI logic separation.
- All navigation, loading, and selection state are observable and refactor-friendly.

## Screenshot
<img src="Images/Thumbnail Navigation Gif.gif" Width="600"/>

## How It Works
- When the app loads a PDF, each page is converted asynchronously to an image stream.
- Thumbnails are displayed at the bottom; tap to navigate, or minimize the strip for more workspace.
- The selected page is highlighted with a blue border.

## Conclusion: Why Thumbnail Navigation?
Thumbnail previews enable fast, visual, photo-like navigation in complex documents. They dramatically improve user experience for any document longer than 10 pages—and are a best practice in reader apps, technical/PDF tools, and productivity solutions.