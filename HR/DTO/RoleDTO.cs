namespace HR.DTO
{
    public class RoleDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<PermissionDTO>? Permissions { get; set; }
    }
}
