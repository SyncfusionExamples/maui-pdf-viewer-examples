using UIKit;

namespace ViewOnlyPdf
{
    public partial class FileService
    {
        internal static partial async Task<string?> PlatformSaveAsAsync(string fileName, Stream stream)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await WriteStream(stream, filePath);

            var viewController = Platform.GetCurrentUIViewController();
            if (viewController == null)
            {
                return filePath;
            }

            var fileUrl = new Foundation.NSUrl(filePath, false);
            var activityController = new UIActivityViewController(new Foundation.NSObject[] { fileUrl }, null);
            if (activityController.PopoverPresentationController != null)
            {
                activityController.PopoverPresentationController.SourceView = viewController.View;
                activityController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect(viewController.View!.Bounds.Width / 2, viewController.View.Bounds.Height / 2, 0, 0);
            }

            await viewController.PresentViewControllerAsync(activityController, true);
            return filePath;
        }
    }
}
