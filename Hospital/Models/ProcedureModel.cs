using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class ProcedureModel
    {
        public int ProcedureId { get; set; }

        public string ProcedureName { get; set; }

        // Duration in minutes
        public int DepartmentId { get; set; }
        
        public TimeSpan ProcedureDuration { get; set; }

        public ProcedureModel(int procedureId, int departmentId, string name, TimeSpan duration)
        {
            this.ProcedureId = procedureId;
            this.DepartmentId = departmentId;
            this.ProcedureName = name;
            this.ProcedureDuration = duration;
        }
    }
}
