using Syncfusion.Maui.PdfViewer;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FormFillWithUseCase
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            // Load the PDF document.
            Stream? stream = typeof(MainPage).GetTypeInfo().Assembly.GetManifestResourceStream("FormFillWithUseCase.Assets.workshop_registration.pdf");
            pdfViewer.LoadDocument(stream);
        }
        /// <summary>	
        /// Handling the focus change on the DOB field to display DatePicker.
        /// </summary>
        private void pdfViewer_FormFieldFocusChanged(object sender, Syncfusion.Maui.PdfViewer.FormFieldFocusChangedEvenArgs e)
        {
            // Check whether the focused field is the date of birth field and let the user choose a date.
            if (e.FormField!=null && e.FormField.Name == "dob")
            {
                if (e.HasFocus)
                {
                    datePickerGrid.IsVisible = true;

                    // Hide the soft keypad shown when clicking the text form field on Android and iOS devices.
#if ANDROID
           if(this.Handler!=null && this.Handler.PlatformView is Android.Views.View inputView)
           {
               using (var inputMethodManager = (Android.Views.InputMethods.InputMethodManager?)inputView.Context?
                 .GetSystemService(Android.Content.Context.InputMethodService))
                   {
                       if (inputMethodManager != null)
                       {
                           var token = Platform.CurrentActivity?.CurrentFocus?.WindowToken;
                           inputMethodManager.HideSoftInputFromWindow(token, Android.Views
                             .InputMethods.HideSoftInputFlags.None);
                           Platform.CurrentActivity?.Window?.DecorView.ClearFocus();
                       }
                    }
           }
#elif IOS && !MACCATALYST
           UIKit.UIApplication.SharedApplication.SendAction(new ObjCRuntime.Selector("resignFirstResponder"), null, null, null);
#endif
                }
            }
        }

        /// <summary>
        /// Handle DatePicker's Ok button action.
        /// </summary>
        private void datePicker_OkButtonClicked(object sender, EventArgs e)
        {
            FormField field = pdfViewer.FormFields.Where(f => f.Name == "dob").First();
            if (field is TextFormField dateOfBirthField )
            {
                if (datePicker.SelectedDate != null)
                {
                    // Remove time and retain only the date from the selected DateTime object.
                    string date = datePicker.SelectedDate.Value.ToString().Split(' ')[0];

                    // Convert the date to dd/mm/yyyy format.
                    string[] dateComponents = date.Split("/");
                    date = $"{dateComponents[1]}/{dateComponents[0]}/{dateComponents[2]}";

                    dateOfBirthField.Text = date;
                }
            }
            datePickerGrid.IsVisible = false;
        }

        /// <summary>
        /// Handle DatePicker's Cancel button action.
        /// </summary>
        private void datePicker_CancelButtonClicked(object sender, EventArgs e)
        {
            datePickerGrid.IsVisible = false;
        }
        /// <summary>
        /// Share the PDF form externally via platform's share dialog
        /// </summary>
        private async void shareButton_Clicked(object sender, EventArgs e)
        {
            bool isFormDataValid = await ValidateFormData();
            if (isFormDataValid)
            {
                MemoryStream outputStream = new MemoryStream();
                await pdfViewer.SaveDocumentAsync(outputStream);

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workshop_registration.pdf");
                File.WriteAllBytes(filePath, outputStream.ToArray());

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Share filled form",
                    File = new ShareFile(filePath)
                });
            }
        }

        /// <summary>
        /// Perform validations on the form data filled
        /// </summary>
        /// <returns>"True", if all the validations are passed. Otherwise, "False".</returns>
        async Task<bool> ValidateFormData()
        {
            List<string> errors = new List<string>();

            foreach (FormField formField in pdfViewer.FormFields)
            {
                if (formField is TextFormField textFormField)
                {
                    if (textFormField.Name == "Name")
                    {
                        if (string.IsNullOrEmpty(textFormField.Text))
                        {
                            errors.Add("Name is required.");
                        }
                        else if (textFormField.Text.Length < 3)
                        {
                            errors.Add("Name should be at least 3 characters.");
                        }
                        else if (textFormField.Text.Length > 30)
                        {
                            errors.Add("Name should not exceed 30 characters.");
                        }
                        else if (Regex.IsMatch(textFormField.Text, @"[0-9]"))
                        {
                            errors.Add("Name should not contain numbers.");
                        }
                        else if (Regex.IsMatch(textFormField.Text, @"[!@#$%^&*(),.?""{}|<>]"))
                        {
                            errors.Add("Name should not contain special characters.");
                        }
                    }
                    else if (textFormField.Name == "dob")
                    {
                        if (string.IsNullOrEmpty(textFormField.Text))
                        {
                            errors.Add("Date of birth is required.");
                        }
                        else if (!DateTime.TryParseExact(textFormField.Text, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                        {
                            errors.Add("Date of birth should be in d/M/yyyy format.");
                        }
                    }
                    else if (textFormField.Name == "email")
                    {
                        if (string.IsNullOrEmpty(textFormField.Text))
                        {
                            errors.Add("Email is required.");
                        }
                        else if (!Regex.IsMatch(textFormField.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                        {
                            errors.Add("Email should be in correct format.");
                        }
                    }
                }
                else if (formField is ListBoxFormField listBoxFormField)
                {
                    if (listBoxFormField.SelectedItems.Count == 0)
                    {
                        errors.Add("Please select at least one course.");
                    }
                }
                else if (formField is SignatureFormField signatureFormField)
                {
                    if (signatureFormField.Signature == null)
                    {
                        errors.Add("Please sign the document.");
                    }
                }
            }

            if (errors.Count > 0)
            {
                await DisplayAlert("Errors", string.Join("\n", errors), "Try Again");
                return false;
            }
            else
                return true;
        }
    }

}
