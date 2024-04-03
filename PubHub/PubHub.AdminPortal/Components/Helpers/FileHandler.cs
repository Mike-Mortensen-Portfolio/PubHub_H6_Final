using Microsoft.AspNetCore.Components.Forms;

namespace PubHub.AdminPortal.Components.Helpers
{
    public class FileHandler
    {
        /// <summary>
        /// Converting the <see cref="IBrowserFile"/> to a <see cref="byte[]"/>.
        /// </summary>
        /// <param name="file">The file that needs to be saved in a <see cref="byte[]"/></param>
        /// <returns>A <see cref="byte[]"/> of the file.</returns>
        public async Task<byte[]> FileToByteArray(IBrowserFile file)
        {
            long maxFilesSize = 1024 * 1024 * 5;

            using (var stream = file.OpenReadStream(maxFilesSize))
            {
                var bytes = new byte[stream.Length];

                using (var memoryStream = new MemoryStream(bytes))
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Reading a <see cref="IBrowserFile"/> to show the image back on the UI.
        /// </summary>
        /// <param name="file">The file that needs to be shown as an image.</param>
        /// <returns>A Base64String which will show the file as an image after reading its contents.</returns>
        public async Task<string> GetImageFile(IBrowserFile file)
        {
            long maxFilesSize = 1024 * 1024 * 5;

            using (var stream = file.OpenReadStream(maxFilesSize))
            {
                var bytes = new byte[stream.Length];

                using (var memoryStream = new MemoryStream(bytes))
                {
                    await stream.CopyToAsync(memoryStream);
                    var filesBytes = memoryStream.ToArray();
                    var toBaseString = "data:image/png;base64," + Convert.ToBase64String(filesBytes);
                    return toBaseString;
                }
            }
        }
    }
}
