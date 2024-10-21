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
        /// <param name="sender">The PdfViewer control that triggered the FormFieldFocusChanged event.</param>
        /// <param name="e">Event arguments containing information about the form field focus change, including which field gained or lost focus.</param>
        private void PdfViewer_FormFieldFocusChanged(object sender, Syncfusion.Maui.PdfViewer.FormFieldFocusChangedEvenArgs e)
        {
            // Check whether the focused field is the date of birth field and let the user choose a date.
            if (e.FormField != null && e.FormField.Name == "dob")
            {
                if (e.HasFocus)
                {
                    datePickerGrid.IsVisible = true;

                    // Hide the soft keypad shown when clicking the text form field on Android and iOS devices.
#if ANDROID
                    if (this.Handler != null && this.Handler.PlatformView is Android.Views.View inputView)
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
        /// Handles the Ok button click event of the DatePicker, allowing the user to confirm their date selection.
        /// </summary>
        /// <param name="sender">The DatePicker control that triggered the Ok button click event.</param>
        /// <param name="e">Event arguments associated with the button click event.</param>
        private void DatePicker_OkButtonClicked(object sender, EventArgs e)
        {
            // Find the form field named "dob" (date of birth).
            FormField field = pdfViewer.FormFields.Where(f => f.Name == "dob").First();

            if (field is TextFormField dateOfBirthField)
            {
                if (datePicker.SelectedDate != null)
                {
                    // Remove the time and retain only the date from the selected DateTime object.
                    string date = datePicker.SelectedDate.Value.ToString().Split(' ')[0];

                    // Convert the date to dd/mm/yyyy format.
                    string[] dateComponents = date.Split("/");
                    date = $"{dateComponents[1]}/{dateComponents[0]}/{dateComponents[2]}";

                    // Update the Date of Birth form field with the formatted date.
                    dateOfBirthField.Text = date;
                }
            }

            // Hide the DatePicker once the selection is complete.
            datePickerGrid.IsVisible = false;
        }

        /// <summary>
        /// Handles the Cancel button click event of the DatePicker, allowing the user to close the DatePicker without making a selection.
        /// </summary>
        /// <param name="sender">The DatePicker control that triggered the Cancel button click event.</param>
        /// <param name="e">Event arguments associated with the button click event.</param>
        private void DatePicker_CancelButtonClicked(object sender, EventArgs e)
        {
            // Hide the DatePicker grid when the Cancel button is clicked.
            datePickerGrid.IsVisible = false;
        }

        /// <summary>
        /// Shares the filled PDF form document.
        /// </summary>
        /// <param name="sender">The button control that triggered the share action.</param>
        /// <param name="e">Event arguments associated with the button click event.</param>
        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            // Validate the form data before sharing.
            bool isFormDataValid = await ValidateFormData();
            if (isFormDataValid)
            {
                // Create a memory stream and save the filled PDF document.
                MemoryStream outputStream = new MemoryStream();
                await pdfViewer.SaveDocumentAsync(outputStream);

                // Generate a temporary file path for the PDF and save the document.
                string tempFilePath = Path.GetTempFileName().Replace(".tmp", ".pdf");
                File.WriteAllBytes(tempFilePath, outputStream.ToArray());

                // If the file exists, open the platform's share dialog to share the PDF.
                if (File.Exists(tempFilePath))
                {
                    var shareFileRequest = new ShareFileRequest
                    {
                        Title = "Share filled form",
                        File = new ShareFile(tempFilePath),
                    };
                    await Share.Default.RequestAsync(shareFileRequest);
                    outputStream.Dispose();
                }
            }
        }

        /// <summary>
        /// Validates the filled form data .
        /// </summary>
        /// <returns>Returns "True" if all validations pass, otherwise "False".</returns>
        async Task<bool> ValidateFormData()
        {
            List<string> errors = new List<string>();

            foreach (FormField formField in pdfViewer.FormFields)
            {
                if (formField is TextFormField textFormField)
                {
                    if (textFormField.Name == "Name")
                    {
                        // Validate Name field
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
                        // Validate Date of Birth field
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
                        // Validate Email field
                        if (string.IsNullOrEmpty(textFormField.Text))
                        {
                            errors.Add("Email is required.");
                        }
                        else if (!Regex.IsMatch(textFormField.Text, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                        {
                            errors.Add("Email should be in the correct format.");
                        }
                    }
                }
                else if (formField is ListBoxFormField listBoxFormField)
                {
                    // Validate ListBox 
                    if (listBoxFormField.SelectedItems.Count == 0)
                    {
                        errors.Add("Please select at least one course.");
                    }
                }
                else if (formField is SignatureFormField signatureFormField)
                {
                    // Validate Signature field
                    if (signatureFormField.Signature == null)
                    {
                        errors.Add("Please sign the document.");
                    }
                }
            }

            // Show errors if any, otherwise validation passed
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
