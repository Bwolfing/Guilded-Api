using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Guilded.Data.Models.Core
{
    [Table("AspNetPrivileges")]
    public class ResourcePrivilege
    {
        #region Properties
        #region Database columns
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        #endregion

        #region Navigation properties
        [InverseProperty("Privilege")]
        public virtual ICollection<RolePrivilege> RolePrivileges { get; set; }

        [NotMapped]
        public IEnumerable<ApplicationRole> Roles
        {
            get
            {
                return RolePrivileges.Select(r => r.Role);
            }
        }
        #endregion
        #endregion
    }
}
