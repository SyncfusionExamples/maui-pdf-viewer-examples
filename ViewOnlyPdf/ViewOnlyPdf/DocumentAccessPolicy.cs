using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewOnlyPdf
{
    /// <summary>
    /// Stand-in for a backend or admin configuration that decides what a user may do with a document.
    /// The PDF viewer has no built-in view-only mode, so the sample applies these flags itself.
    /// </summary>
    public class DocumentAccessPolicy : INotifyPropertyChanged
    {
        bool isApplyingViewOnly;
        bool isViewOnly = true;
        bool allowSave;
        bool allowPrint;
        bool allowAnnotations;
        bool allowTextSelection;
        bool allowFormFields;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// When true, save, print, annotations, and text selection are all denied.
        /// </summary>
        public bool IsViewOnly
        {
            get => isViewOnly;
            set
            {
                if (isViewOnly == value)
                {
                    return;
                }

                isViewOnly = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsNormalMode));
                OnPropertyChanged(nameof(AccessTitle));
                if (value)
                {
                    isApplyingViewOnly = true;
                    AllowSave = false;
                    AllowPrint = false;
                    AllowAnnotations = false;
                    AllowTextSelection = false;
                    AllowFormFields = false;
                    isApplyingViewOnly = false;
                }
            }
        }

        /// <summary>
        /// When true, save, print, annotations, and text selection are all allowed.
        /// </summary>
        public bool IsNormalMode
        {
            get => AllowSave && AllowPrint && AllowAnnotations && AllowTextSelection && AllowFormFields;
            set
            {
                if (!value || IsNormalMode)
                {
                    return;
                }

                isApplyingViewOnly = true;
                AllowSave = true;
                AllowPrint = true;
                AllowAnnotations = true;
                AllowTextSelection = true;
                AllowFormFields = true;
                isViewOnly = false;
                isApplyingViewOnly = false;
                OnPropertyChanged(nameof(IsViewOnly));
                OnPropertyChanged(nameof(IsNormalMode));
                OnPropertyChanged(nameof(AccessTitle));
                OnPropertyChanged(nameof(ShowViewOnlyBadge));
            }
        }

        public bool AllowSave
        {
            get => allowSave;
            set => SetRestricted(ref allowSave, value);
        }

        public bool AllowPrint
        {
            get => allowPrint;
            set => SetRestricted(ref allowPrint, value);
        }

        public bool AllowAnnotations
        {
            get => allowAnnotations;
            set => SetRestricted(ref allowAnnotations, value);
        }

        public bool AllowTextSelection
        {
            get => allowTextSelection;
            set => SetRestricted(ref allowTextSelection, value);
        }

        public bool AllowFormFields
        {
            get => allowFormFields;
            set => SetRestricted(ref allowFormFields, value);
        }

        public bool ShowViewOnlyBadge =>
            !AllowSave || !AllowPrint || !AllowAnnotations || !AllowTextSelection || !AllowFormFields;

        public string AccessTitle
        {
            get
            {
                if (IsNormalMode)
                {
                    return "Normal";
                }

                if (IsViewOnly)
                {
                    return "View only";
                }

                var allowed = new List<string>();
                if (AllowSave)
                {
                    allowed.Add("Save");
                }

                if (AllowPrint)
                {
                    allowed.Add("Print");
                }

                if (AllowAnnotations)
                {
                    allowed.Add("Annotate");
                }

                if (AllowFormFields)
                {
                    allowed.Add("Form fields");
                }

                if (AllowTextSelection)
                {
                    allowed.Add("Select text");
                }

                return allowed.Count == 0 ? "View only" : string.Join(", ", allowed);
            }
        }

        void SetRestricted(ref bool field, bool value, [CallerMemberName] string? propertyName = null)
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged(propertyName);
            OnPropertyChanged(nameof(ShowViewOnlyBadge));
            OnPropertyChanged(nameof(IsNormalMode));
            OnPropertyChanged(nameof(AccessTitle));

            if (!isApplyingViewOnly)
            {
                bool allRestricted = !AllowSave && !AllowPrint && !AllowAnnotations && !AllowTextSelection && !AllowFormFields;
                if (isViewOnly != allRestricted)
                {
                    isViewOnly = allRestricted;
                    OnPropertyChanged(nameof(IsViewOnly));
                    OnPropertyChanged(nameof(AccessTitle));
                }
            }
        }

        void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
