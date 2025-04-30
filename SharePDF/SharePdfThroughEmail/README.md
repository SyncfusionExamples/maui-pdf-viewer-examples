# How to Open the Default Mail App with a PDF Attachment from PDF Viewer in .NET MAUI?

This project demonstrates how to build a .NET MAUI application that opens the device’s default mail app and attaches a PDF document currently loaded in the Syncfusion MAUI PDF Viewer.

# Steps to open mail app with PDF attachment using .NET MAUI PDF Viewer. 
1. Load a PDF in the Syncfusion PDF Viewer.
2. Save the PDF document to a local file.
3. Share the PDF using the devcie's default mail app.

# Prerequisites
1. .NET MAUI installed.
2. Syncfusion.Maui.PdfViewer NuGet package.

# Steps
## 1. Set Up the PDF Viewer
In your MainPage.xaml.cs, the PDF Viewer is initialized with a PDF document embedded in your resources.

```csharp
pdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("SharePdfThroughEmail.Assets.PDF_Succinctly.pdf");
```
## 2. To Share PDF via mail

The SharePDFThroughMail_Clicked method handles sharing the PDF using mail:

1. Access the saved stream from the [SaveDocument](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_SaveDocument_System_IO_Stream_) API to acquire the PDF content.
2. Generate a temporary file to hold the PDF data extracted from the saved stream.
3. Check if the default mail service allows email composition using the [Email.Default.IsComposeSupported](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.applicationmodel.communication.iemail.iscomposesupported?view=net-maui-9.0#microsoft-maui-applicationmodel-communication-iemail-iscomposesupported) API.
4. Create an [EmailMessage](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.applicationmodel.communication.emailmessage) with a descriptive subject, message body, and identified recipient.
5. Attach the previously generated file to the [EmailMessage.Attachments](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.applicationmodel.communication.emailmessage.attachments?view=net-maui-9.0#microsoft-maui-applicationmodel-communication-emailmessage-attachments). 
6. To enable email functionality, platform-specific configuration is required. For detailed instructions, please refer to the [Getting started](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/communication/email?view=net-maui-9.0&tabs=android#get-started) guide.

```csharp
 private async void SharePDFThroughMail_Clicked(object sender, EventArgs e)
 {
     try
     {
         // Define the file name and path for the saved PDF document.
         string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "ModifiedDocument.pdf");

         // Save the PDF document to the specified file location.
         using (FileStream pdfStream = File.Create(fileName))
         {
             // Using pdfViewer instance to save the PDF document to a stream.
             pdfViewer.SaveDocument(pdfStream);
         }

         // Verify if the file exists at the specified path.
         if (File.Exists(fileName))
         {
             // Define a temporary file path for the PDF content to be used for email attachment.
             var tempFilePath = Path.Combine(FileSystem.CacheDirectory, "ModifiedDocument.pdf");

             // Copy the PDF content to the temporary file to prepare for email attachment.
             File.Copy(fileName, tempFilePath, true);

             // Check if email composition is supported on the device.
             if (Email.Default.IsComposeSupported)
             {
                 // Define the email's subject and body text.
                 string subject = "PDF Document Shared through MAUI SfPdfViewer";
                 string body = "Please find the PDF document attached.";

                 // Create the email message with the subject, body, and recipient.
                 var message = new EmailMessage
                 {
                     Subject = subject,
                     Body = body,
                     BodyFormat = EmailBodyFormat.PlainText,
                     To = new List<string> { "recipient@example.com" } // Placeholder recipient email
                 };

                 // Attach the modified PDF file to the email message.
                 message.Attachments?.Add(new EmailAttachment(tempFilePath));

                 // Open the default mail application to compose the email with the attached PDF.
                 await Email.Default.ComposeAsync(message);
             }
             else
             {
                 // Display an alert if email composition is not supported on the device.
                 await DisplayAlert("Email Not Supported", "This device does not support email composition.", "OK");
             }
         }
         else
         {
             // Display an alert if the PDF file does not exist.
             await DisplayAlert("File Error", "The modified PDF file could not be located.", "OK");
         }
     }
     catch (Exception ex)
     {
         // Handle exceptions and display any error messages to the user.
         await DisplayAlert("An error occurred", ex.Message, "OK");
     }
 }
```
# Run the App
1. Build and run the application on your preferred platform.
2. Load the PDF in the viewer.
3. Click the share button to email the document.