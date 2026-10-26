using Syncfusion.Maui.PdfViewer;

namespace ViewOnlyPdf
{
    public partial class MainPage : ContentPage
    {
        const string DeniedMessage = "Not allowed for this document.";
        readonly string[] annotationItemNames =
        {
            "TextMarkups", "FreeText", "Ink", "InkEraser", "Shapes", "Stamps", "Redaction", "Signature", "StickyNote",
            "Annotations", "DigitalSignature"
        };

        string currentFileName = "PDF_Succinctly.pdf";
        bool toolbarConfigured;

        public DocumentAccessPolicy Policy { get; }

        public MainPage(DocumentAccessPolicy policy)
        {
            Policy = policy;
            InitializeComponent();
            PdfViewer.Tapped += PdfViewer_Tapped;
            PdfViewer.AnnotationsLoaded += (_, _) => ApplyDocumentInteraction();
            BindingContext = Policy;
            Policy.PropertyChanged += (_, _) =>
            {
                ApplyPolicy();
                ApplyDocumentInteraction();
            };
            ToolbarItems.Add(new Microsoft.Maui.Controls.ToolbarItem
            {
                Text = "Access",
                Order = ToolbarItemOrder.Primary,
                Priority = 0,
                IconImageSource = new FontImageSource
                {
                    FontFamily = "MauiMaterialAssets",
                    Size = 22,
                    Color = Colors.White
                },
                Command = new Command(OpenAccessSettings)
            });
            LoadPdf("PDF_Succinctly");
        }

        private void PdfViewer_Tapped(object? sender, GestureEventArgs? e)
        {
            MoreOptionsMenu.IsVisible = false;  
        }

        async void OpenAccessSettings()
        {
            await Navigation.PushAsync(new AccessSettingsPage(Policy));
        }

        void LoadPdf(string fileName)
        {
            var stream = GetType().Assembly.GetManifestResourceStream($"ViewOnlyPdf.Assets.{fileName}.pdf");
            if (stream == null)
            {
                return;
            }

            PdfViewer.LoadDocument(stream);
        }

        void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
        {
            if (!toolbarConfigured)
            {
                AddFileOperationsToolbarItems();
                toolbarConfigured = true;
            }

            ApplyPolicy();
            ApplyDocumentInteraction();
        }

        /// <summary>
        /// View-only documents keep existing annotations visible but locked, and form fields cannot be edited.
        /// Other modes unlock annotations and allow form filling.
        /// </summary>
        void ApplyDocumentInteraction()
        {
            bool lockAnnotations = !Policy.AllowAnnotations;
            bool lockForms = !Policy.AllowFormFields;
            PdfViewer.AnnotationSettings.IsLocked = lockAnnotations;
            PdfViewer.AllowEditFormFields = !lockForms;

            if (lockAnnotations && PdfViewer.AnnotationMode != AnnotationMode.None)
            {
                PdfViewer.AnnotationMode = AnnotationMode.None;
            }

            foreach (Annotation annotation in PdfViewer.Annotations)
            {
                annotation.IsLocked = lockAnnotations;
            }

            foreach (FormField field in PdfViewer.FormFields)
            {
                field.ReadOnly = lockForms;
            }
        }

        void AddFileOperationsToolbarItems()
        {
            Button fileOpenButton = CreateToolbarButton("\ue712");
            fileOpenButton.Clicked += FileOpenButton_Clicked;

            Button fileSaveButton = CreateToolbarButton("\ue75f");
            fileSaveButton.Clicked += FileSaveButton_Clicked;

            Button moreOptionsButton = CreateToolbarButton("\uE759");
            moreOptionsButton.Clicked += MoreOptionsButton_Clicked;
            moreOptionsButton.Margin = new Thickness(4, 0, 8, 0);

            string toolbarName = DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Platform == DevicePlatform.MacCatalyst
                ? "PrimaryToolbar"
                : "TopToolbar";
            var toolbar = PdfViewer.Toolbars?.GetByName(toolbarName);
            toolbar?.Items?.Insert(0, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileOpenButton, "FileOpenButton"));
            toolbar?.Items?.Insert(1, new Syncfusion.Maui.PdfViewer.ToolbarItem(fileSaveButton, "FileSaveButton"));
            int moreIndex = IndexOfBuiltInMore(toolbar);
            var customMore = new Syncfusion.Maui.PdfViewer.ToolbarItem(moreOptionsButton, "CustomMoreOptions");
            if (moreIndex >= 0)
            {
                toolbar?.Items?.Insert(moreIndex, customMore);
            }
            else
            {
                toolbar?.Items?.Add(customMore);
            }
        }

        static int IndexOfBuiltInMore(Syncfusion.Maui.PdfViewer.Toolbar? toolbar)
        {
            var items = toolbar?.Items;
            if (items == null)
            {
                return -1;
            }

            for (int i = 0; i < items.Count; i++)
            {
                string? name = items[i].Name;
                if (name is "MoreItem" or "PageLayoutMode")
                {
                    return i;
                }
            }

            return -1;
        }

        static Button CreateToolbarButton(string glyph)
        {
            return new Button
            {
                Text = glyph,
                FontSize = 20,
                FontFamily = "MauiMaterialAssets",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Colors.Transparent,
                BorderColor = Colors.Transparent,
                Padding = 10,
                CornerRadius = 5,
                Margin = new Thickness(5, 0, 0, 0),
                TextColor = Colors.Black
            };
        }

