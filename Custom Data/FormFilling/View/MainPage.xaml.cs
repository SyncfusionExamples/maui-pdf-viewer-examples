using Syncfusion.Maui.PdfViewer;

namespace FormFilling
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the event when the form field value is changed.
        /// </summary>
        private void PdfViewer_FormFieldValueChanged(object sender, FormFieldValueChangedEventArgs? e)
        {
            if (e != null && e.FormField!=null)
            {
                PreviousCustomData.Text = $"Previous Modified Date : {e.FormField.CustomData}";
                e.FormField.CustomData = DateTime.Now.ToString();
                CurrentCustomData.Text = $"Current Modified Date : {e.FormField.CustomData}";
            }
        }

        /// <summary>
        /// Handles the event when the "Save" button is clicked, saving the PDF document to a file in the common app data directory.
        /// </summary>
        private async void Save_Clicked(object sender, EventArgs e)
        {
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "Saved.pdf");
            using FileStream outputStream = File.Create(targetFile);
            pdfViewer.SaveDocument(outputStream);
            await DisplayAlert("Document Saved", "The document has been saved to the file " + targetFile, "OK");
        }
        /// <summary>
        /// Handles the event when the "Import" button is clicked, importing annotations from an XFDF file.
        /// </summary>
        private void Import_Clicked(object sender, EventArgs e)
        {
            string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "Export.xfdf");
            if (File.Exists(fileName))
            {
                Stream inputStream = File.OpenRead(fileName);
                inputStream.Position = 0;
                pdfViewer.ImportFormData(inputStream, Syncfusion.Pdf.Parsing.DataFormat.XFdf);
                DisplayAlert("Form data imported", "Form data from the " + fileName + " file are imported", "OK");
            }
            else
                DisplayAlert("No files to import", "There are no xfdf files to import. Please export the form data to an xfdf file and then import. ", "OK");
        }

        /// <summary>
        /// Handles the event when the "Export" button is clicked, exporting annotations to an XFDF file.
        /// </summary>
        private async void Export_Clicked(object sender, EventArgs e)
        {
            Stream xfdfStream = new MemoryStream();
            pdfViewer.ExportFormData(xfdfStream, Syncfusion.Pdf.Parsing.DataFormat.XFdf);
            await CopyFileToAppDataDirectory(xfdfStream, "Export.xfdf");
        }
        /// <summary>
        /// Copies the contents of the input stream to a file in the application's data directory.
        /// </summary>
        /// <param name="inputStream">The input stream containing the data to be copied.</param>
        /// <param name="filename">The name of the target file.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task CopyFileToAppDataDirectory(Stream inputStream, string filename)
        {
            // Create an output filename
            string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, filename);
            // Copy the file to the AppDataDirectory
            using FileStream outputStream = File.Create(targetFile);
            await inputStream.CopyToAsync(outputStream);
            await DisplayAlert("Form data exported", "The form data are exported to the file " + targetFile, "OK");
        }

    }

}
