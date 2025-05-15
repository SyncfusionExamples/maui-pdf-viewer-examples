using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.SignaturePad;

namespace CustomSignatureDialog
{
    public partial class MainPage : ContentPage
    {
        // Fields for managing annotations, signature input, and layout configuration
        Annotation? selectedAnnotation;
        SfSignaturePad? signatureView;
        Grid? signaturelayout;
        Stream? signatureStream;
        float imageWidth, imageHeight;
        SignatureFormField? signatureFormField;

        // Constructor: Initializes components and subscribes to event handlers
        public MainPage()
        {
            InitializeComponent();
            CreateCustomSignatureDialog(); // Initializes custom signature dialog components
            pdfViewer.SignatureModalViewAppearing += PdfViewer_SignatureModalViewAppearing; // Set up event to suppress default signature dialog
        }

        // Event handler triggered when the PDF viewer is tapped for placing a signature
        private void PdfViewerTapped(object sender, GestureEventArgs e)
        {
            if (signatureStream != null)
            {
                // Create a stamp annotation using the drawn signature's stream
                StampAnnotation stamp = new StampAnnotation(signatureStream, e.PageNumber, new RectF(e.PagePosition.X - imageWidth / 2, e.PagePosition.Y - imageHeight / 2, imageWidth, imageHeight));
                stamp.IsSignature = true; // Mark as signature annotation
                pdfViewer.AddAnnotation(stamp); // Add annotation to the PDF
                signatureStream = null; // Clear stream after use
            }
        }

        // Event handler to cancel the default signature dialog and show a custom dialog
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

