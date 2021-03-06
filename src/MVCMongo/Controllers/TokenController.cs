﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JWTIssuer;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Principal;
using System.IdentityModel.Tokens.Jwt;
using MVCMongo.Core.Abstraction;
using MVCMongo.Core.ViewModel;

namespace MVCMongo.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly TokenOptions _jwtOptions;
       // private readonly ILogger _logger;
        private readonly JsonSerializerSettings _serializerSettings;

        private readonly IUserService _userService;
        public TokenController(IUserService userService, IOptions<TokenOptions> jwtOptions)
        {
            _userService = userService;
            _jwtOptions = jwtOptions.Value;
             ValidateTokenOptions(_jwtOptions);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Get([FromBody] UserViewModel applicationUser)
        {
            var userInfo = _userService.GetUserByName(applicationUser.UserName);
            if(userInfo.Password != applicationUser.Password)
            {
                return BadRequest("Invalid credentials");
            }

            var identity = await GetClaimsIdentity(applicationUser);
            if (identity == null)
            {
                return BadRequest("Invalid credentials");
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(),
                          ClaimValueTypes.Integer64),
                identity.FindFirst("SuperAdmin")
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            };

            var json = JsonConvert.SerializeObject(response, _serializerSettings);
            return new OkObjectResult(json);
        }

        private static void ValidateTokenOptions(TokenOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenOptions.JtiGenerator));
            }
        }

        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);

        /// <summary>
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private static Task<ClaimsIdentity> GetClaimsIdentity(UserViewModel user)
        {
            // Return different Claims based on user role
            if(user != null)
            {
                return Task.FromResult(new ClaimsIdentity(
                  new GenericIdentity(user.UserName, "Token"),
                  new Claim[] {
                      new Claim("SuperAdmin", "IAmSuperAdmin")
                  }));
            }

            // Credentials are invalid, or account doesn't exist
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
