using Syncfusion.Maui.ListView;

namespace CustomToolbar;

/// <summary>
/// PageLayoutModeDialog representing a dialog for selecting page layout mode.
/// </summary>
public partial class PageLayoutModeDialog : ContentView
{
    /// <summary>
    /// Event raised when an item is tapped in the dialog.
    /// </summary>
    public event EventHandler<ItemTappedEventArgs> ItemTapped;

    /// <summary>
    /// Initializes a new instance of the PageLayoutModeDialog class.
    /// </summary>
    public PageLayoutModeDialog() 
    {
        Initialize();
    }
    internal void Initialize()
    {
        InitializeComponent();
        List<AnnotationButtonItem> items = new List<AnnotationButtonItem>()
        {
            new AnnotationButtonItem()
            {
                Icon = "\uE796",
                IconName = "Continuous page"
            },
            new AnnotationButtonItem()
            {
                 Icon = "\uE797",
                IconName = "Page by page"
               
            },
        };
        listView.ItemsSource = items;
        listView.SelectedItem = items[0];
    }

    /// <summary>
    /// Handles the event when an item is tapped in the ListView.
    /// </summary>
    private void SfListView_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
    {
        if(sender is SfListView view)
        {

            if (e.DataItem is AnnotationButtonItem buttonItem && BindingContext is PdfViewerViewModel viewModel)
            {
                if (buttonItem.IconName == "Continuous page")
                    viewModel.PageLayoutMode = Syncfusion.Maui.PdfViewer.PageLayoutMode.Continuous;
                else
                    viewModel.PageLayoutMode = Syncfusion.Maui.PdfViewer.PageLayoutMode.Single;
            }
            ItemTapped?.Invoke(this, new ItemTappedEventArgs(view));
        }
    }
    internal void DisappearHighlight()
    {
        // Clears the selection highlight in the ListView.
        listView.SelectedItem = null;
    }
}

/// <summary>
/// Represents an item in the dialog ListView.
/// </summary>
public class AnnotationButtonItem
{
    public string Icon { get; set; }
    public string IconName { get; set; }
}

/// <summary>
/// Event arguments for the ItemTapped event.
/// </summary>
public class ItemTappedEventArgs : EventArgs
{
    public View TappedItem;

    public ItemTappedEventArgs(IView view)
    {
        TappedItem = view as View;
    }
}
