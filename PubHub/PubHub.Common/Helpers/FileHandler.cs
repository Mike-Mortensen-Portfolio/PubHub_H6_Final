using Microsoft.AspNetCore.Components.Forms;

namespace PubHub.Common.Helpers
{
    public class FileHandler
    {
        // TODO (JBN): Figure out how to handle big amount of files, as we currently cannot handle big files.
        /// <summary>
        /// Converting the <see cref="IBrowserFile"/> to a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="file">The file that needs to be saved in a <see cref="byte[]"/></param>
        /// <returns>A <see cref="byte[]"/> of the file.</returns>
        public async Task<byte[]> FileToByteArray(IBrowserFile file)
        {
            var bufferSize = 1024 * 1024; // 1 MB buffer size
            await using var outputStream = new MemoryStream();
            await using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 1024);

            var buffer = new byte[bufferSize];
            int readBytes;
            // Continue reading from the file stream until no more data is available.
            while ((readBytes = await stream.ReadAsync(buffer)) > 0)
            {
                await outputStream.WriteAsync(buffer.AsMemory(0, readBytes));
            }

            var output = outputStream.ToArray();
            return output;
        }

        /// <summary>
        /// Reading a <see cref="IBrowserFile"/> to show the image back on the UI.
        /// </summary>
        /// <param name="file">The file that needs to be shown as an image.</param>
        /// <returns>A Base64String which will show the file as an image after reading its contents.</returns>
        public async Task<string> GetImageFile(IBrowserFile file)
        {
            long maxFilesSize = 1024 * 1024 * 5;

            using var stream = file.OpenReadStream(maxFilesSize);
            var bytes = new byte[stream.Length];

            using var memoryStream = new MemoryStream(bytes);
            await stream.CopyToAsync(memoryStream);
            var filesBytes = memoryStream.ToArray();
            var toBaseString = "data:image/png;base64," + Convert.ToBase64String(filesBytes);
            return toBaseString;
        }
    }
}
