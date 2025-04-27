// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationConfiguration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Hospital.Configs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplicationConfiguration
    {
        private static readonly object _lock = new object();

        private static ApplicationConfiguration? _instance;

        private string _databaseConnection = "Data Source=DESKTOP-H700VKM\\MSSQLSERVER02;Initial Catalog=HospitalDB;Integrated Security=True;TrustServerCertificate=True";

        public int patientId = 1;
        public int doctorId = 1;
        public int SlotDuration = 30;

        private ApplicationConfiguration()
        {
        }

        public static ApplicationConfiguration GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new ApplicationConfiguration();
                    }
                }
            }

            return _instance;
        }

        public string DatabaseConnection
        {
            get
            {
                return this._databaseConnection;
            }
        }
    }
}
