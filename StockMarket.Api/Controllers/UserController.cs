using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StockMarket.Identity;

namespace StockMarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtBearerTokenSettings jwtBearerTokenSettings;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, IOptions<JwtBearerTokenSettings> jwtTokenOptions, ILogger<UserController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this.jwtBearerTokenSettings = jwtTokenOptions.Value;
        }

        /// <summary>
        /// Register a user
        /// </summary>
        /// <param name="UserDetails"></param>  
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserDetails userDetails)
        {
            if (userDetails == null) 
                return BadRequest("Invalid userDetails");

            var identityUser = new AppUser()
            {
                UserName = userDetails.Username,
                Email = userDetails.Email
            };
            var result = await userManager.CreateAsync(identityUser, userDetails.Password);
            if (!result.Succeeded)
            {
                string errorMessage = result.Errors.ToString();
                return BadRequest("User Reigstration failed: " + errorMessage);
            }

            return Ok("User Reigstration Successful");
        }

        /// <summary>
        /// Login with username and password
        /// </summary>
        /// <param name="LoginCredentials"></param>  
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginCredentials credentials)
        {
            if (credentials == null) 
                return BadRequest("Invalid user credentials");

            var identityUser = await userManager.FindByNameAsync(credentials.Username);
            if (identityUser == null) 
                return BadRequest($"User {credentials.Username} not found");

            var result = userManager.PasswordHasher.VerifyHashedPassword(identityUser, identityUser.PasswordHash, credentials.Password);
            if (result == PasswordVerificationResult.Failed) 
                return BadRequest("Login failed");

            var token = GenerateJwtToken(identityUser);
            logger.LogInformation($"==> User {credentials.Username} logged in on {DateTime.Now}");
            return Ok(new { Token = token, Message = "Login Success" });
        }

        private object GenerateJwtToken(IdentityUser identityUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtBearerTokenSettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, identityUser.UserName.ToString()),
                    new Claim(ClaimTypes.Email, identityUser.Email)
                }),
                Expires = DateTime.UtcNow.AddSeconds(jwtBearerTokenSettings.ExpiryTimeInSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = jwtBearerTokenSettings.Audience,
                Issuer = jwtBearerTokenSettings.Issuer
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Logout la
        /// </summary>
        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok();
        }
    }
}