        void ApplyPolicy()
        {
            PdfViewer.EnableTextSelection = Policy.AllowTextSelection;
            if (!Policy.AllowAnnotations && PdfViewer.AnnotationMode != AnnotationMode.None)
            {
                PdfViewer.AnnotationMode = AnnotationMode.None;
            }

            SetItemVisible("TopToolbar", "FileOpenButton", Policy.AllowSave);
            SetItemVisible("PrimaryToolbar", "FileOpenButton", Policy.AllowSave);
            SetItemVisible("TopToolbar", "FileSaveButton", Policy.AllowSave);
            SetItemVisible("PrimaryToolbar", "FileSaveButton", Policy.AllowSave);
            SetItemVisible("MoreOptionToolbar", "Print", Policy.AllowPrint);
            SetItemVisible("PrimaryToolbar", "Print", Policy.AllowPrint);
            // Built-in more menu always includes print. Remove that button when print is off.
            bool showBuiltInMore = Policy.AllowPrint;
            SetItemVisible("TopToolbar", "MoreItem", showBuiltInMore);
            SetItemVisible("PrimaryToolbar", "PageLayoutMode", showBuiltInMore);
            SetVisualVisible(PdfViewer, "TopToolbarMoreItem", showBuiltInMore);
            SetVisualVisible(PdfViewer, "PrimaryToolbarPageLayoutMode", showBuiltInMore);
            SetItemVisible("TopToolbar", "CustomMoreOptions", !showBuiltInMore);
            SetItemVisible("PrimaryToolbar", "CustomMoreOptions", !showBuiltInMore);
            if (showBuiltInMore)
            {
                MoreOptionsMenu.IsVisible = false;
            }

            var bottomToolbar = PdfViewer.Toolbars?.GetByName("BottomToolbar");
            if (bottomToolbar != null)
            {
                bottomToolbar.IsVisible = Policy.AllowAnnotations;
            }

            foreach (string name in annotationItemNames)
            {
                SetItemVisible("BottomToolbar", name, Policy.AllowAnnotations);
                SetItemVisible("PrimaryToolbar", name, Policy.AllowAnnotations);
            }

            if (Policy.IsViewOnly)
            {
                bool showUndoRedo = !Policy.IsViewOnly;
                SetItemVisible("TopToolbar", "Undo", showUndoRedo);
                SetItemVisible("TopToolbar", "Redo", showUndoRedo);
                SetItemVisible("PrimaryToolbar", "Undo", showUndoRedo);
                SetItemVisible("PrimaryToolbar", "Redo", showUndoRedo);
            }
        }

        static void SetVisualVisible(IVisualTreeElement root, string automationId, bool isVisible)
        {
            foreach (IVisualTreeElement child in root.GetVisualChildren())
            {
                if (child is VisualElement visual && visual.AutomationId == automationId)
                {
                    visual.IsVisible = isVisible;
                }

                SetVisualVisible(child, automationId, isVisible);
            }
        }

        void SetItemVisible(string toolbarName, string itemName, bool isVisible)
        {
            var item = PdfViewer.Toolbars?.GetByName(toolbarName)?.Items?.GetByName(itemName);
            if (item != null)
            {
                item.IsVisible = isVisible;
            }
        }

        void MoreOptionsButton_Clicked(object? sender, EventArgs e)
        {
            if (Policy.AllowPrint)
            {
                MoreOptionsMenu.IsVisible = false;
                return;
            }

            MoreOptionsMenu.IsVisible = !MoreOptionsMenu.IsVisible;
        }

        void OutlineMenu_Tapped(object? sender, TappedEventArgs e)
        {
            MoreOptionsMenu.IsVisible = false;
            PdfViewer.IsThumbnailViewVisible = false;
            PdfViewer.IsOutlineViewVisible = !PdfViewer.IsOutlineViewVisible;
        }

        void ThumbnailMenu_Tapped(object? sender, TappedEventArgs e)
        {
            MoreOptionsMenu.IsVisible = false;
            PdfViewer.IsOutlineViewVisible = false;
            PdfViewer.IsThumbnailViewVisible = !PdfViewer.IsThumbnailViewVisible;
        }

        async void FileOpenButton_Clicked(object? sender, EventArgs e)
        {
            if (!Policy.AllowSave)
            {
                await DisplayAlertAsync("View only", DeniedMessage, "OK");
                return;
            }

            PdfFileData? fileData = await FileService.OpenFile("pdf");
            if (fileData?.Stream == null || string.IsNullOrEmpty(fileData.FileName))
            {
                return;
            }

            currentFileName = fileData.FileName;
            PdfViewer.LoadDocument(fileData.Stream);
        }

        async void FileSaveButton_Clicked(object? sender, EventArgs e)
        {
            if (!Policy.AllowSave)
            {
                await DisplayAlertAsync("View only", DeniedMessage, "OK");
                return;
            }

            try
            {
                Stream savedStream = new MemoryStream();
                await PdfViewer.SaveDocumentAsync(savedStream);
                string? filePath = await FileService.SaveAsAsync(currentFileName, savedStream);
                if (!string.IsNullOrEmpty(filePath))
                {
                    await DisplayAlertAsync("Success", "File saved successfully", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"Failed to save file: {ex.Message}", "OK");
            }
        }
    }
}
