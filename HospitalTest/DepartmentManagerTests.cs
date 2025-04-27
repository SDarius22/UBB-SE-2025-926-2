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
    public class DepartmentManagerTests
    {
        private Mock<IDepartmentsDatabaseService> _mockDatabaseService;
        private DepartmentManager _departmentManager;

        [SetUp]
        public void SetUp()
        {
            _mockDatabaseService = new Mock<IDepartmentsDatabaseService>();
            _departmentManager = new DepartmentManager(_mockDatabaseService.Object);

            DepartmentManager.Departments.Clear();
        }

        [Test]
        public async Task LoadDepartments_PopulatesDepartmentList()
        {
            var departments = new List<DepartmentModel>
            {
                new DepartmentModel(1, "Cardiology"),
                new DepartmentModel(2, "Neurology")
            };


            _mockDatabaseService.Setup(s => s.GetDepartmentsFromDataBase())
                .ReturnsAsync(departments);

            await _departmentManager.LoadDepartments();

            Assert.That(DepartmentManager.Departments.Count, Is.EqualTo(2));
            Assert.That(DepartmentManager.Departments[0].DepartmentName, Is.EqualTo("Cardiology"));
        }

        [Test]
        public void GetDepartments_ReturnsCurrentDepartmentList()
        {
            DepartmentManager.Departments.Add(new DepartmentModel(1, "TestDept"));

            var result = _departmentManager.GetDepartments();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].DepartmentName, Is.EqualTo("TestDept"));
        }
    }
}
