using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
using DataTransferObject.EmployeeDTOS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EmployeeDAO
{
    public class EmployeeDAO
    {
        private readonly FunattendanceAndPayrollSystemContext _MyDb;
        public EmployeeDAO()
        {
            _MyDb = new FunattendanceAndPayrollSystemContext();
        }

        public static List<EmployeeDTO> GetEmployees()
        {
            var listEmployees = new List<EmployeeDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listEmployees = context.Employees.Include(emp => emp.Department)
                        .Where(ep => ep.Position != null) 
                        .Select(ep => new EmployeeDTO
                        {
                            EmployId = ep.EmployId,
                            EmployeeName = ep.EmployeeName,
                            Dob = ep.Dob,
                            Email = ep.Email,
                            PhoneNumber = ep.PhoneNumber,
                            Gender = ep.Gender,
                            Address = ep.Address,
                            Position = ep.Position,
                            Salary = ep.Salary,
                            Status = ep.Status,
                            Image = ep.Image,
                            CreateAt = ep.CreatedAt,
                            DepartmentId = ep.DepartmentId,
                            DepartmentName = ep.Department.DepartmentName
                        }).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listEmployees;
        }
        public static List<EmployeeDTO> GetStaffs()
        {
            var listEmployees = new List<EmployeeDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listEmployees = context.Employees.Include(emp => emp.Department).Where(ep => ep.Position.Equals("Staff"))
                        .Select(ep => new EmployeeDTO
                        {
                            EmployId = ep.EmployId,
                            EmployeeName = ep.EmployeeName,
                            Dob = ep.Dob,
                            Email = ep.Email,
                            PhoneNumber = ep.PhoneNumber,
                            Gender = ep.Gender,
                            Address = ep.Address,
                            Position = ep.Position,
                            DepartmentId = ep.DepartmentId,
                            DepartmentName = ep.Department.DepartmentName
                        }).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listEmployees;
        }

        public static List<CertificateBonusDTO> GetCertificateBonus()
        {
            var listCertificateBonus = new List<CertificateBonusDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listCertificateBonus = context.CertificateBonusRates
                        .Select(ep => new CertificateBonusDTO
                        {
                            BonusAmount = ep.BonusAmount,
                            CertificateName = ep.CertificateName,
                            Id = ep.Id,
                        }).ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listCertificateBonus;
        }

        public static List<CertificateDTO> GetCertificate()
        {
            var listCertificateBonus = new List<CertificateDTO>();
            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    listCertificateBonus = context.EmployeeCertificates
                        .Include(e => e.Employee)
                        .Include(e => e.CertificateBonusRate)
                        .Include(e => e.Images) 
                        .Select(ep => new CertificateDTO
                        {
                            Id = ep.Id,
                            EmployeeId = ep.EmployeeId,
                            CertificateName = ep.CertificateName,
                            IssueDate = ep.IssueDate,
                            ExpiryDate = ep.ExpiryDate,
                            Status = ep.Status,
                            ApprovedBy = ep.ApprovedBy,
                            ApprovedByName = ep.Employee.EmployeeName,
                            CertificateBonusRateId = ep.CertificateBonusRateId,
                            BonusRate = ep.CertificateBonusRate != null ? ep.CertificateBonusRate.BonusAmount : null,
                            EmployeeName = ep.Employee.EmployeeName,
                            ImageUrls = ep.Images.Select(img => img.ImageUrl).ToList()
                        })
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return listCertificateBonus;
        }


        public static List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year, int? departmenId)
        {
            var listEmployees = new List<EmployeeDTO>();

            try
            {
                using (var context = new FunattendanceAndPayrollSystemContext())
                {
                    int currentMonth = month ?? DateTime.Now.Month;
                    int currentYear = year ?? DateTime.Now.Year;

                    var employees = context.Employees
                        .Include(emp => emp.Department)
                        .Where(ep => ep.Position.Equals("Employee") &&
                                    (departmenId == null || ep.DepartmentId == departmenId))
                        .ToList();

                    foreach (var emp in employees)
                    {
                        decimal totalPayrollHours = context.Payrolls
                            .Where(p => p.EmployeeId == emp.EmployId
                                     && p.Month == currentMonth
                                     && p.Year == currentYear)
                            .Sum(p => (decimal?)p.TotalWorkHour ?? 0);

                        var matchedOTs = context.OvertimeRequests
                            .Where(o => o.EmployeeId == emp.EmployId
                                     && o.OvertimeDate.Month == currentMonth
                                     && o.OvertimeDate.Year == currentYear
                                     && o.Status != null
                                     && (o.Status.ToLower() == "presented"))
                            .ToList();

                        decimal totalOTHours = matchedOTs.Sum(o => o.TotalHours ?? 0);

                        System.Diagnostics.Debug.WriteLine($"Employee {emp.EmployId} - Payroll: {totalPayrollHours}h, OT: {totalOTHours}h");

                        decimal totalStandardMinutesPerMonth = 24 * 8 * 60;
                        decimal salaryPerMinute = emp.Salary / totalStandardMinutesPerMonth;

                        decimal totalPayrollMinutes = totalPayrollHours * 60;
                        decimal totalOTMinutes = totalOTHours * 60;

                        decimal totalSalary = Math.Round(
                            (totalPayrollMinutes * salaryPerMinute) +
                            (totalOTMinutes * salaryPerMinute * 2),
                            2, MidpointRounding.AwayFromZero);


                        decimal totalTimeWorked = totalPayrollHours + totalOTHours;

                        listEmployees.Add(new EmployeeDTO
                        {
                            EmployId = emp.EmployId,
                            EmployeeName = emp.EmployeeName,
                            Dob = emp.Dob,
                            Email = emp.Email,
                            PhoneNumber = emp.PhoneNumber,
                            Gender = emp.Gender,
                            Address = emp.Address,
                            Salary = emp.Salary,
                            Position = emp.Position,
                            DepartmentId = emp.DepartmentId,
                            DepartmentName = emp.Department?.DepartmentName,
                            PayrollTime = (double)totalPayrollHours,
                            OvertimeTime = (double)totalOTHours,
                            TotalTimeWorked = (double)totalTimeWorked,
                            TotalSalary = totalSalary
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error in GetEmployeesTotalTimeByMonth: " + e.Message);
            }

            return listEmployees;
        }



        public static List<AttendanceDTO> getAttendanceById(int emp, int? month, int? year)
        {
            using var _db = new FunattendanceAndPayrollSystemContext();
            try
            {
                var query = _db.Attendances
                    .Include(at => at.Employee)
                        .ThenInclude(emp => emp.Department)
                    .Where(at => at.EmployeeId == emp);

                if (month.HasValue && year.HasValue)
                {
                    query = query.Where(at =>
                        at.WorkDate.Month == month.Value &&
                        at.WorkDate.Year == year.Value);
                }

                var attendanceEmp = query
                    .Select(at => new AttendanceDTO
                    {
                        EmployeeId = at.EmployeeId,
                        EmployeeName = at.Employee.EmployeeName,
                        DepartmentName = at.Employee.Department.DepartmentName,
                        WorkDate = at.WorkDate,
                        CheckIn = at.CheckIn,
                        CheckOut = at.CheckOut
                    }).ToList();

                return attendanceEmp;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        // Register
        public static bool Register(RegisterDTO registerDTO)
        {
            using var _MyDb = new FunattendanceAndPayrollSystemContext();
            try
            {
                // Check if email already exists
                var existingUser = _MyDb.Employees.FirstOrDefault(e => e.Email == registerDTO.Email);
                if (existingUser != null) return false;

                // Create new Employee
                var newEmployee = new Employee
                {
                    EmployeeName = registerDTO.EmployeeName,
                    Image = registerDTO.ImageUrl,
                    Dob = registerDTO.Dob,
                    Email = registerDTO.Email,
                    PhoneNumber = registerDTO.PhoneNumber,
                    Gender = registerDTO.Gender,
                    Address = registerDTO.Address,
                    Position = registerDTO.Position ?? "Employee",
                    Salary = registerDTO.Salary ?? 0,
                    Status = registerDTO.Status,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null,
                    DeletedAt = null,
                    DepartmentId = registerDTO.DepartmentId,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password) 
                };

                _MyDb.Employees.Add(newEmployee);
                _MyDb.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Login
        public static Employee? Login(LoginDTO loginDTO)
        {
            using var _MyDb = new FunattendanceAndPayrollSystemContext();
            try
            {
                var employee = _MyDb.Employees.FirstOrDefault(e => e.Email == loginDTO.Email);
                if (employee == null) return null;

                //check password
                bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, employee.Password);

                return isValidPassword ? employee : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static EmployeeProfileDTO getInformationProfile(int empId)
        {
            using var context = new FunattendanceAndPayrollSystemContext();

            var employee = context.Employees
                .Where(e => e.EmployId == empId)
                .Select(e => new EmployeeProfileDTO
                {
                    EmployId = e.EmployId,
                    EmployeeName = e.EmployeeName,
                    Image = e.Image,
                    Dob = DateOnly.FromDateTime(e.Dob.Value),
                    Email = e.Email,
                    PhoneNumber = e.PhoneNumber,
                    Gender = e.Gender,
                    Address = e.Address,
                    Position = e.Position,

                    Skills = e.EmployeeSkills.Select(s => new SkillDTO
                    {
                        SkillId = s.Id,
                        SkillName = s.SkillName,
                        Level = s.Level
                    }).ToList(),

                    Certificates = e.EmployeeCertificates.Select(c => new CertificateDTO
                    {
                        Id = c.Id,
                        CertificateName = c.CertificateName,
                        IssueDate = c.IssueDate,
                        ExpiryDate = c.ExpiryDate
                    }).ToList()
                })
                .FirstOrDefault();

            return employee;
        }

        public static bool UpdateBasicInfo(Employee updated)
        {
            using var db = new FunattendanceAndPayrollSystemContext();
            var emp = db.Employees.FirstOrDefault(e => e.EmployId == updated.EmployId);
            if (emp == null) return false;

            emp.EmployeeName = updated.EmployeeName;
            emp.Dob = updated.Dob;
            emp.Gender = updated.Gender;
            emp.PhoneNumber = updated.PhoneNumber;
            emp.Address = updated.Address;

            db.SaveChanges();
            return true;
        }

        public static bool AddSkill(SkillDTO skillDto)
        {
            try
            {
                using var db = new FunattendanceAndPayrollSystemContext();

                var employee = db.Employees.FirstOrDefault(e => e.EmployId == skillDto.EmployId);
                if (employee == null)
                    return false; 

                var newSkill = new EmployeeSkill
                {
                    EmployeeId = skillDto.EmployId,
                    SkillName = skillDto.SkillName,
                    Level = skillDto.Level,
                };

                db.EmployeeSkills.Add(newSkill);
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error in AddSkill: " + e.Message);
            }
        }

        public static bool AddCertificate(CertificateDTO certificateDto)
        {
            try
            {
                using var db = new FunattendanceAndPayrollSystemContext();

                var employee = db.Employees.FirstOrDefault(e => e.EmployId == certificateDto.EmployeeId);
                if (employee == null) return false;

                // Thêm chứng chỉ trước
                var newCert = new EmployeeCertificate
                {
                    EmployeeId = certificateDto.EmployeeId,
                    CertificateName = certificateDto.CertificateName,
                    IssueDate = certificateDto.IssueDate,
                    ExpiryDate = certificateDto.ExpiryDate,
                    CertificateBonusRateId = certificateDto.CertificateBonusRateId,
                    ApprovedBy = certificateDto.ApprovedBy,
                    Status = certificateDto.Status
                };

                db.EmployeeCertificates.Add(newCert);
                db.SaveChanges(); // newCert.Id được sinh ra ở đây

                // Gắn ảnh nếu có
                if (certificateDto.ImageUrls != null && certificateDto.ImageUrls.Any())
                {
                    var images = certificateDto.ImageUrls.Select(url => new EmployeeCertificateImage
                    {
                        ImageUrl = url,
                        EmployeeCertificateId = newCert.Id // ⚠️ Dùng FK chứ KHÔNG gán navigation property!
                    }).ToList();

                    db.EmployeeCertificateImages.AddRange(images);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error in AddCertificate: " + e.Message, e);
            }
        }


        public static bool AddCertificateBonus(CertificateBonusDTO certificateBonusDTO)
        {
            try
            {
                using var db = new FunattendanceAndPayrollSystemContext();

                var cirtificateBonus = new CertificateBonusRate
                {
                    BonusAmount = certificateBonusDTO.BonusAmount,
                    CertificateName = certificateBonusDTO.CertificateName,
                };

                db.CertificateBonusRates.Add(cirtificateBonus);
                db.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error in AddCertificate: " + e.Message);
            }
        }


        public static bool UpdateSkills(int employeeId, List<EmployeeSkill> newSkills)
        {
            using var db = new FunattendanceAndPayrollSystemContext();
            var emp = db.Employees.Include(e => e.EmployeeSkills).FirstOrDefault(e => e.EmployId == employeeId);
            if (emp == null) return false;

            emp.EmployeeSkills.Clear();
            foreach (var skill in newSkills)
            {
                emp.EmployeeSkills.Add(skill); 
            }

            db.SaveChanges();
            return true;
        }

        public static bool UpdateCertificates(int employeeId, List<EmployeeCertificate> newCertificates)
        {
            using var db = new FunattendanceAndPayrollSystemContext();
            var emp = db.Employees.Include(e => e.EmployeeCertificates).FirstOrDefault(e => e.EmployId == employeeId);
            if (emp == null) return false;

            emp.EmployeeCertificates.Clear();
            foreach (var cert in newCertificates)
            {
                emp.EmployeeCertificates.Add(cert);
            }


            db.SaveChanges();
            return true;
        }

        public static BookingOTDTO bookScheduleOverTime(int emp, DateOnly otDate, TimeOnly startTime, TimeOnly endTime, string reason)
        {
            using var _db = new FunattendanceAndPayrollSystemContext();
            try
            {
                var allowedStart = new TimeOnly(18, 0);
                var allowedEnd = new TimeOnly(22, 0);

                if (startTime < allowedStart || endTime > allowedEnd || startTime >= endTime)
                {
                    throw new Exception("Thời gian OT chỉ được đăng ký trong khoảng từ 18:00 đến 22:00.");
                }

                var existingOT = _db.OvertimeRequests
                                    .FirstOrDefault(ot => ot.OvertimeDate == otDate
                                                       && ot.EmployeeId == emp);

                if (existingOT != null)
                {
                    if (existingOT.Status?.ToLower() == "rejected")
                    {
                        existingOT.StartTime = startTime;
                        existingOT.EndTime = endTime;
                        existingOT.Reason = reason;
                        existingOT.Status = "processing";
                        existingOT.UpdatedAt = DateTime.Now;

                        _db.SaveChanges();

                        return new BookingOTDTO
                        {
                            EmployeeId = existingOT.EmployeeId,
                            OvertimeDate = existingOT.OvertimeDate,
                            StartTime = existingOT.StartTime,
                            EndTime = existingOT.EndTime,
                            Status = existingOT.Status,
                            Reason = existingOT.Reason
                        };
                    }
                    else
                    {
                        throw new Exception("Bạn đã đăng ký OT trong ngày này rồi.");
                    }
                }

                var newRequest = new OvertimeRequest
                {
                    EmployeeId = emp,
                    OvertimeDate = otDate,
                    StartTime = startTime,
                    EndTime = endTime,
                    Reason = reason,
                    Status = "processing",
                    CreatedAt = DateTime.Now
                };

                _db.OvertimeRequests.Add(newRequest);
                _db.SaveChanges();

                return new BookingOTDTO
                {
                    EmployeeId = newRequest.EmployeeId,
                    OvertimeDate = newRequest.OvertimeDate,
                    StartTime = newRequest.StartTime,
                    EndTime = newRequest.EndTime,
                    Status = newRequest.Status,
                    Reason = newRequest.Reason
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public static bool UpdateEmployee(EmployeeUpdateDTO updatedEmployee)
        {
            using var db = new FunattendanceAndPayrollSystemContext();
            var existingEmployee = db.Employees.FirstOrDefault(e => e.EmployId == updatedEmployee.EmployId);

            if (existingEmployee == null) return false;

            existingEmployee.EmployeeName = updatedEmployee.EmployeeName;
            existingEmployee.Dob = updatedEmployee.Dob;
            existingEmployee.Email = updatedEmployee.Email;
            existingEmployee.PhoneNumber = updatedEmployee.PhoneNumber;
            existingEmployee.Gender = updatedEmployee.Gender;
            existingEmployee.Address = updatedEmployee.Address;
            existingEmployee.Position = updatedEmployee.Position;
            existingEmployee.Salary = updatedEmployee.Salary;
            existingEmployee.Status = updatedEmployee.Status;
            existingEmployee.DepartmentId = updatedEmployee.DepartmentId;
            existingEmployee.Image = updatedEmployee.Image;
            existingEmployee.UpdatedAt = DateTime.Now;

            db.SaveChanges();
            return true;
        }
        public static bool DeleteEmployee(int id)
        {
            using var db = new FunattendanceAndPayrollSystemContext();
            using var transaction = db.Database.BeginTransaction();

            try
            {
                var skills = db.EmployeeSkills.Where(s => s.EmployeeId == id).ToList();
                db.EmployeeSkills.RemoveRange(skills);

                var certificates = db.EmployeeCertificates.Where(c => c.EmployeeId == id).ToList();
                db.EmployeeCertificates.RemoveRange(certificates);

                var overtimeRequests = db.OvertimeRequests.Where(o => o.EmployeeId == id).ToList();
                db.OvertimeRequests.RemoveRange(overtimeRequests);

                var attendances = db.Attendances.Where(a => a.EmployeeId == id).ToList();
                db.Attendances.RemoveRange(attendances);

                var payrolls = db.Payrolls.Where(p => p.EmployeeId == id).ToList();
                db.Payrolls.RemoveRange(payrolls);

                var leaves = db.Leaves.Where(l => l.EmployeeId == id).ToList();
                db.Leaves.RemoveRange(leaves);

                var schedules = db.Schedules.Where(s => s.EmployeeId == id).ToList();
                db.Schedules.RemoveRange(schedules);

                var employee = db.Employees.FirstOrDefault(e => e.EmployId == id);
                if (employee == null) return false;

                db.Employees.Remove(employee);
                db.SaveChanges();

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"Error deleting employee: {ex.Message}");
                return false;
            }
        }


    }
}
