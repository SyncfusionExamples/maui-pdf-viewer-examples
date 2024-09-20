using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using Syncfusion.Drawing;
using System.Text;
using Syncfusion.Maui.TreeView;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Redaction;
using Syncfusion.Maui.PdfViewer;
using System.Collections.ObjectModel;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.Core.Internals;


namespace SmartRedaction
{ 
    public partial class MainPage : ContentPage
    {
        private AIService openAIService;
        private bool tapped;
        Animation animation;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new SmartRedactionViewModel();
            openAIService = new AIService();
            animation = new Animation();
            sensitiveInfoView.NodeChecked += SensitiveInfoView_NodeChecked;
            sensitiveInfoViewMobile.NodeChecked += SensitiveInfoView_NodeChecked;
            CreateReduct.CheckedChanged += CreateReduct_CheckedChanged;
            PdfViewer.AnnotationAdded += PdfViewer_AnnotationAdded;
            AddRedact.PropertyChanged += AddRedact_PropertyChanged;
            PdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;

        }

        private void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
        {
            if (openAIService.deploymentName == "DEPLOYMENT_NAME")
            {
                Application.Current?.MainPage?.DisplayAlert("Alert", "The Azure API key or endpoint is missing or incorrect. Please verify your credentials", "OK");
                MobileScan.IsEnabled = false;
                MobileScan.Opacity = 0.5;
                DesktopScanButton.IsEnabled = false;
                DesktopScanButton.Opacity=0.5;
            }
        }

