using Syncfusion.Maui.PdfViewer.Annotations;
using Syncfusion.Maui.PdfViewer;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using System.Globalization;
using System.Reflection;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Controls;
using Syncfusion.Pdf;
using Syncfusion.Maui.Core.Internals;
using System.Diagnostics;

namespace ConversionOfCoordinates
{
    public partial class MainPage : ContentPage
    {
        Point endPoint = new Point(0, 0);
        Point annotationAddedPoint = new Point(0, 0);
        Point Position;
        int PageNumber = 0;
        string filePath;
        Stream stream;
        Stream loadedStream;
        Grid myGrid;
        SemitransparentView semitransparentView;
        PdfPageBase page;
        bool isClientToPageConversionEnabled = false;
        bool isAnnotationAddedInScrollview = false;
        PdfLoadedDocument documentForScrollConversion;
        PdfLoadedDocument documentForClientConversion;

        public MainPage()
        {
            InitializeComponent();
            stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("ConversionOfCoordinates.Annotations.pdf");
            pdfViewer.LoadDocument(stream, flattenOptions: FlattenOptions.Unsupported);

            pdfViewer.Tapped += PdfViewer_Tapped;
            pdfViewer.PropertyChanged += PdfViewer_PropertyChanged;
            semitransparentView = new SemitransparentView();
            semitransparentView.ViewTapped += SemitransparentView_ViewTapped;
        }

