using Hospital.DatabaseServices;
using Hospital.Managers;
using Hospital.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hospital.Tests.Managers
{
    [TestFixture]
    public class MedicalProcedureManagerTests
    {
        private Mock<IMedicalProceduresDatabaseService> _mockDbService;
        private MedicalProcedureManager _manager;

        [SetUp]
        public void SetUp()
        {
            _mockDbService = new Mock<IMedicalProceduresDatabaseService>();
            _manager = new MedicalProcedureManager(_mockDbService.Object);

            MedicalProcedureManager.Procedures.Clear();
        }

        [Test]
        public async Task LoadProceduresByDepartmentId_LoadsDataIntoStaticList()
        {
            var departmentId = 1;
            var procedures = new List<ProcedureModel>
            {
                new ProcedureModel(1, 1, "MRI", TimeSpan.FromMinutes(30)),
                new ProcedureModel(2, 1, "CT Scan", TimeSpan.FromMinutes(20))
            };

            _mockDbService.Setup(s => s.GetProceduresByDepartmentId(departmentId))
                          .ReturnsAsync(procedures);

            await _manager.LoadProceduresByDepartmentId(departmentId);

            Assert.That(MedicalProcedureManager.Procedures, Has.Count.EqualTo(2));
            Assert.That(MedicalProcedureManager.Procedures[0].ProcedureName, Is.EqualTo("MRI"));
        }

        [Test]
        public async Task LoadProceduresByDepartmentId_ExceptionThrown_LogsAndKeepsListEmpty()
        {
            var departmentId = 1;
            _mockDbService.Setup(s => s.GetProceduresByDepartmentId(departmentId))
                          .ThrowsAsync(new System.Exception("Database error"));

            await _manager.LoadProceduresByDepartmentId(departmentId);

            Assert.That(MedicalProcedureManager.Procedures.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetProcedures_ReturnsCurrentList()
        {
            MedicalProcedureManager.Procedures.Add(new ProcedureModel(3, 2, "X-Ray", TimeSpan.FromMinutes(10)));

            var result = _manager.GetProcedures();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ProcedureName, Is.EqualTo("X-Ray"));
        }
    }
}
