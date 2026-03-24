# How to detect whether changes are made in the PDF document?

This project demonstrates how to detect the changes made in the PDF document loaded in the .NET MAUI PDFViewer control.

## Prerequisites

1. A .NET MAUI project set up.
2. The [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer) package is installed.

## Steps

### 1.Install Required NuGet Package

To get started, create a new [Maui App](https://dotnet.microsoft.com/en-us/learn/maui/first-app-tutorial/create) and ensure the following package is installed in your .NET MAUI project:

[Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer)

You can install this package using the NuGet Package Manager or the NuGet CLI.

### 2. Initialize the PDF Viewer in XAML

Start by adding the Syncfusion PDF Viewer control to your XAML file.

**a. Add the Syncfusion namespace in your MainPage.xaml:**

Define the XAML namespace to enable access to the PDF Viewer.

**XAML:**

```xaml
     xmlns:syncfusion="clr-namespace:Syncfusion.Maui.PdfViewer;assembly=Syncfusion.Maui.PdfViewer"
```

**b. Add the PDF Viewer control to your layout:**

Initialize the SfPdfViewer in the XAML file. This will display the PDF Viewer in your app. You can load any PDF document into this Viewer and wire the DocumentLoaded event for the PdfViewer.

**XAML:**

```xaml
     <Grid>
        <syncfusion:SfPdfViewer x:Name="pdfViewer" />
     </Grid>
```

### 3. PdfViewerViewModel Class

In your `PdfViewerViewModel.cs`, store the stream of a PDF document embedded with your resource.

```csharp
    
     internal class PdfViewerViewModel: INotifyPropertyChanged
     {
         private Stream? m_pdfDocumentStream;

         /// <summary>
         /// An event to detect the change in the value of a property.
         /// </summary>
         public event PropertyChangedEventHandler? PropertyChanged;

         /// <summary>
         /// The PDF document stream that is loaded into the instance of the PDF viewer. 
         /// </summary>
         public Stream? PdfDocumentStream
         {
             get
             {
                 return m_pdfDocumentStream;
             }
             set
             {
                 m_pdfDocumentStream = value;
                 OnPropertyChanged("PdfDocumentStream");
             }
         }

         /// <summary>
         /// Constructor of the view model class
         /// </summary>
         public PdfViewerViewModel()
         {
             //Accessing the PDF document that is added as embedded resource as stream.
             m_pdfDocumentStream = typeof(App).GetTypeInfo().Assembly.GetManifestResourceStream("DetectPdfChanges.Assets.form_document.pdf");
         }

         public void OnPropertyChanged(string name)
         {
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
         }
     }
```

### 4. Binding the PDF stream to the .NET MAUI PDFViewer

Bind the PDF stream obtained to the [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource) property in the .NET MAUI PDFViewer.

**a. Namespace Declaration**

Registers the DetectPdfChanges namespace

```xaml
      xmlns:local="clr-namespace:DetectPdfChanges"
```

**b. Binding Context Setup**

Sets the ContentPage data context to an instance of PdfViewerViewModel. Enables data binding between UI elements and properties in the view model.

```xaml
      <ContentPage.BindingContext>
          <local:PdfViewerViewModel />
      </ContentPage.BindingContext>

```

**c. PDF Viewer Configuration**

Binds its [DocumentSource](https://help.syncfusion.com/cr/maui/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_DocumentSource) to the PdfDocumentStream property in the PDfViewerViewModel.cs.

```xaml
      <syncfusion:SfPdfViewer x:Name="PdfViewer" 
                        DocumentSource="{Binding PdfDocumentStream}"/>
```

### 5. Event Handler for Detecting PDF Form Field Changes in SfPdfViewer.

The [FormFieldValueChanged](https://help.syncfusion.com/cr/document-processing/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_FormFieldValueChanged) event handler is implemented to monitor and respond to changes in form fields within the PDF document.

```
     private void PdfViewer_FormFieldValueChanged(object sender, FormFieldValueChangedEventArgs e)
     {
         if (Application.Current != null)
             Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"{e.FormField} value is changed.", "OK");
     }
```

### 6. Event Handler for Detecting PDF Annotation Changes in SfPdfViewer.

**a. AnnotationAdded Event Handler**

The [AnnotationAdded](https://help.syncfusion.com/cr/document-processing/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_AnnotationAdded) event handler is triggered while adding new annotation to the PDF document loaded in the PDFViewer.

```csharp
      private void PdfViewer_AnnotationAdded(object sender, AnnotationEventArgs e)
      {
          if (Application.Current != null)
              Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", "Annotation is added.", "OK");
      }
```

**b. AnnotationEdited Event Handler**

The [AnnotationEdited](https://help.syncfusion.com/cr/document-processing/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_AnnotationEdited) event handler is triggered when existing annotation is edited in the PDFViewer.

```csharp
      private void PdfViewer_AnnotationEdited(object sender, AnnotationEventArgs e)
      {
          if (Application.Current != null)
              Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"Annotation is edited.", "OK");
      }
```

**c. AnnotationRemoved Event Handler**

The [AnnotationRemoved](https://help.syncfusion.com/cr/document-processing/Syncfusion.Maui.PdfViewer.SfPdfViewer.html#Syncfusion_Maui_PdfViewer_SfPdfViewer_AnnotationRemoved) event handler is triggered when removing an annotation from the PDF document loaded in the PDFViewer.

```csharp
      private void PdfViewer_AnnotationRemoved(object sender, AnnotationEventArgs e)
      {
          if(Application.Current != null)
              Application.Current.Windows[0].Page?.DisplayAlert("PDF Edited", $"Annotation is removed.", "OK");
      }
```

### 7. Wiring PDF Edit Detection Events in .NET MAUI PDFViewer.

The event handlers are wired to detect the changes made in the PDF document.

```xaml
      <syncfusion:SfPdfViewer x:Name="PdfViewer" 
                DocumentSource="{Binding PdfDocumentStream}"
                FormFieldValueChanged="PdfViewer_FormFieldValueChanged"
                AnnotationAdded="PdfViewer_AnnotationAdded"
                AnnotationEdited="PdfViewer_AnnotationEdited"
                AnnotationRemoved="PdfViewer_AnnotationRemoved"/>
```

## Run the App

1. Build and run the application in all platforms.
2. Add, edit, and remove the annotation.
3. Edit the form fields.