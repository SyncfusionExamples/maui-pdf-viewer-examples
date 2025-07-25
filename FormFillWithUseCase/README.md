# Working with Form Filling in .NET MAUI PDF Viewer

This repository provides an example of how to fill, validate, and export a completed PDF form using the 
**Syncfusion .NET MAUI PDF Viewer**. It allows users to interactively complete a registration form, 
including text fields, date selection, course selection, and digital signature.

## Process behind PDF form filling

The sample demonstrates how form fields can be programmatically accessed and updated within a loaded PDF document. It includes:

- Loading a PDF form embedded in the app
- Displaying a custom date picker when the Date of Birth field is focused
- Validating user input for name, email, date format, course selection, and signature
- Saving the filled form and sharing it as a PDF file

## Steps to use the sample

Run the application to load a sample PDF document embedded in the project.  
Use the form fields and toolbar buttons to perform the following actions:

- **Fill Form**: Enter values in text fields, select courses, and sign the form.
- **Date Picker**: Automatically appears when focusing on the DOB field.
- **Validate**: Ensures all required fields are correctly filled before submission.
- **Share**: Saves the filled form and opens the platform’s share dialog to export the PDF.
