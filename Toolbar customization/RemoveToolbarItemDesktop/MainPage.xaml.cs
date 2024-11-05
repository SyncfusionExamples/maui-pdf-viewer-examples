using Syncfusion.Maui.PdfViewer;
using System.Reflection;

namespace PdfViewerDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Load the PDF document from the embedded resource.
            PdfViewer.DocumentSource = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("PdfViewerDemo.Assets.pdf_succinctly.pdf");

#if MACCATALYST || WINDOWS
            RemoveOutlineTool();
#endif
        }

        /// <summary>
        /// Remove the outline tool from the primary toolbar of the PDF Viewer on Desktop platform.
        /// </summary>
        void RemoveOutlineTool()
        {
            // Get the primary toolbar of the PDF Viewer that contains primary tools on desktop platforms.
            Syncfusion.Maui.PdfViewer.Toolbar? primaryToolbar = PdfViewer.Toolbars?.GetByName("PrimaryToolbar");
            if (primaryToolbar != null)
            {
                // Get the outline from the toolbar.
                Syncfusion.Maui.PdfViewer.ToolbarItem? outlineTool = primaryToolbar.Items?.GetByName("Outline"); ;

                if (outlineTool != null)
                {
                    // Remove the tool from the toolbar.
                    primaryToolbar?.Items?.Remove(outlineTool);
                }
            }
        }
    }
}