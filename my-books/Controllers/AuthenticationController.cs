using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Microsoft.IdentityModel.Tokens;
using my_books.Data;
using my_books.Data.Model;
using my_books.Data.ViewModel.Authentication;
using Newtonsoft.Json.Converters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace my_books.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        //Refresh Tokens
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            AppDbContext context, 
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterVM payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide valid all required credentials");
            }
            // Implementation for user registration
            var userExists = await _userManager.FindByEmailAsync(payload.Email);
            if(userExists!=null)
            {
                return BadRequest($"User {payload.Email} already exists");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                Email = payload.Email,
                UserName = payload.UserName,
                SecurityStamp = Guid.NewGuid().ToString(),
                Custom = "Default"
            };
            var result = await _userManager.CreateAsync(newUser, payload.Password);

            if (!result.Succeeded)
            {
                return BadRequest("User could not be created!");
            }

            return Created(nameof(RegisterUser), $"User {payload.Email} created");
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVM payload)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Please provide valid all required credentials");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if(user != null && await _userManager.CheckPasswordAsync(user, payload.Password))
            {
                var authResult = await GenerateJwtTokenAsync(user, "");
                return Ok(authResult);
            }
            return Unauthorized("Invalid credentials");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM payload)
        {
            try
            {
                var result = await VerifyAndGenerateTokenAsync(payload);
                if (result == null)
                {
                    return BadRequest("Invalid Token");
                }
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<AuthResultVM> VerifyAndGenerateTokenAsync(TokenRequestVM payload)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                // Check 1 Jwt token format
                var tokenVerification = jwtTokenHandler.ValidateToken(payload.Token, _tokenValidationParameters, out var validatedToken);

                // Check 2 Encryption Algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false) return null;
                }

                // Check 3 Validate expiry date
                var utcExpiryDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expiryDate = UnixTimeStampToDateTimeInUTC(utcExpiryDate);
                if (expiryDate > DateTime.UtcNow)
                {
                    throw new Exception("Token has not expired yet!");
                }

                // Check 4 Refresh token exixts in DB
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);
                if (dbRefreshToken == null)
                {
                    throw new Exception("Refresh token does not exist in DB");
                }
                else
                {
                    // Check 5 Validate Id
                    var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (dbRefreshToken.JwtId != jti)
                    {
                        throw new Exception("Refresh token does not match JWT");
                    }

                    // Check 6 Refresh token is expired 
                    if (dbRefreshToken.DateExpire <= DateTime.UtcNow)
                    {
                        throw new Exception("Refresh token has expired, please re-authenticate");
                    }

                    // Check 7 Refresh token is revoked
                    if (dbRefreshToken.IsRevoked)
                    {
                        throw new Exception("Refresh token has been revoked");
                    }

                    //Generate new token (with existing refresh token)
                    var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                    var newTokenRsponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                    return await newTokenRsponse;
                }
            }
            catch(SecurityTokenExpiredException)
            {
                var dbRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(n => n.Token == payload.RefreshToken);
                //Generate new token (with existing refresh token)
                var dbUserData = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                var newTokenRsponse = GenerateJwtTokenAsync(dbUserData, payload.RefreshToken);

                return await newTokenRsponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DateTime UnixTimeStampToDateTimeInUTC(long utcExpiryDate)
        {
            var datatTimeValue = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            datatTimeValue = datatTimeValue.AddSeconds(utcExpiryDate);
            return datatTimeValue;
        }

        private async Task<AuthResultVM> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),

                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, "http://localhost:44346/"),
            };

            var authSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));
             
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddMinutes(1), // generally 5-10 mins
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var JwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = new RefreshToken();
            if (string.IsNullOrEmpty(existingRefreshToken))
            {
                refreshToken = new RefreshToken()
                {
                    JwtId = token.Id,
                    IsRevoked = false,
                    UserId = user.Id,
                    DateAdded = DateTime.UtcNow,
                    DateExpire = DateTime.UtcNow.AddMonths(6),
                    Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString() // random string
                };

                await _context.RefreshTokens.AddAsync(refreshToken);
                await _context.SaveChangesAsync();
            }
            

            return new AuthResultVM()
            {
                Token = JwtToken,
                RefreshToken = (string.IsNullOrEmpty(existingRefreshToken) ? refreshToken.Token : existingRefreshToken),
                ExpiresAt = token.ValidTo
            };
        }
    }
}