        private void SemitransparentView_ViewTapped(object sender, TapEventArgs e)
        {
            annotationAddedPoint = e.TapPoint;
            int tappedPageNumber = pdfViewer.GetPageNumberFromClientPoint(new Point(e.TapPoint.X, e.TapPoint.Y));
            if (!isClientToPageConversionEnabled)
                return;
            pageNumber.Text = tappedPageNumber.ToString();
            Position = e.TapPoint;
            PageNumber = tappedPageNumber;
            if (tappedPageNumber > 0)
            {
                if (semitransparentView.Children.Count > 0)
                    semitransparentView.Children.Remove(myGrid);
                myGrid = new Grid
                {
                    Background = Colors.Aqua,
                    HeightRequest = 100,
                    WidthRequest = 200,
                };
                AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(e.TapPoint.X, e.TapPoint.Y, 200, 100));
                semitransparentView.Children.Add(myGrid);
                Point pagePoint = pdfViewer.ConvertClientPointToPagePoint(new Point(e.TapPoint.X, e.TapPoint.Y), tappedPageNumber);
                endPoint = pdfViewer.ConvertClientPointToPagePoint(new Point(e.TapPoint.X + 200, e.TapPoint.Y + 100), tappedPageNumber);
                documentForScrollConversion = new PdfLoadedDocument(stream);
                page = documentForScrollConversion.Pages[tappedPageNumber - 1] as PdfPageBase;
                if(page.Rotation != PdfPageRotateAngle.RotateAngle0)
                {
                    pagePoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, pagePoint.X, pagePoint.Y);
                    endPoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, endPoint.X, endPoint.Y);
                }
                PdfRectangleAnnotation squareAnnotation = new PdfRectangleAnnotation(new Syncfusion.Drawing.RectangleF((float)pagePoint.X, (float)pagePoint.Y, (float)(endPoint.X - pagePoint.X), (float)(endPoint.Y - pagePoint.Y)), "");
                squareAnnotation.Border.BorderWidth = 3;
                squareAnnotation.Color = new Syncfusion.Pdf.Graphics.PdfColor(255, 255, 0, 0);
                page.Annotations.Add(squareAnnotation);
            }
        }

        private void PdfViewer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "VerticalOffset" || e.PropertyName == "HorizontalOffset" || e.PropertyName == "ZoomFactor") && isAnnotationAddedInScrollview)
            {
                if(semitransparentView != null)
                {
                    semitransparentView.TranslationY = -pdfViewer.VerticalOffset.Value;
                    semitransparentView.TranslationX = -pdfViewer.HorizontalOffset.Value;

                    semitransparentView.Children.Remove(myGrid);

                    PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(loadedStream);
                    for (int i = 0; i < pdfLoadedDocument.PageCount; i++)
                    {
                        PdfPageBase page = pdfLoadedDocument.Pages[i] as PdfPageBase;
                        if (i == pdfViewer.PageNumber - 1)
                        {
                            foreach (var annotation in page.Annotations)
                            {
                                if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                                {
                                    Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                                    endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), i + 1);

                                    clientPoint.Y = (double)(clientPoint.Y - pdfViewer.VerticalOffset);
                                    endPoint.Y = (double)(endPoint.Y - pdfViewer.VerticalOffset);
                                    clientPoint.X = (double)(clientPoint.X - pdfViewer.HorizontalOffset);
                                    endPoint.X = (double)(endPoint.X - pdfViewer.HorizontalOffset);

                                    myGrid = new Grid
                                    {
                                        Background = Colors.Blue,
                                        WidthRequest = endPoint.X - clientPoint.X,
                                        HeightRequest = endPoint.Y - clientPoint.Y
                                    };
                                    AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X + pdfViewer.HorizontalOffset.Value, clientPoint.Y + pdfViewer.VerticalOffset.Value, endPoint.X - clientPoint.X, endPoint.Y - clientPoint.Y));
                                    if (!semitransparentView.Children.Contains(myGrid) && !pdfViewer.Children.Contains(myGrid))
                                        semitransparentView.Children.Add(myGrid);
                                }
                            }

                        }
                    }
                }
            }
        }

        private void PdfViewer_Tapped(object sender, GestureEventArgs e)
        {
            annotationAddedPoint = e.Position;
            int tappedPageNumber = pdfViewer.GetPageNumberFromClientPoint(new Point(e.Position.X, e.Position.Y));
            pageNumber.Text = tappedPageNumber.ToString();
        }

        internal static Point ReverseRotation(double pageWidth, double pageHeight, PdfPageRotateAngle pageRotationAngle, double x, double y)
        {
            PdfPageRotateAngle reverseAngle = pageRotationAngle;
            if (pageRotationAngle == PdfPageRotateAngle.RotateAngle90)
            {
                reverseAngle = PdfPageRotateAngle.RotateAngle270;
                return AdjustForRotation(pageHeight, pageWidth, reverseAngle, x, y);
            }
            else if (pageRotationAngle == PdfPageRotateAngle.RotateAngle270)
            {
                reverseAngle = PdfPageRotateAngle.RotateAngle90;
                return AdjustForRotation(pageHeight, pageWidth, reverseAngle, x, y);
            }
            return AdjustForRotation(pageWidth, pageHeight, reverseAngle, x, y);
        }

        internal static Point AdjustForRotation(double pageWidth, double pageHeight, PdfPageRotateAngle pageRotationAngle, double x, double y)
        {
            if (pageRotationAngle == PdfPageRotateAngle.RotateAngle90)
            {
                double rotatedX = pageHeight - y;
                double rotatedY = x;
                return new Point(rotatedX, rotatedY);
            }
            else if (pageRotationAngle == PdfPageRotateAngle.RotateAngle180)
            {
                double rotatedX = pageWidth - x;
                double rotatedY = pageHeight - y;
                return new Point(rotatedX, rotatedY);
            }
            else if (pageRotationAngle == PdfPageRotateAngle.RotateAngle270)
            {
                double rotatedX = y;
                double rotatedY = pageWidth - x;
                return new Point(rotatedX, rotatedY);
            }
            else
            {
                return new Point(x, y);
            }
        }

        private async void LoadFromDoc_Clicked(object sender, EventArgs e)
        {
            try
                {
                    isAnnotationAddedInScrollview = false;
                    semitransparentView = new SemitransparentView();
                    AbsoluteLayout.SetLayoutBounds(semitransparentView, new Rect(0, 0, (double)pdfViewer.ClientRectangle.Width - 16, (double)pdfViewer.ClientRectangle.Height));
                    pdfViewer.Children.Add(semitransparentView);
            }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "View is already added", "OK");
                }
            pdfViewer.DocumentLoaded += PdfViewer_DocumentLoaded;
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                    { DevicePlatform.iOS, new[] { "public.pdf" } },
                    { DevicePlatform.Android, new[] { "application/pdf" } },
                    { DevicePlatform.WinUI, new[] { "pdf" } },
                    { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                });

            //Provide picker title if required.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            await PickAndShow(options);
        }

        private void PdfViewer_DocumentLoaded(object? sender, EventArgs? e)
        {
            documentForClientConversion = new PdfLoadedDocument(loadedStream);
            for (int i = 0; i < documentForClientConversion.PageCount; i++)
            {
                PdfPageBase page = documentForClientConversion.Pages[i] as PdfPageBase;
                if (i == 0)
                {
                    foreach (var annotation in page.Annotations)
                    {
                        if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                        {
                            isAnnotationAddedInScrollview = false;
                            Point clientPoint = pdfViewer.ConvertPagePointToClientPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                            endPoint = pdfViewer.ConvertPagePointToClientPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), i + 1);
                            if (semitransparentView.Children.Contains(myGrid))
                                semitransparentView.Children.Remove(myGrid);
                            clientPoint = AdjustForRotation(page.Size.Width, page.Size.Height, page.Rotation, clientPoint.X, clientPoint.Y);
                            endPoint = AdjustForRotation(page.Size.Width, page.Size.Height, page.Rotation, endPoint.X, endPoint.Y);
                            myGrid = new Grid
                            {
                                Background = Colors.Blue,
                                HeightRequest = endPoint.Y - clientPoint.Y,
                                WidthRequest = endPoint.X - clientPoint.X,
                            };
                            AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X, clientPoint.Y, endPoint.X - clientPoint.X, endPoint.Y - clientPoint.Y));
                            semitransparentView.Children.Add(myGrid);
                        }
                    }
                }
            }
            pdfViewer.DocumentLoaded -= PdfViewer_DocumentLoaded;
        }

       
        private void removeView_Clicked(object sender, EventArgs e)
        {
            if(semitransparentView.Children.Contains(myGrid))
               semitransparentView.Children.Remove(myGrid);
            if(pdfViewer.Children.Contains(semitransparentView))
               pdfViewer.Children.Remove(semitransparentView);
            if (pdfViewer.Children.Contains(myGrid))
                pdfViewer.Children.Remove(myGrid);
            semitransparentView = null;
            myGrid = null;
        }

        private async void AddScrollView_Clicked(object sender, EventArgs e)
        {
            isClientToPageConversionEnabled = false;
            try
            {
                semitransparentView = new SemitransparentView();
                AbsoluteLayout.SetLayoutBounds(semitransparentView, new Rect(0, 0, (double)pdfViewer.ClientRectangle.Width - 16, (double)pdfViewer.ExtentHeight));
                pdfViewer.Children.Add(semitransparentView);
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", "View is already added", "OK");
            }
            pdfViewer.DocumentLoaded += DocumentLoadedForScrollView;
            FilePickerFileType pdfFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>{
                    { DevicePlatform.iOS, new[] { "public.pdf" } },
                    { DevicePlatform.Android, new[] { "application/pdf" } },
                    { DevicePlatform.WinUI, new[] { "pdf" } },
                    { DevicePlatform.MacCatalyst, new[] { "pdf" } },
                });

            //Provide picker title if required.
            PickOptions options = new()
            {
                PickerTitle = "Please select a PDF file",
                FileTypes = pdfFileType,
            };
            await PickAndShow(options);
        }

        private void DocumentLoadedForScrollView(object? sender, EventArgs? e)
        {
            documentForClientConversion = new PdfLoadedDocument(loadedStream);
            for (int i = 0; i < documentForClientConversion.PageCount; i++)
            {
                PdfPageBase page = documentForClientConversion.Pages[i] as PdfPageBase;
                if (i == 0)
                {
                    foreach (var annotation in page.Annotations)
                    {
                        if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                        {
                            isAnnotationAddedInScrollview = false;
                                Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                                endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X + squareAnnotation.Bounds.Width, squareAnnotation.Bounds.Y + squareAnnotation.Bounds.Height), i + 1);
                                clientPoint.X = (double)(clientPoint.X - pdfViewer.HorizontalOffset);
                                clientPoint.Y = (double)(clientPoint.Y - pdfViewer.VerticalOffset);
                                endPoint.X = (double)(endPoint.X - pdfViewer.HorizontalOffset);
                                endPoint.Y = (double)(endPoint.Y - pdfViewer.VerticalOffset);
                                isAnnotationAddedInScrollview = true;
                            if (semitransparentView.Children.Contains(myGrid))
                                semitransparentView.Children.Remove(myGrid);
                            clientPoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, clientPoint.X, clientPoint.Y);
                            endPoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, endPoint.X, endPoint.Y);
                            myGrid = new Grid
                            {
                                Background = Colors.Blue,
                                HeightRequest = endPoint.Y - clientPoint.Y,
                                WidthRequest = endPoint.X - clientPoint.X,
                            };
                            AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X, clientPoint.Y, endPoint.X - clientPoint.X, endPoint.Y - clientPoint.Y));
                            semitransparentView.Children.Add(myGrid);
                        }
                    }
                }
            }
        }

        private void LoadForScrollView_Clicked(object sender, EventArgs e)
        {
            using (FileStream inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(inputStream);
                for (int i = 0; i < pdfLoadedDocument.PageCount; i++)
                {
                    PdfPageBase page = pdfLoadedDocument.Pages[i] as PdfPageBase;
                    foreach (var annotation in page.Annotations)
                    {
                        if (annotation is PdfLoadedRectangleAnnotation squareAnnotation)
                        {
                            Point clientPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(squareAnnotation.Bounds.X, squareAnnotation.Bounds.Y), i + 1);
                            endPoint = pdfViewer.ConvertPagePointToScrollPoint(new Point(endPoint.X, endPoint.Y), i + 1);
                            clientPoint.Y = (double)(clientPoint.Y - pdfViewer.VerticalOffset);
                            endPoint.Y = (double)(endPoint.Y - pdfViewer.VerticalOffset);
                            clientPoint.X = (double)(clientPoint.X - pdfViewer.HorizontalOffset);
                            endPoint.X = (double)(endPoint.X - pdfViewer.HorizontalOffset);
                            semitransparentView.Children.Remove(myGrid);
                            myGrid = new Grid
                            {
                                Background = Colors.Blue,
                                HeightRequest = 100,
                                WidthRequest = 200,
                            };
                            AbsoluteLayout.SetLayoutBounds(myGrid, new Rect(clientPoint.X, clientPoint.Y, 200, 100));
                            semitransparentView.Children.Add(myGrid);
                        }
                    }
                }
            }
        }

        private async void Open_Clicked(object sender, EventArgs e)
        {
                isClientToPageConversionEnabled = true;
                await DisplayAlert("Information", "Tap to add annotation.Then click Save to save the annotation in the PDF page", "OK");
            try
            {
                isAnnotationAddedInScrollview = false;
                semitransparentView.Children.Remove(myGrid);
                AbsoluteLayout.SetLayoutBounds(semitransparentView, new Rect(0, 0, (double)pdfViewer.ClientRectangle.Width - 16, (double)pdfViewer.ClientRectangle.Height));
                pdfViewer.Children.Add(semitransparentView);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "View is already added", "OK");
            }
        }

        private async Task PickAndShow(PickOptions options)
        {
            try
            {
                //Pick the file from local storage.
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    //Store the resultant PDF as stream.
                    pdfViewer.DocumentSource = await result.OpenReadAsync();
                    stream = (Stream)pdfViewer.DocumentSource;
                    loadedStream = stream;
                }
            }
            catch (Exception ex)
            {
                //Display error when file picker failed to open files.
                string message;
                if (ex != null && string.IsNullOrEmpty(ex.Message) == false)
                    message = ex.Message;
                else
                    message = "File open failed.";
                Microsoft.Maui.Controls.Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
            }
        }

        private async void addForRotation_Clicked(object sender, EventArgs e)
        {
            string path = "";
            //rotatedStream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("CoordinateConversion.90FreeText.pdf");
            Point pagePoint = pdfViewer.ConvertClientPointToPagePoint(new Point(Position.X, Position.Y), PageNumber);
            endPoint = pdfViewer.ConvertClientPointToPagePoint(new Point(Position.X + 200, Position.Y + 100), PageNumber);
            PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream);
            page = pdfLoadedDocument.Pages[PageNumber - 1] as PdfPageBase;
            pagePoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, pagePoint.X, pagePoint.Y);
            endPoint = ReverseRotation(page.Size.Width, page.Size.Height, page.Rotation, endPoint.X, endPoint.Y);
            PdfRectangleAnnotation squareAnnotation = new PdfRectangleAnnotation(new Syncfusion.Drawing.RectangleF((float)pagePoint.X, (float)pagePoint.Y, (float)(endPoint.X - pagePoint.X), (float)(endPoint.Y - pagePoint.Y)), "");
            squareAnnotation.Border.BorderWidth = 3;
            squareAnnotation.Color = new Syncfusion.Pdf.Graphics.PdfColor(255, 255, 0, 0);
            page.Annotations.Add(squareAnnotation);
