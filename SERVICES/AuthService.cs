using AutoMapper;
using CORE.DTOs;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using REPOSITORY.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        public AuthService(IMapper mapper,IUnitOfWork unitOfWork,ITokenService tokenService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }
        public async Task<AuthBaseResponseDto> Register(UserRegisterDto dto)
        {
            if (dto is null) return new AuthBaseResponseDto(false,"Invalid Inputs");
            if (await _unitOfWork.IUserRepository.GetUserByEmailAsync(dto.Email) is not null) return new AuthBaseResponseDto(false,"Email Already Exist");
            var MappedUser = _mapper.Map<UserRegisterDto, User>(dto);
            MappedUser.HashPassword = HashPassowrdHandler(dto.Password);
            await _unitOfWork.GenericRepo<User>().AddAsync(MappedUser);
            var Result = await _unitOfWork.SaveChangesAsync();
            if (Result <= 0) return new AuthBaseResponseDto(false, "Errors During User Registration");
            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(MappedUser);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "User Registered Successfully";
            return ReturnedResponse;
        }
        public async Task<AuthBaseResponseDto> Login(UserLoginDto dto)
        {
            if (dto is null) return new AuthBaseResponseDto(false, "Invalid Inputs");
            var User = await _unitOfWork.IUserRepository.GetUserByEmailAsync(dto.Email);
            if (User is null) return new AuthBaseResponseDto(false, "Invalid Email or Password");
            if (!VerifyHashedPassword(User.HashPassword,dto.Password)) return new AuthBaseResponseDto(false, "Invalid Email or Password");
            var accessToken = await _tokenService.CreateAccessTokenAsync(User);
            var refreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenEntity = new RefreshToken()
            {
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.Id
            };
            await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(refreshTokenEntity);
            var Result = await _unitOfWork.SaveChangesAsync();
            if(Result <= 0) return new AuthBaseResponseDto(false, "Errors During Login");
            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(User);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "Login Success";
            ReturnedResponse.AccessToken = accessToken;
            ReturnedResponse.RefreshToken = refreshToken;
            return ReturnedResponse;
        }
        public string HashPassowrdHandler(string Password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(Password,salt,10000,HashAlgorithmName.SHA512,32);
            return Convert.ToHexString(salt) + "-" + Convert.ToHexString(hash);
        }
        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var Parts = hashedPassword.Split('-');
            if (Parts.Length != 2) return false;
            byte[] salt = Convert.FromHexString(Parts[0]);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(providedPassword,salt,10000,HashAlgorithmName.SHA512,32);
            return Convert.ToHexString(hash) == Parts[1];
        }
        public async Task<AuthBaseResponseDto> RefreshToken(string token)
        {
            var User = await _unitOfWork.IUserRepository.FindByRefreshTokenAsync(token);
            if (User is null) return new AuthBaseResponseDto(false, "Invalid Token *LoginAgain*");

            var rt = await _unitOfWork.IUserRepository.GetRefreshTokenAsync(token);
            _unitOfWork.GenericRepo<RefreshToken>().Delete(rt);
            var dResult = await _unitOfWork.SaveChangesAsync();
            if (dResult <= 0) return new AuthBaseResponseDto(false, "Errors During Generating New Tokens");
            var newRefreshToken = _tokenService.CreateRefreshToken();
            var refreshTokenEntity = new RefreshToken()
            {
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                UserId = User.Id
            };
            await _unitOfWork.GenericRepo<RefreshToken>().AddAsync(refreshTokenEntity);
            var sResult = await _unitOfWork.SaveChangesAsync();
            if (sResult <= 0) return new AuthBaseResponseDto(false, "Errors During RefreshToken");

            var accessToken = await _tokenService.CreateAccessTokenAsync(User);

            var ReturnedResponse = _mapper.Map<User, AuthBaseResponseDto>(User);
            ReturnedResponse.Success = true;
            ReturnedResponse.Message = "Access Token Generated Successfully";
            ReturnedResponse.AccessToken = accessToken;
            ReturnedResponse.RefreshToken = newRefreshToken;
            return ReturnedResponse;
        }
        public async Task<bool> LogOut(string token)
        {
            //var refreshtoken = await _unitOfWork.IUserRepository.GetRefreshTokenAsync(token);
            //if (refreshtoken is null) return false;
            //_unitOfWork.GenericRepo<RefreshToken>().Delete(refreshtoken);
            //return await _unitOfWork.SaveChangesAsync() > 0;

            var rt = await _unitOfWork.IUserRepository.GetRefreshTokenAsync(token);
            if (rt is null) return false;
            var uId = rt.UserId;
            var deleted = _unitOfWork.IUserRepository.DeleteAllRefreshTokensForUser(uId);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
