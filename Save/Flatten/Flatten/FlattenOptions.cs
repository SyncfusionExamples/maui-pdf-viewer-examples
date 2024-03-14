using Syncfusion.Maui.ListView;
using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace Flatten;

public partial class FlattenOptions : ContentView
{
    // Property to access the SfPdfViewer control.
    internal SfPdfViewer PdfViewer { get; set; }

    // Constructor for FlattenOptions class, initializes the instance.
    public FlattenOptions()
    {
        Initialize();
    }

    // Method to initialize the FlattenOptions instance.
    internal void Initialize()
    {
        InitializeComponent();

        // Define a list of AnnotationButtonItem objects.
        List<AnnotationButtonItem> items = new List<AnnotationButtonItem>()
        {
           new AnnotationButtonItem() // Initialize an AnnotationButtonItem object for "No flatten".
           {
            IconName = "No flatten"
           },
           new AnnotationButtonItem() // Initialize an AnnotationButtonItem object for "Flatten annotations".
           {
            IconName = "Flatten annotations"
           },
           new AnnotationButtonItem() // Initialize an AnnotationButtonItem object for "Flatten form fields".
           {
            IconName = "Flatten form fields"
           },
        };
        // Set the items source for the ListView.
        listView.ItemsSource = items;
        // Select the first item in the ListView.
        listView.SelectedItem = items[0]; 
    }

    // Event handler for item tapped event in the ListView.
    private void SfListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if (sender is SfListView view) // Check if the sender is a SfListView.
        {
            // Check if the tapped item is an AnnotationButtonItem.
            if (e.DataItem is AnnotationButtonItem buttonItem) 
            {
                // Check if the selected option is "Flatten annotations".
                if (buttonItem.IconName == "Flatten annotations")
                {
                    // Flatten annotations and disable flattening for form fields.
                    foreach (var annotation in PdfViewer.Annotations)
                    {
                        annotation.FlattenOnSave = true;
                    }
                    foreach (var formField in PdfViewer.FormFields)
                    {
                        formField.FlattenOnSave = false;
                    }
                }
                // Check if the selected option is "Flatten form fields".
                else if (buttonItem.IconName == "Flatten form fields") 
                {
                    // Flatten form fields and disable flattening for annotations.
                    foreach (var formField in PdfViewer.FormFields)
                    {
                        formField.FlattenOnSave = true;
                    }
                    foreach (var annotation in PdfViewer.Annotations)
                    {
                        annotation.FlattenOnSave = false;
                    }
                }
                // If neither "Flatten annotations" nor "Flatten form fields" is selected.
                else
                {
                    // Disable flattening for both annotations and form fields.
                    foreach (var formField in PdfViewer.FormFields)
                    {
                        formField.FlattenOnSave = false;
                    }
                    foreach (var annotation in PdfViewer.Annotations)
                    {
                        annotation.FlattenOnSave = false;
                    }
                }
                SaveDocument(); 
            }
        }
    }

    // Method to save the modified PDF document.
    void SaveDocument()
    {
        // Create a memory stream.
        Stream stream = new MemoryStream();
        // Save the document to the stream.
        PdfViewer.SaveDocument(stream);
        // Define the file name.
        string fileName = "SavedPDF.pdf"; 
#if WINDOWS
   string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName); // Define the file path for Windows.
   using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
   {
       stream.CopyTo(fileStream); // Copy the stream to the file stream.
   }
#else
        string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName); // Define the file path for Android, iOS, and Mac Catalyst.
        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            // Copy the stream to the file stream.
            stream.CopyTo(fileStream);
        }
#endif
        Application.Current!.MainPage!.DisplayAlert("Information", "Successfully saved", "OK"); // Display a success message.
    }

    // Method to clear the selection in the ListView.
    internal void DisappearHighlight()
    {
        // Set the selected item to null to clear the selection.
        listView.SelectedItem = null; 
    }
}

// Represents an item used in the annotation button list.
public class AnnotationButtonItem
{
    // Gets or sets the name of the icon associated with this item.
    public string IconName { get; set; }
}

