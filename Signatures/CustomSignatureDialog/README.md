# How to Create a Customized Signature Pad in the Application to Add Signatures to a PDF Document?

---

This repository contains the demo project for the KB article:  
**"How to create a customized signature pad in the application to add signatures to a PDF document?"**  
The project demonstrates how to create a customized signature pad in the application to add signatures to a PDF document using [Syncfusion MAUI PDF Viewer](https://www.syncfusion.com/maui-controls/maui-pdf-viewer).

---

## NuGet Packages Used

- [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer)

---

## Steps to Create a Custom Signature Dialog

1. **Include the Namespace in MainPage.xaml.cs File:**

   ```csharp
   using Syncfusion.Maui.SignaturePad;

2. **Create a Custom Signature Dialog Using SfSignaturePad:**

1. **Set Up the Grid Layout for the Dialog:**

- Create a Grid to organize the layout for the signature dialog.
- Define three rows for different controls and three columns for layout flexibility.

``` csharp
signaturelayout = new Grid
{
    HeightRequest = 500,
    RowDefinitions = { new RowDefinition { Height = 50 }, new RowDefinition { Height = GridLength.Star }, new RowDefinition { Height = 50 } },
    ColumnDefinitions = { new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = GridLength.Star } },
    BackgroundColor = Colors.Lavender,
    HorizontalOptions = LayoutOptions.Center,
    VerticalOptions = LayoutOptions.Center,
    Padding = new Thickness(5, 0)
};

// Dynamic width for phones
signaturelayout.WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Phone ? (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) : 500;

```
2.**Create and Arrange Controls in the Grid:**

- **Draw Label:** Indicates where to draw the signature.
- **Close Button:** Closes the dialog.
- **Signature Pad:** Area for drawing signatures.
- **Clear Button:** Clears the signature pad.
- **Create Button:** Saves the drawn signature.

``` csharp
Label drawLabel = new Label { Text = "Draw", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
Grid.SetRow(drawLabel, 0); Grid.SetColumn(drawLabel, 1);

Button closeButton = new Button { Text = "Close", BackgroundColor = Color.FromArgb("#6750A4"), HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
closeButton.Clicked += CloseButton_Clicked; Grid.SetRow(closeButton, 0); Grid.SetColumn(closeButton, 2);

signatureView = new SfSignaturePad { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Background = Colors.White };
Grid.SetRow(signatureView, 1); Grid.SetColumnSpan(signatureView, 3);

Button clearButton = new Button { Text = "Clear", BackgroundColor = Color.FromArgb("#6750A4"), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center };
clearButton.Clicked += ClearButton_Clicked; Grid.SetRow(clearButton, 2); Grid.SetColumn(clearButton, 0);

Button createButton = new Button { Text = "Create", BackgroundColor = Color.FromArgb("#6750A4"), TextColor = Colors.White, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
createButton.Clicked += CreateButton_Clicked; Grid.SetRow(createButton, 2); Grid.SetColumn(createButton, 2);

```
3. **Add Controls to the Signature Layout:**

Finally, all the components (label, buttons, and signature pad) are added to the grid layout, which is then added to the parent signature grid container.

```csharp

signaturelayout.Children.Add(drawLabel);
signaturelayout.Children.Add(closeButton);
signaturelayout.Children.Add(signatureView);
signaturelayout.Children.Add(clearButton);
signaturelayout.Children.Add(createButton);
signature.Children.Add(signaturelayout);

```
4. **Show the Signature Dialog:**
To show the signature dialog, set the [AnnotationMode](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.AnnotationMode.html) to signature when signature button is clicked. Use the following code:
```csharp 
// setting signature annotation mode when signature button is clicked.
  private void signatureButton_Clicked(object sender, EventArgs e)
  {
      pdfViewer.AnnotationMode = AnnotationMode.Signature;
  } 
```

To suppress the built-in signature dialog, wire the [SignatureModalViewAppearing](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_SignatureModalViewAppearing) event and set the visibility of `signaturelayout` to true in the `PdfViewer_SignatureModalViewAppearing` event handler. 

``` csharp
// Event for suppressing the built-in signature dialog.
pdfViewer.SignatureModalViewAppearing += PdfViewer_SignatureModalViewAppearing;

// Handle the custom signature dialog visiblity in the event handler.
private void PdfViewer_SignatureModalViewAppearing(object? sender, FormFieldModalViewAppearingEventArgs e)
{
   e.Cancel = true; // Cancel default dialog
   if (e.FormField != null && e.FormField is SignatureFormField formField)
   {
      signatureFormField = formField; // Store form field for signature placement
   }
   if (signaturelayout != null)
   {
      signaturelayout.IsVisible = true; // Show custom signature dialog
      signatureButton.IsVisible = false; // Hide signature button
   }
}
```
## Usage

1. **Open a PDF Document:**  
   Launch the app. The PDF (e.g., Rental agreement) is loaded automatically in the Syncfusion PDF Viewer.

2. **To Add Signature:**  
  To add a signature within the document. click the signature button in the bottom of the document.This will show the custom signature dialog instead of the default modal which uses SfSignaturePad.

3. **Draw Your Signature:**  
   Draw your signature in the dialog.  
   - Tap **Clear** to erase and redraw.
   - Tap **Close** to cancel and exit the signature dialog.

4. **Create Signature:**  
   Tap **Create** to create. Your signature will be created and can be added on the tapped position of the document.

5. **Delete the signature:**  
   If you want to change the signature or delete the signature, you can select the added signature and click the delete button in the top of the document in the toolbar. 

6. **Save the document:**  
   After adding the signature, you can save the signed PDF document by clicking the save button in the top of the document in the toolbar.

7. **Open new document:**  
   You can also open the new document by clicking the open button in the top of the document in the toolbar.
---