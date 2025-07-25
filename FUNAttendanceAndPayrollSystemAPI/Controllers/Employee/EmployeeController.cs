using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.EmployeeDTOS;
using DataTransferObject.LeaveDTO;
using FUNAttendanceAndPayrollSystemAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Repository.EmRepository;
using System.Collections;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Employee
{
    [Route("Employee/")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly PhotoService _photoService;
        private readonly EmailService _emailService;
        public EmployeeController(PhotoService photoService, EmailService emailService)
        {
            _photoService = photoService;
            _emailService = emailService;
        }
        private readonly IEmployeeRepository repository = new EmployeeRepository();

        [HttpPut("updateCertificate")]
        public async Task<IActionResult> UpdateCertificate([FromBody] CertificateDTO certificateDTO)
        {
            if (certificateDTO == null || certificateDTO.Id <= 0)
                return BadRequest("Invalid certificate data.");

            var employeeEmail = EmployeeDAO.GetEmployeeEmailCertificate(certificateDTO.Id);

            var result = EmployeeDAO.UpdateCertificateStatus(certificateDTO);
            if (!result)
                return BadRequest("Certificate update failed.");

            string subject = $"Your Certificate Request has been {certificateDTO.Status}";
            string body = $@"
        <h3>Hello {certificateDTO.EmployeeName},</h3>
        <p>Your certificate request has been 
        <span style='color:{(certificateDTO.Status == "Approved" ? "green" : "red")}'><strong>{certificateDTO.Status}</strong></span>.</p>
        <p>Regards,<br/>{certificateDTO.ApprovedByName} - HR Department</p>";

            await _emailService.SendEmailAsync(employeeEmail, subject, body);

            return Ok("Certificate updated and email sent.");
        }
       
        [HttpGet("ListEmployees")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees() => repository.GetEmployees();

        [HttpGet("ListStaff")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetStaffs() => repository.GetStaffs();

        [HttpGet("GetCertificate")]
        public ActionResult<IEnumerable<CertificateDTO>> GetCertificate() => EmployeeDAO.GetCertificate();

        [HttpGet("GetCertificateByApprover")]
        public ActionResult<IEnumerable<CertificateDTO>> GetCertificateByApprover(int id) => EmployeeDAO.GetCertificateByApprover(id);

        [HttpGet("ListCertificateBonus")]
        public ActionResult<IEnumerable<CertificateBonusDTO>> GetCertificateBonus() => EmployeeDAO.GetCertificateBonus();

        [HttpGet("MyAttendance")]
        public IActionResult GetAttendanceById(int emp, int? month = null, int? year = null)
        {
            var empAttendance = repository.getAttendanceById(emp, month, year);
            if (empAttendance == null)
            {
                return BadRequest("emp id values must not be null.");
            }
            return Ok(empAttendance);
        }
     
        [HttpPost("RegisterScheduleOT")]
        public IActionResult RegisterScheduleOT([FromBody] OTRequestDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.Reason))
            {
                return BadRequest("Reason must not be empty.");
            }

            try
            {
                var empAttendance = repository.bookScheduleOverTime(model.Emp,
            model.OtDate,
            model.StartTime,
            model.EndTime,
            model.Reason);

                if (empAttendance == null)
                {
                    return NotFound("Employee not found or unable to register OT.");
                }

                return Ok(empAttendance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex.Message}");
            }
        }

        [HttpGet("Profile/{id}")]
        public IActionResult GetInformationProfile(int id)
        {
            if(id == null)
            {
                return BadRequest("Please login before watch profile");
            }
            try
            {
                var getInfor = repository.getInformationProfile(id);
                return Ok(getInfor);
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("getTotalHourByEmployee")]
        public ActionResult<IEnumerable<EmployeeDTO>> getTotalHourByEmployee(int? month, int? year, int? departmenId) => repository.GetEmployeesTotalTimeByMonth(month,year,departmenId);


        [HttpPut("UpdateBasicInfo")]
        public IActionResult UpdateBasicInfo([FromBody] EmployeeProfileDTO updated)
        {
            if (updated == null || updated.EmployId == 0)
            {
                return BadRequest("Invalid employee information.");
            }

            var employee = new BusinessObject.Models.Employee
            {
                EmployId = updated.EmployId,
                EmployeeName = updated.EmployeeName,
                Dob = updated.Dob.Value.ToDateTime(TimeOnly.MinValue),
                Gender = updated.Gender,
                PhoneNumber = updated.PhoneNumber,
                Address = updated.Address,
                Position = updated.Position,
            };

            bool result = repository.UpdateBasicInfo(employee);

            if (!result)
            {
                return NotFound("Employee not found.");
            }

            return Ok("Basic information updated successfully.");
        }

        [HttpPost("AddSkills")]
        public IActionResult AddSkills([FromBody] SkillDTO skillDto)
        {
            if (skillDto == null)
                return BadRequest("Invalid skill data");

            var result = EmployeeDAO.AddSkill(skillDto);
            if (result)
                return Ok(new { message = "Skill added successfully" });

            return BadRequest("Failed to add skill");
        }

        [HttpPost("AddCertificate")]
        public async Task<IActionResult> AddCertificate([FromForm] CertificateDTO certificate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Upload ảnh nếu có
            var imageUrls = new List<string>();
            if (certificate.ImageFiles != null && certificate.ImageFiles.Count > 0)
            {
                foreach (var file in certificate.ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var url = await _photoService.UploadPhotoAsync(file); // 👈 thay bằng dịch vụ thật của bạn
                        imageUrls.Add(url);
                    }
                }
            }

            // Gán lại danh sách ảnh vào DTO
            certificate.ImageUrls = imageUrls;

            // Lưu vào DB thông qua DAO
            var result = EmployeeDAO.AddCertificate(certificate);
            if (result)
                return Ok(new { message = "Certificate added successfully" });

            return BadRequest("Failed to add certificate");
        }


        [HttpPost("AddCertificateBonus")]
        public IActionResult AddCertificateBonus([FromBody] CertificateBonusDTO certificateBonusDTO)
        {
            if (certificateBonusDTO == null)
            {
                return BadRequest("Invalid certificate bonus data");
            }
            var result = EmployeeDAO.AddCertificateBonus(certificateBonusDTO);
            if (result)
            {
                return Ok(new { message = "Certificate bonus added successfully" });
            }
            return BadRequest("Failed to add certificate bonus");
        }

        [HttpPost("UpdateSkills")]
        public IActionResult UpdateSkills([FromBody] UpdateSkillsRequest request)
        {
            int employeeId = request.EmployeeId;
            var skills = request.Skills;
            if (employeeId == 0 || skills == null)
            {
                return BadRequest("Invalid skill update request.");
            }

            var skillEntities = skills.Select(s => new EmployeeSkill
            {
                Id = s.SkillId,
                SkillName = s.SkillName,
                Level = s.Level,
                EmployeeId = employeeId
            }).ToList();

            bool result = repository.UpdateSkills(employeeId, skillEntities);
            if (!result)
            {
                return NotFound("Employee not found.");
            }

            return Ok("Skills updated successfully.");
        }


        [HttpPost("UpdateCertificates")]
        public IActionResult UpdateCertificates([FromBody] UpdateCertificatesRequest request)
        {
            int employeeId = request.EmployeeId;
            var certs = request.Certificates;
            if (employeeId == 0 || certs == null)
            {
                return BadRequest("Invalid certificate update request.");
            }

            var certEntities = certs.Select(c => new EmployeeCertificate
            {
                Id = c.Id,
                CertificateName = c.CertificateName,
                ExpiryDate = c.ExpiryDate,
                IssueDate = c.IssueDate,
                EmployeeId = employeeId
            }).ToList();

            bool result = repository.UpdateCertificates(employeeId, certEntities);
            if (!result)
            {
                return NotFound("Employee not found.");
            }

            return Ok("Certificates updated successfully.");
        }
        [HttpPut("UpdateEmployee/{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] EmployeeUpdateDTO updatedEmployee)
        {
            if (id != updatedEmployee.EmployId)
            {
                return BadRequest("ID mismatch");
            }

            bool result = repository.UpdateEmployee(updatedEmployee);

            if (!result)
            {
                return NotFound("Employee not found");
            }

            return Ok("Employee updated successfully");
        }



        [HttpDelete("DeleteEmployee/{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                bool result = repository.DeleteEmployee(id);

                if (!result)
                {
                    return NotFound("Employee not found or deletion failed");
                }

                return Ok(new { success = true, message = "Employee and all related data deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error deleting employee: {ex.Message}" });
            }
        }

        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Đường dẫn thư mục wwwroot/uploads
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Ghi file lên server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Trả về đường dẫn tương đối để lưu vào database
            return "/uploads/" + fileName;
        }

    }
}