        // Method to create the custom signature dialog layout
        private void CreateCustomSignatureDialog()
        {
            if (signatureView == null)
            {
                // Configure the layout grid for the custom signature dialog
                signaturelayout = new Grid
                {
                    HeightRequest = 500, // Fixed height for dialog
                    RowDefinitions = // Define rows
                    {
                        new RowDefinition { Height = new GridLength(50) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(50) }
                    },
                    ColumnDefinitions = // Define columns
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    BackgroundColor = Colors.Lavender, // Set lavender background color
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(5, 0)
                };
                signaturelayout.WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Phone ? (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) : 500; // Dynamic width based on device

                // Add UI components to the grid
                Label drawLabel = new Label { Text = "Draw Signature", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                Grid.SetRow(drawLabel, 0);
                Grid.SetColumn(drawLabel, 1);

                Button closeButton = new Button { Text = "Close", HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromArgb("#6750A4") };
                closeButton.Clicked += CloseButton_Clicked; // Attach event to close dialog
                Grid.SetRow(closeButton, 0);
                Grid.SetColumn(closeButton, 2);

                signatureView = new SfSignaturePad { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Background = Colors.White };
                Grid.SetRow(signatureView, 1);
                Grid.SetColumnSpan(signatureView, 3); // Span across all columns

                Button clearButton = new Button { Text = "Clear", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromArgb("#6750A4") };
                clearButton.Clicked += ClearButton_Clicked; // Attach event to clear signature
                Grid.SetRow(clearButton, 2);
                Grid.SetColumn(clearButton, 0);

                Button createButton = new Button { Text = "Create", BackgroundColor = Color.FromArgb("#6750A4"), TextColor = Colors.White, CornerRadius = 5, HeightRequest = 40, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                createButton.Clicked += CreateButton_Clicked; // Attach event to create signature
                Grid.SetRow(createButton, 2);
                Grid.SetColumn(createButton, 2);

                // Add all components to the layout grid
                signaturelayout.Children.Add(drawLabel);
                signaturelayout.Children.Add(closeButton);
                signaturelayout.Children.Add(signatureView);
                signaturelayout.Children.Add(clearButton);
                signaturelayout.Children.Add(createButton);
                signature.Children.Add(signaturelayout);
                signaturelayout.IsVisible = false; // Initially hide dialog
            }
        }

        // Event handler for closing the custom signature dialog
        private void CloseButton_Clicked(object? sender, EventArgs e)
        {
            signatureView?.Clear(); // Clear contents of the signature pad
            if (signaturelayout != null)
                signaturelayout.IsVisible = false; // Hide the dialog
            signatureButton.IsVisible = true; // Show signature button
        }

        // Event handler to clear the signature pad
        private void ClearButton_Clicked(object? sender, EventArgs e)
        {
            signatureView?.Clear(); // Remove all drawn content in signature pad          
        }

        // Event handler to save the PDF document with annotations
        private async void saveAsButton_Clicked(object sender, EventArgs e)
        {
            // Save the document to an output stream
            Stream outStream = new MemoryStream();
            await pdfViewer.SaveDocumentAsync(outStream);
            try
            {
                if (viewModel.FileData != null)
                {
                    // Save the document to a file
                    string? filePath = await FileService.SaveAsAsync(viewModel.FileData.FileName, outStream);
                    await DisplayAlert("File saved", $"The file is saved to {filePath}", "OK");
                }
            }
            catch (Exception exception)
            {
                // Notify the user if the save operation fails
                await DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
            }
        }

        // Event handler for deleting a selected annotation from the PDF
        private void deleteButton_Clicked(object sender, EventArgs e)
        {
            if (selectedAnnotation != null)
                pdfViewer.RemoveAnnotation(selectedAnnotation); // Remove selected annotation
            ToggleDeleteOptionVisibility(false); // Hide delete button after removal
        }

        // Toggle the visibility of the delete option in the UI
        void ToggleDeleteOptionVisibility(bool isVisible)
        {
            deleteButton.IsVisible = isVisible; // Update button visibility
        }

        // Manage the state when an annotation is selected, enabling delete operation
        private void pdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
        {
            selectedAnnotation = e.Annotation; // Store selected annotation for further actions
            ToggleDeleteOptionVisibility(true); // Display delete button
        }

        // Manage state when an annotation is deselected, disabling certain options
        private void pdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
        {
            if (e.Annotation == selectedAnnotation)
                selectedAnnotation = null; // Clear current annotation selection
            ToggleDeleteOptionVisibility(false); // Hide delete button
        }

        // Enable the signature mode in the PDF viewer interface
        private void signatureButton_Clicked(object sender, EventArgs e)
        {
            pdfViewer.AnnotationMode = AnnotationMode.Signature; // Set annotation mode to signature
        }

        // Event handler to create an image stream from the drawn signature content
        private async void CreateButton_Clicked(object? sender, EventArgs e)
        {
            if (signatureView is SfSignaturePad signature) // Ensure valid instance of signature pad is used
            {
                var inkPoints = signature?.GetSignaturePoints() ?? new List<List<float>>(); // Gather points constituting the signature
                if (signatureFormField != null)
                {
                    // For form fields, convert ink points to an ink annotation
                    int pageNumber = signatureFormField.PageNumber;
                    InkAnnotation ink = new InkAnnotation(inkPoints, pageNumber);
                    ink.IsSignature = true;
                    ink.Color = Colors.Black;
                    ink.BorderWidth = 2;
                    signatureFormField.Signature = ink; // Add ink annotation to the form field
                }
                else
                {
                    // Convert drawn signature to an image source if no form field
                    ImageSource? imagestream = signature?.ToImageSource();
                    if (imagestream is StreamImageSource streamsource)
                    {
                        var stream = await streamsource.Stream(CancellationToken.None); // Get the stream for the image source
                        stream.Position = 0; // Reset position in the stream
                        Microsoft.Maui.Graphics.IImage platformImage = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
                        imageWidth = platformImage.Width;
                        imageHeight = platformImage.Height;

                        // Scale the image dimensions proportionately if they exceed limits
                        if (imageWidth < imageHeight)
                        {
                            if (imageWidth > 150)
                            {
                                imageHeight = 150 * (imageHeight / imageWidth);
                                imageWidth = 150;
                            }
                        }
                        else
                        {
                            if (imageHeight > 150)
                            {
                                imageWidth = 150 * (imageWidth / imageHeight);
                                imageHeight = 150;
                            }
                        }
                        stream.Position = 0; // Reset stream position
                        signatureStream = stream; // Retain stream for annotation
                    }
                    ShowToast("Tap to add signature"); // Alert user to place signature
                }
                signatureFormField = null; // Clear form field reference after use
                signatureView?.Clear(); // Clear the signature pad
                if (signaturelayout != null)
                    signaturelayout.IsVisible = false; // Hide the dialog after creating signature
                signatureButton.IsVisible = true; // Show signature button for further interaction
            }
        }

        // Method to show temporary toast notifications for user interaction feedback
        async void ShowToast(string text)
        {
            toast.Opacity = 1;
            toastText.Text = text; // Set text for the toast message
            toast.InputTransparent = true; // Disable input interference
            await toast.FadeTo(0, 2000, Easing.SinIn); // Animate fade-out over 2 seconds
        }
    }
}