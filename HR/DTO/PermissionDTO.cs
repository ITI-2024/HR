namespace HR.DTO
{
    public class PermissionDTO
    {
        public string name { get; set; }
        public bool? Create { get; set; }
        public bool? Delete { get; set; }
        public bool? View { get; set; }
        public bool? Update { get; set; }

    }
}
