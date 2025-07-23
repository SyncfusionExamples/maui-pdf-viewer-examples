# How to open a PDF document from a base64 string?
This project demonstrates how to open a PDF document from a given base64 string text by converting it to a byte[] and assigning the obtained byte[] to the DocumentSource property.

## Prerequisites
1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Code logic
Decode the Base64 string into a byte array using the Convert class from the System namespace, which is designed for data type conversion. In this case, a Base64-encoded string and converts it back into its original binary format.
```csharp
    // Decode the Base64 string into a byte array.
    byte[] decodedBytes = System.Convert.FromBase64String(base64Text);
```

## Run the App
Build and run the application on all platforms.
