﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationConfiguration class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Backend.Configs
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


        private string _databaseConnection = "Data Source=DESKTOP-CL1KD74\\SQLEXPRESS01;Initial Catalog=HospitalManagement;Integrated Security=True;TrustServerCertificate=True";

        public int patientId = 1;
        public int doctorId = 1;
        public int SlotDuration = 30;

        private string jwtSecret = "axbc32-fdashuf-dfashjifduijlsa-sdajhfgasdhjg-fdaujihgfdj";
        private int tokenExpirationMinutes = 60;
        private string issuer = "appointmentmanager";
        private string audience = "account";

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

        public string JwtSecret
        {
            get
            {
                return this.jwtSecret;
            }
        }

        public int TokenExpirationMinutes
        {
            get
            {
                return this.tokenExpirationMinutes;
            }
        }

        public string Issuer { get => this.issuer;}
        public string Audience { get => this.audience;}
    }
}
