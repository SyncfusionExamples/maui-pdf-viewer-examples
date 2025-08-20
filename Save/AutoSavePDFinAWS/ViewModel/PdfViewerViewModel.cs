using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Syncfusion.Maui.PdfViewer;
using System.ComponentModel;

namespace AutoSavePDFinAWS
{
    internal class PdfViewerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Reference to the Syncfusion PDF Viewer control for document operations
        /// </summary>
        private SfPdfViewer? _pdfViewer;

        /// <summary>
        /// Name of the currently loaded PDF file
        /// </summary>
        private string? _currentFilePath = string.Empty;

        /// <summary>
        /// Text displayed to user showing current operation status
        /// </summary>
        private string _notificationText = "No PDF loaded. Please click the OPEN button to select a PDF file.";

        /// <summary>
        /// Flag indicating whether auto-save is enabled
        /// </summary>
        private bool _isAutoSaveEnabled = true;

        /// <summary>
        /// Flag indicating whether a PDF document is currently loaded
        /// </summary>
        private bool _isDocumentLoaded = false;

        // Set your AWS credentials and region
        string accessKey = "YOUR_ACCESS_KEY";

        string secretKey = "YOUR_SECRET_KEY";

        RegionEndpoint region = RegionEndpoint.YOUR_REGION; // Change to your desired region

        // Specify the bucket name and object key
        string bucketName = "YOUR_BUCKET_NAME";

        string objectKey = "YOUR_OBJECT_KEY";

        /// <summary>
        /// Constructor of the view model class
        /// </summary>
        public PdfViewerViewModel()
        {
            OpenPdfCommand = new Command(LoadPDF);
            SavePdfCommand = new Command(SavePdf);
        }

        /// <summary>
        /// An event to detect the change in the value of a property.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether a PDF document is currently loaded.
        /// Automatically updates the notification text when the value changes.
        /// </summary>
        public bool IsDocumentLoaded
        {
            get => _isDocumentLoaded;
            set
            {
                if (_isDocumentLoaded != value)
                {
                    // Update notification text based on document load state
                    if (value == false)
                    {
                        NotificationText = FilePath + " - Closed";
                    }
                    else
                    {
                        NotificationText = FilePath + " - Opened";
                    }
                    _isDocumentLoaded = value;
                    OnPropertyChanged("IsDocumentLoaded");
                }
            }
        }

        /// <summary>
        /// Gets or sets the notification text displayed to the user.
        /// Provides real-time feedback about current operations.
        /// </summary>
        public string NotificationText
        {
            get => _notificationText;
            set
            {
                if (_notificationText != value)
                {
                    _notificationText = value;
                    OnPropertyChanged("NotificationText");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto-save is enabled.
        /// When true, PDF changes are automatically saved on edit.
        /// </summary>
        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set
            {
                if (_isAutoSaveEnabled != value)
                {
                    _isAutoSaveEnabled = value;
                    OnPropertyChanged("IsAutoSaveEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the reference to the Syncfusion PDF Viewer control.
        /// Used for document loading and saving operations.
        /// </summary>
        public SfPdfViewer? PdfViewer
        {
            get => _pdfViewer;
            set => _pdfViewer = value;
        }

        /// <summary>
        /// The file path where the file is saved and opened into the PdfViewer. 
        /// </summary>
        public string? FilePath
        {
            get
            {
                return _currentFilePath;
            }
            set
            {
                _currentFilePath = value;
                OnPropertyChanged("FilePath");
            }
        }

        /// <summary>
        /// Command for opening/selecting a PDF file from device storage
        /// </summary>
        public Command OpenPdfCommand { get; }

        /// <summary>
        /// Command for manually saving the current PDF document
        /// </summary>
        public Command SavePdfCommand { get; }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void LoadPDF()
        {
            // Get saved stream from S3
            MemoryStream pdfStream = OpenFileFromS3();

            if(PdfViewer != null)
                // Assigned the stream to the "PdfDocumentStream" property.
                PdfViewer.DocumentSource = pdfStream;

            NotificationText = "Opening - " + $"{FilePath}" + "...";
        }

        private MemoryStream OpenFileFromS3()
        {
            // Create an Amazon S3 client using provided credentials and region.
            // This client is used to communicate with the S3 service.
            using (var s3Client = new AmazonS3Client(accessKey, secretKey, region))
            {
                // Prepare a request to get the object (file) from the specified bucket and key.
                var request = new GetObjectRequest
                {
                    BucketName = bucketName, // The name of the S3 bucket.
                    Key = objectKey // The path or filename of the object in the bucket.
                };

                // Get the file path.
                FilePath = bucketName + "/" + objectKey;

                // Execute the request to get the object from S3.
                using (var response = s3Client.GetObjectAsync(request).Result)

                // Access the response stream which contains the file's data.
                using (var responseStream = response.ResponseStream)
                {
                    // Create a new memory stream to hold the file's contents.
                    var memoryStream = new MemoryStream();

                    // Copy the data from the response stream into the memory stream.
                    responseStream.CopyTo(memoryStream);

                    memoryStream.Position = 0; // Reset position before returning

                    // Return the populated memory stream.
                    return memoryStream;
                }
            }
        }

        public async Task UploadPdfToS3Async(Stream pdfStream)
        {

            // Create an Amazon S3 client using provided credentials and region.
            // This client is used to communicate with the S3 service.
            var s3Client = new AmazonS3Client(accessKey, secretKey, region);

            // Initialize the TransferUtility, which simplifies file uploads to S3.
            var transferUtility = new TransferUtility(s3Client);

            // Reset Stream Position before upload.
            pdfStream.Position = 0;

            // Create Upload Request.
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = pdfStream, // The PDF file stream to upload.
                BucketName = bucketName, // The target S3 bucket name.
                Key = objectKey, // The key(path / filename) for the uploaded object.
                ContentType = "application/pdf" // Set the MIME type to indicate it's a PDF.
            };

            // Get the file path.
            FilePath = bucketName + "/" + objectKey;

            // Upload the File to AWS S3 using "UploadAsync" method in the "TransferUtility" class.
            await transferUtility.UploadAsync(uploadRequest);

            NotificationText = FilePath + " - Saved successfully";
        }

        public async void SavePdf()
        {
            // Create a new memory stream to hold the saved PDF document
            Stream savedStream = new MemoryStream();

            if (PdfViewer != null)
                // Asynchronously save the current document content into the memory stream
                await PdfViewer.SaveDocumentAsync(savedStream);

            await UploadPdfToS3Async(savedStream);
        }

        public void OnDocumentEdited()
        {
            if (IsAutoSaveEnabled)
            {
                // Automatically save the document when auto-save is enabled
                SavePdf();
            }
            else
            {
                // Just notify user that document has been edited
                NotificationText = FilePath + " - Edited (Auto-save disabled)";
            }
        }
    }
}