using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Models
{
    public class permission
    {
        public int id { get; set; }
        public string name { get; set; } //sectionname
        public bool? view { get; set; }
        public bool? create { get; set; }
        public bool? update { get; set; }
        public bool? delete { get; set; }

        [ForeignKey("roleName")]
        public int? roleid {  get; set; }

        [JsonIgnore]
        public virtual RoleName? roleName { get; set; }

    }
}
