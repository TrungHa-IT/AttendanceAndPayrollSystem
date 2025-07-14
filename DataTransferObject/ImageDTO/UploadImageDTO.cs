using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.ImageDTO
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
    }
}
