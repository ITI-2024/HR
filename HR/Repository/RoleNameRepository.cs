using HR.Models;
using Microsoft.EntityFrameworkCore;

namespace HR.Repository
{
    public class RoleNameRepository : IRoleNameRepository
    {
        private readonly HRDbcontext _db;
        public RoleNameRepository(HRDbcontext db)
        {
            _db = db;
        }
        public async Task<RoleName> GetRoleNameById(int? roleId)
        {
            return await _db.Roles.Include(r=>r.Permissions).FirstOrDefaultAsync(r => r.Id == roleId);
        }
       public virtual async Task<List<RoleName>> GetAllRoles()
        {
            return _db.Roles.Include(r=>r.Permissions).ToList();
        }

        public virtual async Task<RoleName> RoleNameCreate(RoleName roleName)
        {
            _db.Roles.Add(roleName);
            _db.SaveChanges();
            return roleName;
        }
        public virtual async Task RoleNameUpdate(RoleName role)
        {
            _db.Roles.Update(role); 
            _db.SaveChanges();
        }
        public virtual async Task RoleNameDelete(RoleName roleName)
        {
            _db.Roles.Remove(roleName); 
            _db.SaveChanges();
        }
        public virtual async Task<RoleName> GetRoleNameByName(string roleName)
        {
            return await _db.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.GroupName == roleName);

        }
    }
}
