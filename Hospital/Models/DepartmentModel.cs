using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class DepartmentModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }

        public DepartmentModel() { }

        public DepartmentModel(int departmentId, string departmentName)
        {
            this.DepartmentId = departmentId;
            this.Name = departmentName;
        }
    }
}
