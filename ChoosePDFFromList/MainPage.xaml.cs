namespace ChoosePDFFromList;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

    /// <summary>
    /// ListView ItemTapped event handler to update the PDF document stream
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        if (this.BindingContext is PdfData bindingContext)
        {
            // Update the document stream
            bindingContext.UpdateDocumentStream(e.Item.ToString());
        }
    }
}

