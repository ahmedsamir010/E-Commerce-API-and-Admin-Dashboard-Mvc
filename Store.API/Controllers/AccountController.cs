using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using Store.API.Dtos;
using Store.API.Errors;
using Store.API.Helpers;
using Store.API.Models;
using Store.Core.Entities;
using Store.Core.Entities.Identity;
using Store.Core.Services;
using Store.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Store.API.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IEmailSettings _emailSettings;
        private readonly AppSettings _applicationSettings;
        private readonly HttpClient _httpClient;
        public AccountController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      ITokenService tokenService,
      IMapper mapper,
      IConfiguration configuration,
      IEmailSettings emailSettings,
      IOptions<AppSettings> _applicationSettings, HttpClient httpClient

      )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _configuration = configuration;
            _emailSettings = emailSettings;
            this._applicationSettings = _applicationSettings.Value;
            _httpClient = httpClient;
        }


        // Login With Google
        [HttpPost("LoginWithGoogle")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] string credential)
        {
            //   Console.WriteLine($"Credential ; {credential}");
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { this._applicationSettings.GoogleClientId }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

                var existingUser = await _userManager.FindByEmailAsync(payload.Email);

                if (existingUser == null)
                {
                    // User does not exist, create a user login record
                    var userLogin = new UserLoginInfo("Google", payload.Subject, "Google");

                    var user = new AppUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName
                    };

                    var result = await _userManager.CreateAsync(user);

                    if (!result.Succeeded)
                    {
                        // Handle the failure to create user
                        return BadRequest(new ApiResponse(400, "Failed to create user."));
                    }

                    // Add the Google login to the created user
                    result = await _userManager.AddLoginAsync(user, userLogin);

                    if (!result.Succeeded)
                    {
                        // Handle the failure to add login
                        return BadRequest(new ApiResponse(400, "Failed to create user login."));
                    }

                    existingUser = user;
                }

                // Generate and return JWT token
                return Ok(new UserDto
                {
                    Token = await _tokenService.CreateTokenAsync(existingUser, _userManager),
                    Email = existingUser.UserName,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName
                });
            }
            catch (Exception ex)
            {
                // Log the error

                return BadRequest(new ApiResponse(400, "Google Authentication error."));
            }
        }


        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse(400, "Email is already taken"));
            }

            var newUser = new AppUser
            {
                FirstName = registerDto.firstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(newUser, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to create user"));
            }

            var token = await _tokenService.CreateTokenAsync(newUser, _userManager);

            var userDto = new UserDto
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Token = token
            };
            return Ok(userDto);
        }

        [HttpGet("GetUserData")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserData()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            return Ok(new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }

        [HttpGet("GetUserAddress")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);
            var addressDto = _mapper.Map<AddressDto>(user?.Address);
            return addressDto;
        }

        [HttpPut("UpdateUserAddress")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto updateAddressDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found."));
            }

            var address = user.Address;
            if (address == null)
            {
                address = new Address();
                user.Address = address;
            }

            if (updateAddressDto != null)
            {
                address.FirstName = updateAddressDto.FirstName;
                address.LastName = updateAddressDto.LastName;
                address.Street = updateAddressDto.Street;
                address.City = updateAddressDto.City;
                address.Country = updateAddressDto.Country;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var mappedUpdateAddress = _mapper.Map<AddressDto>(address);
                return Ok(mappedUpdateAddress);
            }

            return BadRequest(new ApiResponse(400, "Failed to update user address."));
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
                return BadRequest(new ApiResponse(400, "User not found"));
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var resetPasswordUrl = $"{_configuration["FrontEndUrl"]}/ResetPassword?email={user.Email}&token={encodedToken}";
            var message = GetResetPasswordEmailBody(resetPasswordUrl);  // Define the resetPasswordUrl variable
            var email = new Mail
            {
                Subject = "Reset Password",
                To = forgotPasswordDto.Email,
                Body = message
            };
            _emailSettings.SendEmail(email);

            return Ok(new ApiResponse(200, "The Reset Password link has been sent to your mail"));
        }


        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User not found."));
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse(200, "Password has been changed."));
            }

            return BadRequest(new ApiResponse(400, "Failed to change the password."));
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User not found."));
            }

            var result = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordDto.Token);
            if (!result)
            {
                return BadRequest(new ApiResponse(400, "Invalid password reset token"));
            }
            var resetRresult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
            if (resetRresult.Succeeded)
            {
                return Ok(new ApiResponse(200, "Password has been reset successfully."));
            }
            return BadRequest(new ApiResponse(400, "Failed to reset the password."));
        }
        private string GetResetPasswordEmailBody(string resetPasswordUrl)
        {
            return $@"
            <html>
            <head>
                <style>
                    /* Define styles for the body */
                    body {{
                        background-color: #f2f2f2;
                        font-family: Arial, sans-serif;
                        font-size: 16px;
                        margin: 0;
                        padding: 0;
                    }}

                    /* Define styles for the container */
                    .container {{
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
                        margin: 20px auto;
                        max-width: 600px;
                        padding: 20px;
                    }}

                    /* Define styles for the heading */
                    h1 {{
                        color: #333;
                        font-size: 24px;
                        margin-bottom: 16px;
                        text-align: center;
                    }}

                    /* Define styles for the paragraph */
                    p {{
                        color: #666;
                        margin-bottom: 16px;
                        text-align: center;
                    }}

                    /* Define styles for the button container */
                    .button-container {{
                        text-align: center;
                    }}

                    /* Define styles for the button */
                    .button {{
                        background-color: #007bff;
                        border-radius: 5px;
                        color: #fff;
                        display: inline-block;
                        font-size: 16px;
                        margin: 20px auto;
                        padding: 12px 24px;
                        text-align: center;
                        text-decoration: none;
                        transition: background-color 0.3s ease;
                    }}

                    .button:hover {{
                        background-color: #0069d9;
                        cursor: pointer;
                    }}

                    /* Define styles for the footer */
                    .footer {{
                        color: #999;
                        font-size: 14px;
                        margin-top: 40px;
                        text-align: center;
                    }}

                    .footer span {{
                        color: #666;
                        font-weight: bold;
                    }}
                </style>
            </head>

            <body>
                <div class=""container"">
                    <h1>Reset Password</h1>
                    <p>Hello,</p>
                    <p>We have sent you this email in response to your request to reset your password on our website.</p>
                    <p>To reset your password, please click the button below:</p>
                    <div class=""button-container"">
                        <a href=""{resetPasswordUrl}"" class=""button"">Reset Password</a>
                    </div>
                    <p>Please ignore this email if you did not request a password change.</p>
                    <p class=""footer"">
                        This email was sent to you by <span>Market Abo Samra</span>.<br>
                        If you have any questions, please contact our support team.
                    </p>
                </div>
            </body>

            </html>";
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse(401, "The email address is incorrect."));
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!passwordValid)
            {
                return Unauthorized(new ApiResponse(401, "The password is incorrect."));
            }
            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };

            await _tokenService.CreateTokenAsync(user, _userManager);
            return Ok(userDto);
        }


    






















    }
}
