using Syncfusion.Maui.PdfViewer;
using Syncfusion.Maui.SignaturePad;

namespace CustomSignatureDialog
{
    public partial class MainPage : ContentPage
    {
        Annotation? selectedAnnotation;
        SfSignaturePad? signatureView;
        Grid? signaturelayout;
        Stream? signatureStream;
        float imageWidth, imageHeight;

        // Main constructor initializing components and event handlers
        public MainPage()
        {
            InitializeComponent();
            CreateCustomSignatureDialog(); // Initialize custom signature dialog
                                           // pdfViewer.PropertyChanged += PdfViewer_PropertyChanged; // Handle property changes
            pdfViewer.SignatureModalViewAppearing += PdfViewer_SignatureModalViewAppearing; // Event to suppress built-in signature dialog
            pdfViewer.SignatureModalViewDisappearing += PdfViewer_SignatureModalViewDisappearing;
        }

        private void PdfViewer_SignatureModalViewDisappearing(object? sender, EventArgs e)
        {
            signatureButton.IsVisible = true;
        }

        // Event handler for property changes in the PDF viewer
        private void PdfViewer_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Toggle visibility of the signature button based on annotation mode
            if (e.PropertyName == nameof(Syncfusion.Maui.PdfViewer.SfPdfViewer.AnnotationMode))
                signatureButton.IsVisible = pdfViewer.AnnotationMode != AnnotationMode.Signature;
        }

        // Event handler for tapping action on the PDF viewer
        private void PdfViewerTapped(object sender, GestureEventArgs e)
        {
            if (signatureStream != null)
            {
                // Create stamp annotation using drawn signature
                StampAnnotation stamp = new StampAnnotation(signatureStream, e.PageNumber, new RectF(e.PagePosition.X - imageWidth / 2, e.PagePosition.Y - imageHeight / 2, imageWidth, imageHeight));
                stamp.IsSignature = true; // Mark as signature
                pdfViewer.AddAnnotation(stamp); // Add annotation to the PDF
                signatureStream = null; // Reset the stream
            }
        }

        // Event handler to manage visibility of the custom signature dialog
        private void PdfViewer_SignatureModalViewAppearing(object? sender, FormFieldModalViewAppearingEventArgs e)
        {
            e.Cancel = true; // Cancel the default dialog
            if (signaturelayout != null)
            {  
                signaturelayout.IsVisible = true;
                signatureButton.IsVisible = false;
            }
        }

