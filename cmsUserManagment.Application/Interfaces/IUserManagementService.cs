using cms.Domain.Entities;

using cmsUserManagment.Application.DTO;

namespace cmsUserManagment.Application.Interfaces;

public interface IUserManagementService
{
    Task<IEnumerable<User>> GetAllUsers();
    Task<User?> GetUserById(Guid id);
    Task<bool> UpdateUser(Guid id, UpdateUserDto user);
    Task<bool> DeleteUser(Guid id);
    Task<bool> DeleteBulkUsers(IEnumerable<Guid> ids);

    public Task<IEnumerable<User>> SearchUsers(
        string? username,
        string? email,
        bool? isAdmin,
        string? orderBy = "username",
        bool descending = false);
}
