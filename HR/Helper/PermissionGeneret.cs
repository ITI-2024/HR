using HR.Models;

namespace HR.Helper
{
    public static class PermissionGeneret
    {
        public static List<string> GeneratePermissionsList(string module,bool?  create, bool? delete, bool? view, bool? update)
        {
            List<string> permissions = new List<string>();

            if (create==true)
                permissions.Add($"{module}.Create");
            if (view==true)
                permissions.Add($"{module}.View");
            if (update == true)
                permissions.Add($"{module}.Update");
            if (delete==true)
                permissions.Add($"{module}.Delete");

            return permissions;
        }

        
    }
}