        // Method to create a custom signature dialog
        private void CreateCustomSignatureDialog()
        {
            if (signatureView == null)
            {
                // Create the layout grid for signature dialog
                signaturelayout = new Grid
                {
                    HeightRequest = 500,
                    RowDefinitions =
                    {
                        new RowDefinition { Height = new GridLength(50) },
                        new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                        new RowDefinition { Height = new GridLength(50) }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    BackgroundColor = Colors.Lavender,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(5, 0)
                };
                signaturelayout.WidthRequest = DeviceInfo.Idiom == DeviceIdiom.Phone ? (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density) : 500;

                // Create and arrange dialog components
                Label drawLabel = new Label { Text = "Draw", HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
                Grid.SetRow(drawLabel, 0);
                Grid.SetColumn(drawLabel, 1);

                Button closeButton = new Button { Text = "Close", HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromArgb("#6750A4") };
                closeButton.Clicked += CloseButton_Clicked;
                Grid.SetRow(closeButton, 0);
                Grid.SetColumn(closeButton, 2);

                signatureView = new SfSignaturePad { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, Background = Colors.White };
                Grid.SetRow(signatureView, 1);
                Grid.SetColumnSpan(signatureView, 3);

                Button clearButton = new Button { Text = "Clear", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.FromArgb("#6750A4") };
                clearButton.Clicked += ClearButton_Clicked;
                Grid.SetRow(clearButton, 2);
                Grid.SetColumn(clearButton, 0);

                Button createButton = new Button { Text = "Create", BackgroundColor = Color.FromArgb("#6750A4"), TextColor = Colors.White, CornerRadius = 5, HeightRequest = 40, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                createButton.Clicked += CreateButton_Clicked;
                Grid.SetRow(createButton, 2);
                Grid.SetColumn(createButton, 2);

                // Add all components to the layout
                signaturelayout.Children.Add(drawLabel);
                signaturelayout.Children.Add(closeButton);
                signaturelayout.Children.Add(signatureView);
                signaturelayout.Children.Add(clearButton);
                signaturelayout.Children.Add(createButton);
                signature.Children.Add(signaturelayout);
                signaturelayout.IsVisible = false; // Initially hide the dialog
            }
        }

        // Event handler to close the signature dialog
        private void CloseButton_Clicked(object? sender, EventArgs e)
        {
            signatureView?.Clear(); // Clear the signature pad
            if (signaturelayout != null)
                signaturelayout.IsVisible = false; // Hide the dialog
            signatureButton.IsVisible = true;
        }

        // Event handler to clear the signature pad
        private void ClearButton_Clicked(object? sender, EventArgs e)
        {
            signatureView?.Clear(); // Clear all drawn signature in signature pad           
        }

        // Event handler to save the PDF as a document
        private async void saveAsButton_Clicked(object sender, EventArgs e)
        {
            Stream outStream = new MemoryStream();
            await pdfViewer.SaveDocumentAsync(outStream); // Save current document to stream
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
                // Display error if save fails
                await DisplayAlert("Error", $"The file is not saved. {exception.Message}", "OK");
            }
        }

        // Event handler to delete selected annotation
        private void deleteButton_Clicked(object sender, EventArgs e)
        {
            if (selectedAnnotation != null)
                pdfViewer.RemoveAnnotation(selectedAnnotation); // Remove the annotation
            ToggleDeleteOptionVisibility(false); // Hide delete button
        }

        // Method to toggle the visibility of the delete button
        void ToggleDeleteOptionVisibility(bool isVisible)
        {
            deleteButton.IsVisible = isVisible;
        }

        // Event handler for annotation selection
        private void pdfViewer_AnnotationSelected(object sender, AnnotationEventArgs e)
        {
            selectedAnnotation = e.Annotation; // Store the selected annotation
            ToggleDeleteOptionVisibility(true); // Show delete button
        }

        // Event handler for annotation deselection
        private void pdfViewer_AnnotationDeselected(object sender, AnnotationEventArgs e)
        {
            if (e.Annotation == selectedAnnotation)
                selectedAnnotation = null; // Clear the selection
            ToggleDeleteOptionVisibility(false); // Hide delete button
        }

        // Event handler to set annotation mode to signature
        private void signatureButton_Clicked(object sender, EventArgs e)
        {
            pdfViewer.AnnotationMode = AnnotationMode.Signature; // Enable signature mode
        }

        // Event handler to create signature image from drawn content
        private async void CreateButton_Clicked(object? sender, EventArgs e)
        {
            if (signatureView is SfSignaturePad signature)
            {
                // Convert drawn signature to image source
                ImageSource? imagestream = signature.ToImageSource();
                if (imagestream is StreamImageSource streamsource)
                {
                    var stream = await streamsource.Stream(CancellationToken.None);
                    stream.Position = 0;
                    Microsoft.Maui.Graphics.IImage platformImage = Microsoft.Maui.Graphics.Platform.PlatformImage.FromStream(stream);
                    imageWidth = platformImage.Width;
                    imageHeight = platformImage.Height;

                    // Scale the image dimensions if necessary
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
                    stream.Position = 0;
                    signatureStream = stream; // Save stream for creating annotation
                }
            }
            signatureView?.Clear(); // Clear the signature pad
            if (signaturelayout != null)
                signaturelayout.IsVisible = false; // Hide the dialog
            signatureButton.IsVisible = true;
        }
    }
}