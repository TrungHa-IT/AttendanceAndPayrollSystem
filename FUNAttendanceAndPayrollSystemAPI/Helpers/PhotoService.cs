using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;

namespace FUNAttendanceAndPayrollSystemAPI.Helpers
{
    public class PhotoService
        {
            private readonly Cloudinary _cloudinary;

            public PhotoService(IOptions<CloudinarySettings> config)
            {
                var acc = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret);

                _cloudinary = new Cloudinary(acc);
            }

            public async Task<string> UploadPhotoAsync(IFormFile file)
            {
                if (file.Length <= 0) return null;

                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }

                return null;
            }
        }

}
