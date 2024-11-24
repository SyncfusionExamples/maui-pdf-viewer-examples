using Syncfusion.Drawing;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SmartRedaction
{
    public class SmartRedactionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TreeItem> Patterns { get; set; } = new ObservableCollection<TreeItem>();
        private ObservableCollection<TreeItem> _sensitiveInfo;
        public ObservableCollection<TreeItem> SensitiveInfo
        {
            get { return _sensitiveInfo; }
            set
            {
                if (_sensitiveInfo != value)
                {
                    _sensitiveInfo = value;
                    OnPropertyChanged(nameof(SensitiveInfo));
                }
            }
        }


        internal string[] SelectedPatterns = new string[] { };
        internal string[] CheckedInfo = new string[] { };

        public ObservableCollection<TreeItem> ChildNodes = new ObservableCollection<TreeItem>();
        internal bool dataFetched { get; set; } = false;
        internal bool dataRendered { get; set; } = false;
        internal Dictionary<int, List<TextBounds>> textboundsDetails;
        internal static int annotCount = 0;
        internal int textBoundsCount = 0;


        private Stream? _pdfFile;
        public Stream? PdfFile
        {
            get { return _pdfFile; }
            set
            {
                _pdfFile = value;
                OnPropertyChanged(nameof(PdfFile));
            }
        }

        public SmartRedactionViewModel()
        {
            SensitiveInfo = new ObservableCollection<TreeItem>();
            LoadPatterns();
            LoadPdfFile();
        }

        private ICommand m_openDocumentCommand;
        public ICommand OpenDocumentCommand
        {
            get
            {
                if (m_openDocumentCommand == null)
                    m_openDocumentCommand = new Command<object>(OpenDocument);
                return m_openDocumentCommand;
            }
        }
        //create the method OpenFile
        async void OpenDocument(object commandparamenter)
        {
            //Create file picker with file type as PDF.
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

        /// <summary>
        /// Picks the file from local storage and store as stream.
        /// </summary>
        public async Task PickAndShow(PickOptions options)
        {
            try
            {
                //Pick the file from local storage.
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    //Store the resultant PDF as stream.
                    PdfFile = await result.OpenReadAsync();
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
                Application.Current?.MainPage?.DisplayAlert("Error", message, "OK");
            }

        }

        private void LoadPatterns()
        {
            Patterns = new ObservableCollection<TreeItem>
                {
                    new TreeItem { NodeId = "personNames", NodeText = "Person Names", IsChecked=true },
                    new TreeItem { NodeId = "organizationNames", NodeText = "Organization Names" , IsChecked=true},
                    new TreeItem { NodeId = "emailAddresses", NodeText = "Email addresses", IsChecked=true },
                    new TreeItem { NodeId = "phoneNumbers", NodeText = "Phone Numbers", IsChecked=true },
                    new TreeItem { NodeId = "addresses", NodeText = "Addresses", IsChecked=true },
                    new TreeItem { NodeId = "dates", NodeText = "Dates", IsChecked=true },
                    new TreeItem { NodeId = "accountNumbers", NodeText = "Account Numbers" , IsChecked = true},
                    new TreeItem { NodeId = "creditCardNumbers", NodeText = "Credit Card Numbers" , IsChecked = true}
                };
        }
        public void OnPageAppearing()
        {
            if (dataFetched)
            {
                // Populate the ChildNodes and SensitiveInfo collections
                foreach (var detail in textboundsDetails)
                {
                    foreach (var textBounds in detail.Value)
                    {
                        ChildNodes.Add(new TreeItem
                        {
                            NodeId = "RedactedRect" + annotCount++,
                            NodeText = textBounds.SensitiveInformation,
                            PageNumber = detail.Key + 1,
                            Bounds = textBounds.Bounds,
                        });
                    }
                }

                // Group ChildNodes by pageNumber and convert Child list to ObservableCollection
                var groupedByPage = ChildNodes.GroupBy(node => node.PageNumber)
                                              .Select(group => new TreeItem
                                              {
                                                  NodeId = group.Key.ToString(),
                                                  NodeText = "Page " + (group.Key),
                                                  PageNumber = group.Key,
                                                  Expanded = true,
                                                  Child = new ObservableCollection<TreeItem>(group.ToList()) // Convert to ObservableCollection
                                              })
                                              .ToList();

                SensitiveInfo.Add(new TreeItem
                {
                    NodeId = "Select All",
                    NodeText = $"Select All",
                    Expanded = true,
                    Child = new ObservableCollection<TreeItem>(groupedByPage) // Convert to ObservableCollection
                });

                OnPropertyChanged(nameof(SensitiveInfo)); // Notify UI of changes
            }
        }


        private void LoadPdfFile()
        {
            _pdfFile = this.GetType().Assembly.GetManifestResourceStream("SmartRedaction.Assets.redaction.pdf");
            OnPropertyChanged(nameof(PdfFile));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TreeItem : INotifyPropertyChanged
    {
        public string NodeId { get; set; }
        public string NodeText { get; set; }
        public bool Expanded { get; set; }
        public int PageNumber { get; set; }
        public RectangleF Bounds { get; set; }
        public ObservableCollection<TreeItem> Child { get; set; } = new ObservableCollection<TreeItem>();

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
