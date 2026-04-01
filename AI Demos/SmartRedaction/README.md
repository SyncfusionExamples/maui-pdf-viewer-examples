# AI‑Driven Smart Redaction in .NET MAUI PDF Viewer
This sample demonstrates how to use AI‑powered Smart Redaction with the .NET MAUI PDF Viewer to automatically identify and permanently remove sensitive information from PDF documents. It enables users to scan documents, review detected sensitive data, and redact selected content with full control.

## Process behind Smart Redaction
Smart Redaction leverages AI models to analyze PDF content and detect sensitive information based on selected categories such as personal details, contact information, and financial data. Detected content is highlighted and grouped page‑wise, allowing users to review and selectively redact information before applying permanent redaction.
In addition to AI‑based detection, the sample also supports manual redaction, enabling users to mark custom areas directly in the document.

## Steps to Use the Sample

1. Run the application to load a PDF document in the .NET MAUI PDF Viewer.
2. Select the information types that need to be redacted in Redaction panel.
3. Click the Scan button to analyze the document.
4. Review the detected items listed page‑wise.
5. Select or deselect items as required and click OK.
6. The selected content is marked for redaction in the document.
7. Once content is marked, the Redact button becomes enabled.
8. Click Redact to permanently remove the selected information.

## Manual Mark for Redaction
The sample also supports manual redaction for custom scenarios:
1. Enable Mark for Redact Check box.
2. Select any area directly on the PDF that needs to be redacted.
3. Click Redact to permanently remove the selected content.

<img src="Images/smart redaction.png" width="800"/>

**Note:** In the project directory, locate the `AIHelper.cs` file. Replace the default values in the following code snippet with your specific AI endpoint, deployment name, and API key to ensure proper functionality.

```csharp
private string aiEndpoint = "https://yourendpoint.com/";
private string deploymentName = "DEPLOYMENT_NAME";
private string apiKey = "AZURE_OPENAI_API_KEY";