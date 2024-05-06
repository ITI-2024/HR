using HR.Models;

namespace HR.Repository
{
    public interface IRoleNameRepository
    {
        Task<RoleName> GetRoleNameById(int? roleId);
        Task<RoleName> GetRoleNameByName(string roleName);
        Task<List<RoleName>> GetAllRoles();
        Task RoleNameUpdate(RoleName role);
        Task<RoleName> RoleNameCreate(RoleName roleName);
        Task RoleNameDelete(RoleName roleName);
    
    }
}
