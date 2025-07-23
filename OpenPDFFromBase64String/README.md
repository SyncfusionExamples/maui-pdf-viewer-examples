# How to open a PDF document from a base64 string?
This project demonstrates how to open a PDF document from a given base64 string text by converting it to a byte[] and assigning the obtained byte[] to the DocumentSource property.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.
3. Base64 string converted PDF text file in Assets folder. (In your project, create a folder named Assets (if it doesn't already exist). Add the text file containing base64 string text file (For example PDFEncodedBase64String.txt) in the Assets folder of your project, and set its Build Action to EmbeddedResource. (The pdf which is converted as base64 string text is [PDF_Succinctly1928776572](https://s3.amazonaws.com/files2.syncfusion.com/dtsupport/directtrac/general/pd/PDF_Succinctly1928776572.pdf?AWSAccessKeyId=AKIAWH6GYCX354WITGDG&Expires=1753248365&Signature=ra3pQ1YDz2FAUjGvceLmZHTl834%3D) which has 62 pages.))

## Steps 
1. Get the current executing assembly.
```csharp
    // Get the current executing assembly
    var assembly = Assembly.GetExecutingAssembly();
```
2. Open a StreamReader to read the embedded resource text file which has base64 string content.
```csharp
    // Open a StreamReader to read the embedded resource named "OpenPDFFromBase64String.PDFEncodedBase64String.txt" which has base64 string content.
    using var reader = new StreamReader(
        assembly.GetManifestResourceStream("OpenPDFFromBase64String.Assets.PDFEncodedBase64String.txt")!);;
```
3. Read the entire Base64 string content from the embedded text file.
```csharp
    // Read the entire Base64 string content from the embedded text file.
    string base64Text = reader.ReadToEnd();
```
4. Decode the Base64 string into a byte array..
```csharp
    // Decode the Base64 string into a byte array.
    byte[] decodedBytes = Convert.FromBase64String(base64Text);
```
5. return the byte array.
```csharp
    // return the byte array.
    return decodedBytes;
```

## Run the App
Build and run the application on all platforms.




