using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AngularApp1.Server.Models;
using AngularApp1.Server.Models.SetModels;
using AngularApp1.Server.Models.ViewModels;
using AngularApp1.Server.Repositories;

namespace AngularApp1.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<object> _passwordHasher;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<object>();
        }

        public async Task<IEnumerable<GetUser>> GetUsersAsync()
        {
            return await _repository.GetAllUsersAsync();
        }

        public async Task<GetUser?> GetUserByIdAsync(long id)
        {
            return await _repository.GetUserByIdAsync(id);
        }

        public async Task<Result> PostUserAsync(PostUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return new Result { Success = false, Message = "El correo electrónico es obligatorio." };

            if (string.IsNullOrWhiteSpace(model.FullName))
                return new Result { Success = false, Message = "El nombre completo es obligatorio." };

            if (model.CompanyId <= 0)
                return new Result { Success = false, Message = "La empresa es obligatoria." };

            if (model.RoleId <= 0)
                return new Result { Success = false, Message = "El rol es obligatorio." };

            if (string.IsNullOrWhiteSpace(model.Password))
                return new Result { Success = false, Message = "La contraseña es obligatoria para nuevos usuarios." };

            // Check duplicate email
            var existingUser = await _repository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
                return new Result { Success = false, Message = "El correo electrónico ya está registrado." };

            var hashedPassword = _passwordHasher.HashPassword(new object(), model.Password);
            return await _repository.CreateUserAsync(model, hashedPassword);
        }

        public async Task<Result> PutUserAsync(long id, PostUser model)
        {
            if (string.IsNullOrWhiteSpace(model.Email))
                return new Result { Success = false, Message = "El correo electrónico es obligatorio." };

            if (string.IsNullOrWhiteSpace(model.FullName))
                return new Result { Success = false, Message = "El nombre completo es obligatorio." };

            if (model.CompanyId <= 0)
                return new Result { Success = false, Message = "La empresa es obligatoria." };

            if (model.RoleId <= 0)
                return new Result { Success = false, Message = "El rol es obligatorio." };

            // Check duplicate email (excluding this user)
            var existingUser = await _repository.GetUserByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != id)
                return new Result { Success = false, Message = "El correo electrónico ya está registrado por otro usuario." };

            string? hashedPassword = null;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                hashedPassword = _passwordHasher.HashPassword(new object(), model.Password);
            }

            return await _repository.UpdateUserAsync(id, model, hashedPassword);
        }

        public async Task<Result> DeleteUserAsync(long id)
        {
            return await _repository.DeleteUserAsync(id);
        }
    }
}
