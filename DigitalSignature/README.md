# Add digital signatures to PDF files in .NET-MAUI

This repository contains an example that demonstrates how to add digital signatures to PDF files using the Syncfusion&reg; .NET-MAUI PDF Viewer. The sample also shows how to open PDF files from, and save signed PDFs back to, local device storage.

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package.
3. The [Syncfusion.Maui.Core](https://www.nuget.org/packages/Syncfusion.Maui.Core) package.
4. The [CommunityToolkit.Maui](https://www.nuget.org/packages/CommunityToolkit.Maui) package.

## How to digitally sign PDF files using the .NET-MAUI PDF Viewer.

### 1. Install Required NuGet Package

Create a new [MAUI App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create), install the [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer), [Syncfusion.Maui.Core](https://www.nuget.org/packages/Syncfusion.Maui.Core) and [CommunityToolkit.Maui](https://www.nuget.org/packages/CommunityToolkit.Maui) packages using either.

* NuGet Package Manager
* NuGet CLI

### 2. Namespaces required

**C#:**

```csharp
    using CommunityToolkit.Maui.Storage;
    using Syncfusion.Maui.PdfViewer;
    using Syncfusion.Pdf.Security;
```

### 3. Initialize and Configure the PDF Viewer

Start by adding the Syncfusion PDF Viewer control to your XAML file.

#### a. Add the Syncfusion namespace in `MainPage.xaml`

This namespace enables access to the PDF Viewer control.

**XAML:**

```xaml
    xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```

#### b. Add the PDF Viewer to your layout

**XAML:**

```xaml
     <Grid>
        <syncfusion:SfPdfViewer x:Name="pdfViewer"
                                ShowToolbars="False"
                                DocumentSource="{Binding PdfDocumentStream}" />
     </Grid>
```

### 4. Enable digital signing in the PDF Viewer

In the code-behind, enable digital signature support on the PDF Viewer and subscribe to the `DigitalSignatureModalViewAppearing` event. This event is raised when the user taps an unsigned signature field, allowing the application to provide the certificate and signing options.

**C#:**

```csharp
    public MainPage()
    {
        InitializeComponent();

        pdfViewer.DigitalSignatureSettings.EnableValidation = true;
        pdfViewer.DigitalSignatureSettings.IsValidationBannerVisible = true;
        pdfViewer.DigitalSignatureSettings.EnableSigning = true;
        pdfViewer.DigitalSignatureModalViewAppearing += PdfViewer_DigitalSignatureModalViewAppearing;
        pdfViewer.SigningFailed += PdfViewer_SigningFailed;
    }
```

### 5. Provide the signing options

In the `DigitalSignatureModalViewAppearing` handler, build a `SigningOptions` instance and assign it to the `e.Options` property. The signing options include the signature field to be signed, the certificate stream and password, optional reason/location/contact information, the digest algorithm, the cryptographic standard, and the visual appearance of the signature.

**C#:**

```csharp
    private void PdfViewer_DigitalSignatureModalViewAppearing(object? sender, DigitalSignatureModalViewAppearingEventArgs e)
    {
        e.Options = new SigningOptions
        {
            SignatureField = e.SignatureField,
            CertificateStream = certificateStream,
            CertificatePassword = certificatePassword,
            Reason = Reason,
            LocationInfo = Location,
            ContactInfo = ContactInfo,
            DigestAlgorithm = DigestAlgorithm.SHA256,
            CryptographicStandard = CryptographicStandard.CADES,
            Appearance = new SignatureAppearanceSettings
            {
                ShowSignerName = true,
                ShowDate = true,
                ShowReason = true,
                ShowLocation = true
            }
        };
    }
```

### 6. Create open, save, and digital ID buttons.

Create buttons in the layout to open a PDF from local storage, configure a Digital ID for signing, save the signed PDF back to local storage, and toggle the signature panel.

**XAML:**

```xaml
    <Grid ColumnDefinitions="Auto,Auto,*,Auto,Auto">

        <!-- Open -->
        <Button Grid.Column="0"
                x:Name="OpenButton"
                Text="&#xE712;"
                FontFamily="MauiMaterialAssets"
                FontSize="24"
                Clicked="OnOpenClicked" />

        <!-- Digital ID -->
        <Button Grid.Column="1"
                x:Name="DigitalSignatureButton"
                Text="Digital ID"
                Clicked="OnDigitalSignatureClicked" />

        <!-- Save -->
        <Button Grid.Column="3"
                x:Name="SaveButton"
                Text="&#xe75f;"
                FontFamily="MauiMaterialAssets"
                FontSize="24"
                Clicked="OnSaveClicked" />

        <!-- Signature Panel -->
        <Button Grid.Column="4"
                x:Name="SignaturePanelButton"
                Text="&#xE737;"
                FontFamily="MauiMaterialAssets"
                FontSize="24"
                Clicked="OnSignaturePanelClicked" />
    </Grid>
```

### 7. Create event handler for the open button.

In the open button event handler, platform-specific file type filters are defined to ensure the file picker displays only compatible PDF files across different operating systems. The file picker is then configured with a custom title and the appropriate file type settings. Once launched, it waits for the user to select a PDF file. After selection, the application opens a read stream from the chosen file and loads it in the PDF Viewer control using the [LoadDocument](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_LoadDocument_System_IO_Stream_System_String_System_Nullable_Syncfusion_Maui_PdfViewer_FlattenOptions__) method.

**C#:**

```csharp
    private async void OnOpenClicked(object? sender, EventArgs? e)
    {
        // Define platform-specific file types for the file picker.
        FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                { DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                { DevicePlatform.Android, new[] { "application/pdf" } },
                { DevicePlatform.WinUI, new[] { "pdf" } },
                { DevicePlatform.MacCatalyst, new[] { "pdf" } },
            });

        // Configure the file picker options.
        PickOptions options = new()
        {
            PickerTitle = "Choose a PDF file",
            FileTypes = pdfFileType,
        };

        // Launch the file picker and wait for user selection.
        var result = await FilePicker.Default.PickAsync(options);

        // Check if a file was selected.
        if (result != null && result.FileName != null &&
            result.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            currentFileName = result.FileName;

            // Get the stream.
            Stream pdfStream = await result.OpenReadAsync();

            // Load the Pdf in the PdfViewer control using LoadDocument method.
            pdfViewer.LoadDocument(pdfStream);
        }
    }
```

### 8. Configure the Digital ID for signing

Open a dialog that lets the user pick a `.pfx` or `.p12` certificate file, enter its password, and optionally supply location, reason, and contact information. The selected values are then used to populate the `SigningOptions` in step 5 when the user taps a signature field.

**C#:**

```csharp
    private async void OnSelectCertificateClicked(object? sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select Certificate File"
            });

            if (result != null)
            {
                _certificateStream = await result.OpenReadAsync();
                if (result.FileName.EndsWith(".pfx", StringComparison.OrdinalIgnoreCase) ||
                    result.FileName.EndsWith(".p12", StringComparison.OrdinalIgnoreCase))
                {
                    CertificateFileName.Text = result.FileName;
                }
                else
                {
                    // Show an inline validation error for invalid file.
                    _certificateStream = null;
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error", $"Failed to select certificate: {ex.Message}", "OK");
        }
    }
```

### 9. Create event handler for the save button.

In the save button event handler, a memory stream is created and the current document content is saved into the memory stream, then saved to local storage using the `SaveAsync` method in the CommunityToolkit.Maui library. The resulting file path is displayed to the user.

**C#:**

```csharp
    private async void OnSaveClicked(object? sender, EventArgs? e)
    {
        // Create a new memory stream to hold the saved PDF document.
        Stream saveDocumentStream = new MemoryStream();

        // Asynchronously save the current document content into the memory stream.
        await pdfViewer.SaveDocumentAsync(saveDocumentStream);
        saveDocumentStream.Position = 0;

        // Save the PDF in the local storage using the SaveAsync method in the FileSaver
        // class present in the CommunityToolkit.Maui source.
        if (!string.IsNullOrEmpty(currentFileName))
        {
            var fileSaverResult = await FileSaver.Default.SaveAsync(currentFileName, saveDocumentStream);

            if (fileSaverResult.IsSuccessful)
            {
                string filePath = string.IsNullOrEmpty(fileSaverResult.FilePath)
                    ? "Path information is not available."
                    : fileSaverResult.FilePath;

                await DisplayAlertAsync("File Saved", $"The file is saved to:\n{filePath}", "OK");
            }
            else
            {
                await DisplayAlertAsync(
                    "Error",
                    fileSaverResult.Exception?.Message ?? "Save failed.",
                    "OK");
            }
        }
    }
```

### 10. Toggle the signature panel

Use the `IsSignaturePanelVisible` property on the PDF Viewer's `DigitalSignatureSettings` to show or hide the panel that lists the signature fields present in the loaded PDF.

**C#:**

```csharp
    private void OnSignaturePanelClicked(object? sender, EventArgs? e)
    {
        pdfViewer.DigitalSignatureSettings.IsSignaturePanelVisible =
            !pdfViewer.DigitalSignatureSettings.IsSignaturePanelVisible;
    }
```

### 11. Handle signing failures

Subscribe to the `SigningFailed` event of the PDF Viewer to surface any error that occurs while applying a signature, including the error type, message, and inner exception.

**C#:**

```csharp
    private async void PdfViewer_SigningFailed(object? sender, SigningFailedEventArgs e)
    {
        string message =
            $"Error Type: {e.ErrorType}\n\n" +
            $"Message: {e.ErrorMessage}";

        if (e.InnerException != null)
        {
            message += $"\n\nException: {e.InnerException.Message}";
        }

        await DisplayAlertAsync("Signing Failed", message, "OK");
    }
```

### Conclusion

We hope you enjoyed learning how to add digital signatures to PDF files using the .NET-MAUI PDF Viewer, and how to open and save PDF files from and to local device storage.

Refer to our [.NET MAUI PDF Viewer's feature tour](https://www.syncfusion.com/maui-controls/maui-pdf-viewer) page to learn about its other groundbreaking feature representations. You can also explore our [.NET MAUI PDF Viewer Documentation](https://help.syncfusion.com/maui/pdf-viewer/getting-started) to understand how to present and manipulate data.

For current customers, check out our .NET MAUI components on the [License and Downloads](https://www.syncfusion.com/sales/teamlicense) page. If you are new to Syncfusion, try our 30-day [free trial](https://www.syncfusion.com/downloads/maui) to explore our .NET MAUI PDF Viewer and other .NET MAUI components.

Please let us know in the following comments if you have any queries or require clarifications. You can also contact us through our [support forums](https://www.syncfusion.com/downloads/maui), [support ticket](https://support.syncfusion.com/create) or [feedback portal](https://www.syncfusion.com/feedback/maui). We are always happy to assist you!