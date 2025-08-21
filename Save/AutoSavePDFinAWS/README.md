# PDF Auto-Save Edits using .NET MAUI PDF Viewer and AWS Amazon S3 storage

A .NET MAUI application that demonstrates automatic saving of PDF edits using the Syncfusion PDF Viewer control and AWS Amazon S3 storage. This application provides real-time auto-save functionality whenever annotations, form fields, or other PDF modifications are made.

## 🚀 Features

- **Auto-Save Functionality**: Automatically saves PDF changes when annotations are added, edited, or removed
- **Manual Save Option**: Toggle auto-save on/off with manual save capability
- **Real-time Notifications**: Visual feedback showing the current status of operations
- **Cross-Platform**: Runs on Android, iOS, macOS, and Windows
- **AWS-Integration**: To store securely and efficient loading with seamless integration across mobile and desktop platforms.

## 📄 Dependencies

- [Syncfusion.Maui.PdfViewer](https://www.nuget.org/packages/Syncfusion.Maui.PdfViewer): PDF viewing and editing capabilities
- [AWSSDK.S3](https://www.nuget.org/packages/AWSSDK.S3):Amazon Simple Storage Service (Amazon S3), provides developers and IT teams with secure, durable, highly-scalable object storage

## 📱 How to Use

### 1. Launch the Application
- Start the application on your preferred platform
- You'll see the main interface with a toolbar at the top

### 2. Open a PDF File
- Click the **📂 Open** button (folder icon) in the toolbar
- Select a PDF file from your AWS Amazon S3 storage
- The PDF will load in the viewer

### 3. Auto-Save Configuration
- **Auto Save Enabled** (default): Any edits automatically save to the original file in the AWS Amazon S3 storage
- **Auto Save Disabled**: Use the **💾 Save** button to manually save changes

### 4. Edit the PDF
- Add annotations (highlights, notes, drawings)
- Fill form fields
- Watch the notification area for save status updates

### 5. Monitor Status
- The notification area shows current operation status:
  - File opening/closing status
  - Auto-save operations
  - Manual save confirmations
  
### 6. Verify Changes
- **Check the Input File**: After making edits and saving (either automatically or manually) in AWS Amazon S3 storage
- **Navigate to File Location**: Go to the original PDF file location on AWS Amazon S3 storage
- **Open with PDF Reader**: You can see that the file has been permanently modified
- **Confirm Edits Persist**: All annotations, form field changes, and modifications are saved to the original file in the AWS Amazon S3 storage