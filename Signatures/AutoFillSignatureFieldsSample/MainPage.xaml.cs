using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace SignatureDemo
{
    public partial class MainPage : ContentPage
    {
        FormField? focusedSignatureFormField = null;

        public MainPage()
        {
            InitializeComponent();
            // Load the PDF document. Specifies the path within the assembly.
            pdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("SignatureDemo.Assets.Rental agreement.pdf");
        }

        // Handles when the signature modal view is about to appear
        private void pdfViewer_SignatureModalViewAppearing(object sender, FormFieldModalViewAppearingEventArgs e)
        {
            e.Cancel = true; // Cancel the default modal view
            focusedSignatureFormField = e.FormField; // Store the currently focused form field
            signatureCaptureView.IsVisible = true; // Show the custom signature capture view
        }

        // Handle the SignatureCreated event from the signature capture view
        private async void SignatureCaptureView_SignatureCreated(object sender, ImageSource e)
        {
            if (focusedSignatureFormField != null) // Ensure a form field is focused
            {
                // Add the signature to the focused form field
                AddSignToSignatureFormField(focusedSignatureFormField, e);

                // Determine the user role based on the form field name
                string userRole = GetUserRoleFromField(focusedSignatureFormField.Name);

                // Identify relevant fields for the same user role to apply the signature
                var relevantFields = pdfViewer.FormFields
                    .Where(field => field.Name.Contains(userRole) && field is SignatureFormField)
                    .Cast<SignatureFormField>();

                if (relevantFields.Count() > 0)
                {
                    // Prompt the user for applying the signature across relevant fields
                    bool applyEverywhere = await DisplayAlert("Signature Created",
                        $"Signature for {userRole} successfully created. Would you like to apply this signature to all relevant fields?",
                        "Yes", "No");

                    if (applyEverywhere)
                    {
                        int updatedFieldsCount = 0; // Track the number of fields updated

                        foreach (var field in relevantFields)
                        {
                            if (field.Signature == null) // Only add signature if not already signed
                            {
                                AddSignToSignatureFormField(field, e);
                                updatedFieldsCount++;
                            }
                        }

                        // Show a success message with the number of fields updated
                        await DisplayAlert("Success",
                            $"{updatedFieldsCount} fields updated with the signature for {userRole}.",
                            "OK");
                    }
                }
            }
        }

        // Determine the user role based on the field name (e.g., Tenant or Landlord)
        private string GetUserRoleFromField(string fieldName)
        {
            if (fieldName.ToLower().Contains("tenant"))
                return "Tenant";
            else if (fieldName.ToLower().Contains("landlord"))
                return "Landlord";
            else if (fieldName.ToLower().Contains("witness"))
                return "Witness";
            return "User";
        }

        // Adds a signature image to a PDF signature form field
        async void AddSignToSignatureFormField(FormField formField, ImageSource sign)
        {
            // Check if the form field is a signature field
            if (formField is SignatureFormField signatureFormField)
            {
                // Check if the provided signature image is a StreamImageSource
                if (sign is StreamImageSource streamImageSource)
                {
                    // Get the image stream from the image source
                    Stream? imageStream = await streamImageSource.Stream(CancellationToken.None);
                    imageStream.Position = 0; // Reset the stream position before reading

                    // Create a new memory stream to copy the image data
                    Stream copiedStream = new MemoryStream();
                    await imageStream.CopyToAsync(copiedStream); // Copy the image data
                    copiedStream.Position = 0; // Ensure the copied stream is at the beginning

                    // Create a stamp annotation from the signature image.
                    // The RectF determines the location and size (here: X=0, Y=0, Width=300, Height=90).
                    StampAnnotation stampAnnotation = new StampAnnotation(
                        copiedStream,
                        formField.PageNumber,
                        new RectF(0, 0, 300, 90))
                    {
                        IsSignature = true, // Mark this annotation as a signature
                        IsLocked = true     // Lock the annotation to prevent further editing
                    };

                    // Assign the stamp annotation as the signature on this form field
                    signatureFormField.Signature = stampAnnotation;
                }
            }
        }
    }
}