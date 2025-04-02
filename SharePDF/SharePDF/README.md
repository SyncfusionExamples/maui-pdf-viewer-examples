# How to share the PDF document with other apps using .NET MAUI PDF Viewer?

This project demonstrates how to share a PDF document with other apps in .NET MAUI PDF Viewer application using Syncfusion PDF Viewer. The application loads a PDF file and enables users to share it through a button click.

# Steps to share the PDF document with other apps using .NET MAUI PDF Viewer.
1. Load a PDF in the Syncfusion PDF Viewer.
2. Save the PDF document to a local file.
3. Share the PDF using the mobile platform’s share functionality.

# Prerequisites
1. .NET MAUI installed.
2. Syncfusion.Maui.PdfViewer NuGet package.

# Steps
## 1. Set Up the PDF Viewer
In your MainPage.xaml.cs, the PDF Viewer is initialized with a PDF document embedded in your resources.

```csharp
pdfViewer.DocumentSource = this.GetType().Assembly.GetManifestResourceStream("SharePDF.Assets.PDF_Succinctly.pdf");
```
## 2. To Share PDF 
The SharePdf_Clicked method handles sharing the PDF:

1. Save the PDF to the app's data directory.
2. Check if the file exists.
3. Copy the file to a temporary location.
4. Prepare and execute the share request.

```csharp
private async void SharePdf_Clicked(object sender, EventArgs e)
{
    try
    {
        string fileName = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, "ModifiedDocument.pdf");
        using (FileStream pdfStream = File.Create(fileName))
        {
            pdfViewer.SaveDocument(pdfStream);
        }

        if (File.Exists(fileName))
        {
            var tempFilePath = System.IO.Path.Combine(FileSystem.CacheDirectory, "ModifiedDocument.pdf");
            File.Copy(fileName, tempFilePath, true);

            var request = new ShareFileRequest
            {
                Title = "Share PDF File",
                File = new ShareFile(tempFilePath)
            };

            await Microsoft.Maui.ApplicationModel.DataTransfer.Share.RequestAsync(request);
        }
        else
        {
            await DisplayAlert("Error", "PDF file not found.", "OK");
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", ex.Message, "OK");
    }
}
```
# Run the App
1. Build and run the application on your preferred platform.
2. Load the PDF in the viewer.
3. Click the share button to share the document.