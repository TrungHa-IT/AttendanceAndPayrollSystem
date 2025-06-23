using FUNAttendanceAndPayrollSystemAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using Repository.EmployeeRepository;
using DataTransferObject.AuthDTO;

namespace FUNAttendanceAndPayrollSystemAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly IEmployeeRepository _repository;

        public AuthController(IConfiguration configuration)
        {
            _tokenGenerator = new JwtTokenGenerator(configuration);
            _repository = new EmployeeRepository(); 
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO registerDTO)
        {
            var result = _repository.Register(registerDTO);
            if (!result)
                return BadRequest("Email already exists!");

            return Ok("Registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var user = _repository.Login(loginDTO);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var token = _tokenGenerator.GenerateToken(user);
            return Ok(new
            {
                Token = token,
                Name = user.EmployeeName,
                Email = user.Email,
                Role = user.Position
            });
        }
    }
}
