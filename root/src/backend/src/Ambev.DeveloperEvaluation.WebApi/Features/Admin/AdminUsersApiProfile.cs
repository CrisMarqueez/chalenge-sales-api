using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Admin;

public sealed class AdminUsersApiProfile : Profile
{
    public AdminUsersApiProfile()
    {
        CreateMap<UpdateAdminUserRequest, UpdateUserCommand>();
        CreateMap<GetUserResult, AdminUserResponse>();
    }
}
