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
        private readonly PhotoService _photoService;

        public AuthController(
            IConfiguration configuration,
            IEmployeeRepository repository,
            PhotoService photoService)
        {
            _tokenGenerator = new JwtTokenGenerator(configuration);
            _repository = repository;
            _photoService = photoService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO)
        {
            
            if (registerDTO.Image != null && registerDTO.Image.Length > 0)
            {
                var photoUrl = await _photoService.UploadPhotoAsync(registerDTO.Image);
                registerDTO.ImageUrl = photoUrl;
            }

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
                Role = user.Position,
                EmployeeId = user.EmployId,
            });
        }
    }
}
