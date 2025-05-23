﻿namespace Hospital.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a room within a department, with associated equipment and capacity.
    /// </summary>
    public class RoomModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoomModel"/> class with parameters.
        /// </summary>
        /// <param name="roomID">The unique identifier of the room.</param>
        /// <param name="capacity">The number of people the room can accommodate.</param>
        /// <param name="departmentID">The ID of the department the room belongs to.</param>
        /// <param name="equipmentID">The ID of the equipment assigned to the room.</param>
        public RoomModel(int roomID, int capacity, int departmentID, int equipmentID)
        {
            this.RoomID = roomID;
            this.Capacity = capacity;
            this.DepartmentID = departmentID;
            this.EquipmentID = equipmentID;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoomModel"/> class.
        /// </summary>
        public RoomModel()
        {
        }

        /// <summary>
        /// Gets or sets the unique identifier of the room.
        /// </summary>
        public int RoomID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the room.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Gets or sets the number of people the room can accommodate.
        /// </summary>
        public int DepartmentID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the department the room belongs to.
        /// </summary>
        public int EquipmentID { get; set; }

        public virtual EquipmentModel Equipment { get; set; }
        public virtual DepartmentModel Department { get; set; }
    }
}