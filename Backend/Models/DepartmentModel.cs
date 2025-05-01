using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class DepartmentModel
    {
        public int DepartmentID { get; set; }
        public string Name { get; set; }

        public DepartmentModel() { }

        public DepartmentModel(int departmentId, string departmentName)
        {
            this.DepartmentID = departmentId;
            this.Name = departmentName;
        }
    }
}
