using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FUNAttendanceAndPayrollSystemAPI.Helpers;
using DataTransferObject.ImageDTO;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Image
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        private readonly PhotoService _photoService;

        public UploadImageController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("photo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadImageDTO request)
        {
            var imageUrl = await _photoService.UploadPhotoAsync(request.File);

            if (imageUrl == null)
                return BadRequest("Upload failed");

            return Ok(new { Url = imageUrl });
        }
    }
}