        private void AddRedact_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (AddRedact.IsEnabled)
                AddRedact.Opacity = 1;
            else
                AddRedact.Opacity = 0.5;
        }

        private void OkClicked(object sender, EventArgs e)
        {
            AddRedact.IsEnabled=true;
            MobileRedactLayout.IsVisible=false;
        }
        private void CreateReduct_CheckedChanged(object? sender, CheckedChangedEventArgs e)
        {
            if (CreateReduct.IsChecked)
            {
                PdfViewer.AnnotationMode = AnnotationMode.Square;
                PdfViewer.AnnotationSettings.Square.BorderWidth = 1;
                PdfViewer.AnnotationSettings.Author = "RedactedRect";
            }
            else
            {
                PdfViewer.AnnotationMode = AnnotationMode.None;
            }
        }

        private void PdfViewer_AnnotationAdded(object? sender, AnnotationEventArgs e)
        {
            if (CreateReduct.IsChecked && e.Annotation is SquareAnnotation)
            {
                e.Annotation.Name = $"RedactedRect{PdfViewer.Annotations.Count}";
                e.Annotation.Author = "RedactedRect";
                AddRedact.IsEnabled = true;
            }
            else if (e.Annotation is SquareAnnotation)
            {
                SelectRedactitem.IsEnabled = true;
            }
        }


        private void SensitiveInfoView_NodeChecked(object? sender, NodeCheckedEventArgs e)
        {
            if (e.Node?.Content is TreeItem treeItem)
            {
                // Access the NodeText
                string nodeId = treeItem.NodeId;
                string nodeText = treeItem.NodeText;

                // Add or remove annotation for "Select All" nodes
                if (nodeId == "Select All")
                {
                    foreach (TreeItem item in ViewModel.ChildNodes)
                    {
                        if (e.Node.IsChecked == true)
                        {
                            // Create a rectangle annotation
                            RectF Bounds = new RectF()
                            {
                                X = item.Bounds.X,
                                Y = item.Bounds.Y,
                                Width = item.Bounds.Width,
                                Height = item.Bounds.Height
                            };
                            SquareAnnotation annotation = new SquareAnnotation(Bounds, item.pageNumber)
                            {
                                Color = Colors.Red,    // Set stroke color
                                BorderWidth = 1,       // Set stroke thickness
                                Name = item.NodeId     // Set annotation ID
                            };

                            // Add the annotation to the PDF viewer
                            PdfViewer.AddAnnotation(annotation);
                        }
                        else
                        {
                            // Find and remove the corresponding annotation
                            ReadOnlyObservableCollection<Annotation> annotations = PdfViewer.Annotations;
                            foreach (Annotation annotation in annotations)
                            {
                                if (annotation.Name == item.NodeId)
                                {
                                    PdfViewer.RemoveAnnotation(annotation);
                                    break;
                                }
                            }
                        }
                    }
                }
                // Add or remove annotation for a specific page
                else if (nodeText.Contains("Page"))
                {
                    foreach (TreeItem item in ViewModel.ChildNodes)
                    {
                        if (item.pageNumber == int.Parse(treeItem.NodeId))
                        {
                            if (e.Node.IsChecked == true)
                            {
                                // Create and add annotation for the specific page
                                RectF Bounds = new RectF()
                                {
                                    X = item.Bounds.X,
                                    Y = item.Bounds.Y,
                                    Width = item.Bounds.Width,
                                    Height = item.Bounds.Height
                                };
                                SquareAnnotation annotation = new SquareAnnotation(Bounds, item.pageNumber)
                                {
                                    Color = Colors.Red,
                                    BorderWidth = 1,
                                    Name = item.NodeId
                                };
                                PdfViewer.AddAnnotation(annotation);
                            }
                            else
                            {
                                // Remove annotation for the specific page
                                ReadOnlyObservableCollection<Annotation> annotations = PdfViewer.Annotations;
                                foreach (Annotation annotation in annotations)
                                {
                                    if (annotation.Name == item.NodeId)
                                    {
                                        PdfViewer.RemoveAnnotation(annotation);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                // Add or remove annotation for other nodes
                else
                {
                    // Handle other specific nodes
                    if (e.Node.IsChecked == true)
                    {
                        // Create and add annotation
                        RectF Bounds = new RectF()
                        {
                            X = treeItem.Bounds.X,
                            Y = treeItem.Bounds.Y,
                            Width = treeItem.Bounds.Width,
                            Height = treeItem.Bounds.Height
                        };
                        SquareAnnotation annotation = new SquareAnnotation(Bounds, treeItem.pageNumber)
                        {
                            Color = Colors.Red,
                            BorderWidth = 1,
                            Name = treeItem.NodeId
                        };
                        PdfViewer.AddAnnotation(annotation);
                    }
                    else
                    {
                        // Remove annotation
                        ReadOnlyObservableCollection<Annotation> annotations = PdfViewer.Annotations;
                        foreach (Annotation annotation in annotations)
                        {
                            if (annotation.Name == treeItem.NodeId)
                            {
                                PdfViewer.RemoveAnnotation(annotation);
                                break;
                            }
                        }
                    }
                }
            }
            if (PdfViewer.Annotations.Count >= 1 &&
    PdfViewer.Annotations.OfType<SquareAnnotation>()
    .Count(a => !string.IsNullOrEmpty(a.Author) || a.Name.Contains("RedactedRect") || a.Author.Contains("RedactedRect")) >= 1)
            {
                SelectRedactitem.IsEnabled = true;
                SelectRedactItem_Mobile.IsEnabled=true;
            }
            else
            {
                SelectRedactitem.IsEnabled= false;
                SelectRedactItem_Mobile.IsEnabled = false;
            }
        }

        private void OnToggleTreeViewClicked(object sender, EventArgs e)
        {
            popupContainer.IsVisible = !popupContainer.IsVisible;
            SenstiveInfoContainer.IsVisible = false;
        }

        private float ConvertPointToPixel(float number)
        {
            return (number * 96f / 72f);
        }
        private string ExtractedTextFromPDF()
        {
            List<string> extractedText = new List<string>();
            var documentSource = PdfViewer.DocumentSource;
            if (documentSource != null)
            {
                Stream stream = (Stream)documentSource;
                PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream);
                // Loading page collections
                PdfLoadedPageCollection loadedPages = loadedDocument.Pages;
                // Extract annotations to a memory stream and convert to string
                using (MemoryStream annotationStream = new MemoryStream())
                {
                    loadedDocument.ExportAnnotations(annotationStream, AnnotationDataFormat.Json);
                    string annotations = ConvertToString(annotationStream);
                    if (!String.IsNullOrEmpty(annotations))
                    {
                        extractedText.Add("Annotations: " + annotations);
                    }
                }
                // Extract form fields to a memory stream and convert to string
                using (MemoryStream formStream = new MemoryStream())
                {
                    if (loadedDocument.Form != null)
                    {
                        loadedDocument.Form.ExportData(formStream, DataFormat.Json, "form");
                        string formFields = ConvertToString(formStream);
                        if (!String.IsNullOrEmpty(formFields))
                        {
                            extractedText.Add("Form fields: " + formFields);
                        }
                    }
                }
                // Extract text from existing PDF document pages
                for (int i = 0; i < loadedPages.Count; i++)
                {
                    string text = $"... Page {i + 1} ...\n";
                    text += loadedPages[i].ExtractText();
                    extractedText.Add(text);
                }
                string result = string.Join(Environment.NewLine, extractedText);
                return result;
            }
            return "";
        }
        private string ConvertToString(MemoryStream memoryStream)
        {
            // Reset the position of the MemoryStream to the beginning
            memoryStream.Position = 0;
            var reader = new StreamReader(memoryStream, System.Text.Encoding.UTF8);
            return reader.ReadToEnd();
        }
        private void UpdateCheckedPatterns()
        {
            var selectedPattern = PatternView.DataSource.Items;
            List<string> selectedItems = new List<string>();

            if (selectedPattern != null && selectedPattern.Any())
            {
                foreach (var item in selectedPattern)
                {
                    if (((TreeItem)item).IsChecked)
                    {
                        selectedItems.Add(((TreeItem)item).NodeText);
                    }
                }
            }

            // Convert the list to an array and update SelectedPatterns
            ViewModel.SelectedPatterns = selectedItems.ToArray();
        }
        private void UpdateCheckedInfos()
        {
            var checkedItems = sensitiveInfoView.CheckedItems;
            List<string> selectedItems = new List<string>();

            if (checkedItems != null && checkedItems.Any())
            {
                foreach (var item in checkedItems)
                {
                    var treeItem = item as TreeItem;
                    if (treeItem != null)
                    {
                        // Access the NodeId of the checked item
                        var nodeId = treeItem.NodeId;
                        selectedItems.Add(nodeId);
                    }
                }
            }

            // Convert the list to an array and update SelectedPatterns
            ViewModel.CheckedInfo = selectedItems.ToArray();
        }
        private async void ScanClick(object sender, EventArgs e)
        {
#if WINDOWS || MACCATALYST
            LoadingIndicator.IsRunning=true;
#else
            LoadingIndicatorMobile.IsRunning = true;
#endif
            UpdateCheckedPatterns();
            if (ViewModel.SelectedPatterns != null)
            {
                List<string> selectedItems = ViewModel.SelectedPatterns.ToList();
                //Extract the text from the PDF
                string extractedText = ExtractedTextFromPDF();

                //Find the text bounds with the selected patterns
                ViewModel.textboundsDetails = await FindText(extractedText, selectedItems);

                //Count the no. of bounds fetched
                ViewModel.textBoundsCount = ViewModel.textboundsDetails.Sum(pair => pair.Value.Count);
                if (ViewModel.textBoundsCount > 0)
                {
                    ViewModel.dataFetched = true;

                    ViewModel.OnPageAppearing();
#if WINDOWS || MACCATALYST
                    popupContainer.IsVisible = false;
                    SenstiveInfoContainer.IsVisible = true;
#else
                    popupContainerMobile.IsVisible = false;
                    SenstiveInfoContainerMobile.IsVisible = true;
#endif
                    sensitiveInfoView.ItemsSource = null;
                    sensitiveInfoView.ItemsSource = ViewModel.SensitiveInfo;
                    sensitiveInfoViewMobile.ItemsSource = null;
                    sensitiveInfoViewMobile.ItemsSource = ViewModel.SensitiveInfo;
                    //ViewModel.OnAfterRender(ViewModel.dataFetched);
                }
#if WINDOWS || MACCATALYST
                LoadingIndicator.IsRunning=false;
#else
                LoadingIndicatorMobile.IsRunning = false;
#endif
            }
        }
        private async Task<Dictionary<int, List<TextBounds>>> FindText(string extractedText, List<string> selectedItems)
        {

            if (PdfViewer.DocumentSource != null)
            {
                List<string> sensitiveData = await GetSensitiveDataFromPDF(extractedText, selectedItems);
                Stream documentStream = (Stream)PdfViewer.DocumentSource;

                //Remove the Prefixs
                List<string> sensitiveInformations = RemovePrefix(sensitiveData, selectedItems);
                Dictionary<int, List<TextBounds>> boundsData = FindTextBounds(documentStream, sensitiveInformations);
                return boundsData;
            }
            Dictionary<int, List<TextBounds>> temp = new Dictionary<int, List<TextBounds>>();
            return temp;

        }
        internal async Task<List<string>> GetSensitiveDataFromPDF(string text, List<string> selectedItems)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("I have a block of text containing various pieces of information. Please help me identify and extract any Personally Identifiable Information (PII) present in the text. The PII categories I am interested in are:");

            foreach (var item in selectedItems)
            {
                stringBuilder.AppendLine(item);
            }

            stringBuilder.AppendLine("Please provide the extracted information as a plain list, separated by commas, without any prefix or numbering or extra content.");

            string prompt = stringBuilder.ToString();

            var answer = await openAIService.GetAnswerFromGPT(prompt, ExtractedTextFromPDF());

            if (answer != null)
            {
                var output = answer.Trim();

                // Use a HashSet to remove duplicates
                var namesSet = new HashSet<string>(output
                    ?.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(name => name.Trim())
                    .Where(name => !string.IsNullOrEmpty(name)) ?? Enumerable.Empty<string>());

                return namesSet.ToList();
            }
            return new List<string>();
        }

        public Dictionary<int, List<TextBounds>> FindTextBounds(Stream stream, List<string> sensitiveInformations)
        {
            Dictionary<int, List<TextBounds>> accumulatedBounds = new Dictionary<int, List<TextBounds>>();

            using (PdfLoadedDocument loadedDocument = new PdfLoadedDocument(stream))
            {
                foreach (var info in sensitiveInformations)
                {
                    if (!string.IsNullOrEmpty(info))
                    {
                        Dictionary<int, List<RectangleF>> bounds;

                        // Find the text bounds
                        loadedDocument.FindText(info, out bounds);

                        // Merge bounds into accumulatedBounds
                        foreach (var pair in bounds)
                        {
                            if (!accumulatedBounds.ContainsKey(pair.Key))
                            {
                                accumulatedBounds[pair.Key] = new List<TextBounds>();
                            }

                            // Add the bounds with the corresponding sensitive information
                            accumulatedBounds[pair.Key].AddRange(pair.Value.Select(rect => new TextBounds
                            {
                                SensitiveInformation = info,
                                Bounds = rect
                            }));
                        }
                    }
                }
            }
            return accumulatedBounds;
        }

        private List<string> RemovePrefix(List<string> sensitiveInfo, List<string> selectedItems)
        {
            for (int i = 0; i < sensitiveInfo.Count; i++)
            {
                foreach (var item in selectedItems)
                {
                    // Remove the selected items title prefix from the extracted sensitive information
                    string prefix = item + ": ";
                    if (sensitiveInfo[i].ToLower().Contains(prefix, StringComparison.Ordinal))
                    {
                        sensitiveInfo[i] = sensitiveInfo[i].Substring((sensitiveInfo[i].IndexOf(':') + 1));
                    }
                }
            }
            return sensitiveInfo;
        }

        private void Redact()
        {
            CreateReduct.IsChecked = false;
            MemoryStream pdf = new MemoryStream();
            PdfViewer.SaveDocument(pdf);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdf);

            foreach (PdfLoadedPage page in loadedDocument.Pages)
            {
                List<PdfLoadedAnnotation> removeAnnotations = new List<PdfLoadedAnnotation>();
                foreach (PdfLoadedAnnotation annotation in page.Annotations)
                {
                    if (annotation is PdfLoadedRectangleAnnotation)
                    {
                        //Check the annot for Redaction
                        if (annotation.Name.Contains("RedactedRect") || annotation.Author.Contains("RedactedRect"))
                        {
                            removeAnnotations.Add(annotation);
                            PdfRedaction redaction = new PdfRedaction(annotation.Bounds, Syncfusion.Drawing.Color.Black);
                            page.AddRedaction(redaction);
                            annotation.Flatten = true;
                        }

                    }
                }

                //Remove from the Annotation list
                foreach (PdfLoadedAnnotation annotation in removeAnnotations)
                {
                    page.Annotations.Remove(annotation);
                }

            }

            loadedDocument.Redact();

            //Reload the document to view the redaction
            MemoryStream stream = new MemoryStream();
            loadedDocument.Save(stream);
            PdfViewer.LoadDocument(stream);
            loadedDocument.Close(true);
            AddRedact.IsEnabled = false;
        }

        private void RedactClicked(object sender, EventArgs e)
        {

            Redact();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
#if WINDOWS || MACCATALYST
popupContainer.IsVisible = !popupContainer.IsVisible;
            SenstiveInfoContainer.IsVisible = false;
            SelectRedactitem.IsEnabled = false;
#else
            popupContainerMobile.IsVisible = !popupContainerMobile.IsVisible;
            SenstiveInfoContainerMobile.IsVisible = false;
#endif
            ViewModel.SensitiveInfo.Clear();
        }
        private void SavePDF(object sender, EventArgs e)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, "SavedSample.pdf");

            var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            PdfViewer.SaveDocument(stream);
            Application.Current?.MainPage?.DisplayAlert("Success", $"Document saved successfully at:\n{filePath}", "OK");

        }

        private void OpenCloseMobileRedactLayout(object sender, EventArgs e)
        {
            if (MobileRedactLayout.IsVisible)
            {
                MobileRedactLayout.IsVisible = false;
                StartBubbleAnimation();
            }
            else
            {
                MobileRedactLayout.IsVisible = true;
                StopBubbleAnimation();
            }
                
        }

        private void image_Loaded(object sender, EventArgs e)
        {
            StartBubbleAnimation();
        }
        private void StartBubbleAnimation()
        {
            if (!tapped)
            {
                var bubbleEffect = new Animation(v => AIButton.Scale = v, 1, 1.15, Easing.CubicInOut);
                var fadeEffect = new Animation(v => AIButton.Opacity = v, 1, 0.5, Easing.CubicInOut);

                animation.Add(0, 0.5, bubbleEffect);
                animation.Add(0, 0.5, fadeEffect);
                animation.Add(0.5, 1, new Animation(v => AIButton.Scale = v, 1.15, 1, Easing.CubicInOut));
                animation.Add(0.5, 1, new Animation(v => AIButton.Opacity = v, 0.5, 1, Easing.CubicInOut));

                animation.Commit(this, "BubbleEffect", length: 1500, easing: Easing.CubicInOut, repeat: () => true);

            }
        }

        private void StopBubbleAnimation()
        {
            this.AbortAnimation("BubbleEffect");
            tapped = false;
        }
    }




    public class TextBounds
    {
        public string SensitiveInformation { get; set; } = string.Empty;
        public RectangleF Bounds { get; set; }
    }



}
