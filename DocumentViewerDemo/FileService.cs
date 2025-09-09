using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DocumentViewerDemo
{
    /// <summary>
    /// Provides the functionality to open and save PDF files. 
    /// </summary>
    public partial class FileService
    {
        
        /// <summary>
        /// Saves the PDF stream with given file name using platform specific file saving APIs. 
        /// </summary>
        /// <param name="fileName">The file name with which the PDF needs to be saved.</param>
        /// <param name="fileStream">The stream of the PDF to be saved.</param>
        /// <returns></returns>
        public async static Task<string?> SaveAsAsync(string fileName, Stream fileStream)
        {
            return await PlatformSaveAsAsync(fileName, fileStream);
        }

        /// <summary>
        /// Writes the given stream to the specified file path.
        /// </summary>
        /// <param name="stream">The stream of the PDF file to be written.</param>
        /// <param name="filePath">The file path to which the PDF file needs to be written.</param>
        /// <returns></returns>
        static async Task WriteStream(Stream stream, string? filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                await using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                fileStream.SetLength(0);
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                await stream.CopyToAsync(fileStream).ConfigureAwait(false);
            }
        }
    }
}
