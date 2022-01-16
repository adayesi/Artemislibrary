﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Models;
using DTOs;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookLibraryProject.Controllers
{
    public class AccountController : BaseApiController
    {
      
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController (UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken, do choose another one");

            var user = _mapper.Map<AppUser>(registerDto);


            user.UserName = registerDto.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded) return BadRequest(result.Errors);




            return new UserDto
            {

                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<bool>> Login(LoginDto loginDto)
        {
            //var res = new ResponseDto<UserDto>();

            var user = await _userManager.FindByNameAsync(loginDto.Username);

            //user = await _userManager.Users
            //    .Include(p => p.Photos)
            //    .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            UserDto userDetails = new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                //PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user.Gender
            };

            //res.Status = true;
            //res.Message = "Login Successfully!";
            //res.Data = userDetails;

            return Ok(userDetails);


            
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
    
}
