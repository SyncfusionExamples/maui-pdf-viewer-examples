using UIKit;

namespace ViewOnlyPdf
{
    public partial class FileService
    {
        internal static partial async Task<string?> PlatformSaveAsAsync(string fileName, Stream stream)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            await WriteStream(stream, filePath);
            return filePath;
        }
    }
}
