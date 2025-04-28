# Sign once, Apply Everywhere: Automating Multiple Signature Fields with .NET MAUI PDF Viewer

---

This repository contains the demo project for the blog post:  
**"Sign once, Apply Everywhere: Automating multiple signature fields with .NET MAUI PDF Viewer."**  
The project demonstrates capturing a signature once and programmatically applying it to multiple fields within a PDF document using [Syncfusion MAUI PDF Viewer](https://www.syncfusion.com/maui-controls/maui-pdf-viewer).

---

## NuGet Packages Used

- [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer)

---

## Usage

1. **Open a PDF Document:**  
   Launch the app. The PDF (e.g., Rental agreement) is loaded automatically in the Syncfusion PDF Viewer.

2. **Initiate Signature:**  
   Tap on a signature field within the document. This will show the custom signature capture popup instead of the default modal which uses SfSignaturePad.

3. **Draw Your Signature:**  
   Draw your signature in the popup.  
   - Tap **Clear** to erase and redraw.
   - Tap **Close** to cancel and exit the signature capture.

4. **Confirm Signature:**  
   Tap **OK** to confirm. Your signature will be programmatically added to the selected field.

5. **Apply to All Relevant Fields:**  
   After adding your signature, you’ll be prompted to apply the signature to all fields for the same user role (e.g., Tenant, Landlord, or Witness).  
   - Tap **Yes** to automatically apply your signature to every matching field that hasn’t been signed yet.
   - Tap **No** to keep the signature only on the selected field.

6. **Review the Document:**  
   Check that your signature appears in all relevant fields as expected. You can now continue working with or saving your signed PDF.