#if ANDROID || MACCATALYST || IOS
            string fileName = "rotated.pdf";
            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = System.IO.Path.Combine(folderPath, fileName);


            // Ensure the directory exists
            Directory.CreateDirectory(folderPath);
#else
        path = "D:/rotated.pdf";
#endif
            FileStream outputStream; byte[] byteArray;
            using (outputStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                pdfLoadedDocument.Save(outputStream);
                outputStream.Position = 0;
                // Read the content of the stream into a byte array
                byteArray = new byte[outputStream.Length];
                outputStream.Read(byteArray, 0, (int)outputStream.Length);
            }
#if ANDROID || MACCATALYST || IOS
            double zoomfactor = pdfViewer.ZoomFactor;
            double? hOffset = pdfViewer.HorizontalOffset;
            double? vOffset = pdfViewer.VerticalOffset;
            pdfViewer.LoadDocument(byteArray);
            pdfViewer.ZoomFactor = zoomfactor;
            pdfViewer.ScrollToOffset((double)hOffset, (double)vOffset);
#endif
        }

        private void Save_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                string fileName = "SavedPDF.pdf";
#if WINDOWS
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName); // Define the file path for Windows.
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    documentForScrollConversion.Save(fileStream); // Copy the stream to the file stream.
                }
#else
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName); // Define the file path for Android, iOS, and Mac Catalyst.
                FileStream fileStream;
                using ( fileStream = new FileStream(filePath, FileMode.Create))
        {
                    // Copy the stream to the file stream.
                    documentForScrollConversion.Save(fileStream);
        }
            pdfViewer.LoadDocument(fileStream);
#endif
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", "The file is in opened state please close the file", "Ok");
            }
            DisplayAlert("Information", "File saved successfully", "Ok");
        }
    }

}
