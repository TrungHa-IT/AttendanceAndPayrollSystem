using BusinessObject.Models;
using DataAccess.EmployeeDAO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
using System.Collections;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Employee
{
    [Route("Employee/")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository repository = new EmployeeRepository();
        [HttpGet("ListEmployees")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetEmployees() => repository.GetEmployees();

        [HttpGet("ListStaff")]
        public ActionResult<IEnumerable<EmployeeDTO>> GetStaffs() => repository.GetStaffs();

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
                Id = c.CertificateId,
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
