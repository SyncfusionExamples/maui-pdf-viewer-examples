using Syncfusion.Maui.PdfViewer;

namespace InvisbleSignatureDemo;

public partial class InvisibleSignature : ContentView
{
    InvisibleSignatureViewModel bindingContext;

    public InvisibleSignature()
    {
        InitializeComponent();
        bindingContext = new InvisibleSignatureViewModel(PdfViewer);
        PdfViewer.FormFieldValueChanged += PdfViewer_FormFieldValueChanged;
        BindingContext = bindingContext;
        bindingContext.IsEnableSave = false;
    }

    private void PdfViewer_FormFieldValueChanged(object? sender, Syncfusion.Maui.PdfViewer.FormFieldValueChangedEventArgs? e)
    {
        if ((e?.FormField as SignatureFormField)?.Signature != null)
        {
            bindingContext.IsCompleteSigningEnable = true;
            PdfViewer.AnnotationSettings.IsLocked = true;
        }
    }

    private void Save_Clicked(object? sender, EventArgs e)
    {
        bindingContext?.SaveDocument();
    }

    private async void PdfViewer_DocumentLoaded(object sender, EventArgs e)
    {
        bindingContext.IsCompleteSigningEnable = false;
        await Task.Delay(2000);
        bindingContext.ValidatedSignature(bindingContext.signatureValidatedStream);
        await Task.Delay(5000);
        bindingContext.IsSuccessMsgVisible = false;
    }
}
