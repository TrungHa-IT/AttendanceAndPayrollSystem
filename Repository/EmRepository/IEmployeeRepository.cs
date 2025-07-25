﻿using BusinessObject.Models;
using DataTransferObject.AttendanceDTO;
using DataTransferObject.AuthDTO;
using DataTransferObject.EmployeeDTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.EmRepository
{
    public interface IEmployeeRepository
    {
        List<EmployeeDTO> GetEmployees();
        List<EmployeeDTO> GetStaffs();
        bool UpdateEmployee(EmployeeUpdateDTO updatedEmployee);
        bool DeleteEmployee(int id);
        List<AttendanceDTO> getAttendanceById(int emp, int? month, int? year);
        BookingOTDTO bookScheduleOverTime(int emp, DateOnly otDate, TimeOnly startTime, TimeOnly endTime, string reason);
        bool Register(RegisterDTO registerDTO);
        Employee? Login(LoginDTO loginDTO);
        List<EmployeeDTO> GetEmployeesTotalTimeByMonth(int? month, int? year, int? departmenId);
        EmployeeProfileDTO getInformationProfile(int empId);

        bool UpdateBasicInfo(Employee updated);
        bool UpdateSkills(int employeeId, List<EmployeeSkill> newSkills);
        bool UpdateCertificates(int employeeId, List<EmployeeCertificate> newCertificates);
        bool AddSkill(SkillDTO skillDto);
    }


}
