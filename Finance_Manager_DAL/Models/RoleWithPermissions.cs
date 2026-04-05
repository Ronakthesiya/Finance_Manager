using Finance_Manager_MAL.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance_Manager_DAL.Models
{
    public class RoleWithPermissions
    {
        public Role Role { get; set; }
        public List<string> Permissions { get; set; }
    }
}
