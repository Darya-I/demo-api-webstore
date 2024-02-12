using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiDemo_ML_lesson.Configurations;
using WebApiDemo_ML_lesson.Models;
using WebApiDemo_ML_lesson.Models.DTOs;


namespace WebApiDemo_ML_lesson.Controllers
{
    [Route("api/[controller]")]                             // api/authentication
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        // private readonly JwtConfig _jwtConfig;

        public AuthenticationController(                    //конструктор
            UserManager<IdentityUser> userManager,
            IConfiguration configuration
            ) //,JwtConfig jwtConfig
        { 
            //_jwtConfig = jwtConfig;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO requestDTO) 
        {
            // проверить входящий запрос validate the incoming request
            if(ModelState.IsValid) 
            {
                // we need to check if the email already exist
                var user_exist = await _userManager.FindByEmailAsync(requestDTO.Email);
                if (user_exist != null) 
                {
                    return BadRequest(new AuthResult()
                    {
                        Result = false,
                        Errors = new List<string>()
                        {
                            "Почта уже существует, email already exist"
                        }
                    });

                }

                // create a user
                var new_user = new IdentityUser() 
                {
                    Email = requestDTO.Email,
                    UserName = requestDTO.Email
                };
                var is_created = await _userManager.CreateAsync(new_user, requestDTO.Password);

                if (is_created.Succeeded)
                {
                    //generate token
                    var token = GenerateJwtToken(new_user);
                    return Ok(new AuthResult()
                    {
                        Result = true,
                        Token = token
                    });
                }

                return BadRequest(new AuthResult()
                {
                    Errors = new List<string>()
                    {
                        "ошибка сервера, server error"
                    },
                    Result = false
                });
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            if (ModelState.IsValid)
            {
                // check if the user exist, проверьте, существует ли пользователь
                var existing_user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (existing_user == null) 
                { 
                    return BadRequest(new AuthResult
                    {
                        Errors = new List<string>()
                        {
                            "недопустимые данные, invalid payload"
                        },
                        Result = false   
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existing_user, loginRequest.Password);
                if (!isCorrect)
                {
                    return BadRequest(new AuthResult 
                    { 
                        Errors = new List<string>() 
                        { 
                            "Invalid credentials, неверные данные"
                        },
                        Result = false
                    });
                }

                var jwtToken = GenerateJwtToken(existing_user);

                return Ok(new AuthResult
                {
                    Token = jwtToken,
                    Result = true
                });
            }

            return BadRequest(new AuthResult
            {
                Errors = new List<string>
                {
                    "недопустимые данные, invalid payload"
                },
                Result = false
            });
        }
        

        private string GenerateJwtToken(IdentityUser user) 
        { 
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            
            
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Secret").Value); //_jwtConfig.Secret

            // token description, описание токена

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, value:user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
                }),
                
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
           
        }

    }
}
