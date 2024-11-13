using Syncfusion.Maui.AIAssistView;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;
namespace Summarizer
{
    public class PdfViewerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<IAssistItem> messages;

        private bool showLoading = false;
        public ICommand CopyCommand { get; set; }
        public ICommand RetryCommand { get; set; }
        public ICommand AssistViewRequestCommand { get; set; }

        private Stream? _pdfFile;
        public Stream? PdfFile
        {
            get => _pdfFile;
            set
            {
                if (_pdfFile != value)
                {
                    _pdfFile = value;
                    OnPropertyChanged(nameof(PdfFile));
                }
            }
        }

        public ObservableCollection<IAssistItem> Messages
        {
            get
            {
                return this.messages;
            }

            set
            {
                this.messages = value;
                RaisePropertyChanged("Messages");
            }
        }

        public bool ShowLoading
        {
            get { return this.showLoading; }
            set { this.showLoading = value; RaisePropertyChanged("ShowLoading"); }
        }

        internal AssistServices assistService = new AssistServices();
        
        public PdfViewerViewModel()
        {
             string basePath = "Summarizer.Assets";
            _pdfFile = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream($"{basePath}.summarizeDoc.pdf");
            this.messages = new ObservableCollection<IAssistItem>();
            this.CopyCommand = new Command<object>(ExecuteCopyCommand);
            this.RetryCommand = new Command<object>(ExecuteRetryCommand);
            this.AssistViewRequestCommand = new Command<object>(ExecuteRequestCommand);
            messages.Add(new AssistItem { Text = "Summarize this document", IsRequested = true });
        }

        private async void ExecuteRequestCommand(object obj)
        {
            this.ShowLoading = true;
            var requests = ((RequestEventArgs)obj);
            if (requests.RequestItem != null)
            {
                var request = requests.RequestItem.Text;
                await this.GetResult(request);
            }
            this.ShowLoading = false;
        }

        public async Task GetResult(string query)
        {
            await Task.Delay(1);
            var reply = await assistService.GetSolutionToPrompt(query);
            var suggestion = await assistService.GetSuggestion(query);
            AssistItem botMessage = new AssistItem() { Text = reply, Suggestion = suggestion };
            this.Messages.Add(botMessage);
        }

        private void ExecuteCopyCommand(object obj)
        {
            string text = ((AssistItem)obj).Text;
            text = Regex.Replace(text, "<.*?>|&nbsp;", string.Empty);
            Clipboard.SetTextAsync(text);
        }

        private async void ExecuteRetryCommand(object obj)
        {
            this.ShowLoading = true;
            var response = ((AssistItem)obj).Text;
            var query = await assistService.GetPrompt(response);
            await this.GetResult(query).ConfigureAwait(true);
            this.ShowLoading = false;
        }

        #region PropertyChanged

        /// <summary>
        /// Property changed handler.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Occurs when property is changed.
        /// </summary>
        /// <param name="propName">changed property name</param>
        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        private void OnPropertyChanged(string propertyName) =>
          PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }

}