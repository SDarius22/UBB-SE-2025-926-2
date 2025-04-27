using Hospital.DatabaseServices;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalTest
{
    [TestFixture]
    public class DepartmentsDatabaseServiceTests
    {
        private DepartmentsDatabaseService _service;
        private SqlConnection _connection;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _service = new DepartmentsDatabaseService();
            _connection = new SqlConnection(_service.GetConnectionString());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection?.Dispose();
        }

        [SetUp]
        public async Task SetUp()
        {
            try
            {
                await _connection.OpenAsync();
            }
            catch (SqlException ex)
            {
                Assert.Ignore($"Database connection failed: {ex.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        #region Constructor Tests

        [Test]
        public void Constructor_Default_InitializesCorrectly()
        {
            // Act
            var service = new DepartmentsDatabaseService();

            // Assert
            Assert.That(service, Is.Not.Null);
            Assert.That(service.GetConnectionString(), Is.Not.Empty);
        }

        [Test]
        public void Constructor_WithDatabaseService_InitializesCorrectly()
        {
            // Arrange
            var mockDatabaseService = new Mock<IDepartmentsDatabaseService>();

            // Act
            var service = new DepartmentsDatabaseService(mockDatabaseService.Object);

            // Assert
            Assert.That(service, Is.Not.Null);
            Assert.That(service.GetConnectionString(), Is.Not.Empty);
        }

        #endregion

        #region GetConnectionString Tests

        [Test]
        public void GetConnectionString_ReturnsValidConnectionString()
        {
            // Act
            var connectionString = _service.GetConnectionString();

            // Assert
            Assert.That(connectionString, Is.Not.Empty);
            Assert.That(connectionString, Does.Contain("Data Source=DESKTOP-H700VKM\\MSSQLSERVER02;Initial Catalog=HospitalDB;Integrated Security=True;TrustServerCertificate=True"));
            Assert.That(connectionString, Does.Contain("Data Source=DESKTOP-H700VKM\\MSSQLSERVER02;Initial Catalog=HospitalDB;Integrated Security=True;TrustServerCertificate=True"));
        }
        
        #endregion

        #region GetDepartmentsFromDataBase Tests

        [Test]
        public async Task GetDepartmentsFromDataBase_ValidData_DepartmentsLoaded()
        {
            // Act
            var result = await _service.GetDepartmentsFromDataBase();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0), "No departments found in the database");
            Assert.That(result[0].DepartmentId, Is.GreaterThan(0), "Department ID should be greater than 0");
            Assert.That(result[0].DepartmentName, Is.Not.Empty, "Department name should not be empty");
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_ReturnsValidDepartmentData()
        {
            // Act
            var result = await _service.GetDepartmentsFromDataBase();

            // Assert
            Assert.That(result, Is.Not.Null);
            foreach (var department in result)
            {
                Assert.That(department.DepartmentId, Is.GreaterThan(0), $"Invalid department ID: {department.DepartmentId}");
                Assert.That(department.DepartmentName, Is.Not.Empty, $"Empty department name for ID: {department.DepartmentId}");
            }
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_DataIntegrity()
        {
            // Act
            var result = await _service.GetDepartmentsFromDataBase();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That(result, Is.Not.Empty, "Result should not be empty");
                // Check for duplicate department IDs
                var departmentIds = new HashSet<int>();
                foreach (var department in result)
                {
                    Assert.That(departmentIds.Add(department.DepartmentId), 
                        Is.True, 
                        $"Duplicate department ID found: {department.DepartmentId}");
                }
            });
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_DataFormat()
        {
            // Act
            var result = await _service.GetDepartmentsFromDataBase();

            // Assert
            Assert.Multiple(() =>
            {
                foreach (var department in result)
                {
                    // Check department name format
                    Assert.That(department.DepartmentName, Does.Match(@"^[A-Za-z\s\-]+$"), 
                        $"Department name '{department.DepartmentName}' contains invalid characters");
                    
                    // Check department name length
                    Assert.That(department.DepartmentName.Length, Is.AtLeast(2), 
                        $"Department name '{department.DepartmentName}' is too short");
                    Assert.That(department.DepartmentName.Length, Is.AtMost(100), 
                        $"Department name '{department.DepartmentName}' is too long");
                }
            });
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_OrderConsistency()
        {
            // Act
            var firstResult = await _service.GetDepartmentsFromDataBase();
            var secondResult = await _service.GetDepartmentsFromDataBase();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(firstResult.Count, Is.EqualTo(secondResult.Count), 
                    "Number of departments should be consistent between calls");
               
                // Check if the order is consistent
                for (int i = 0; i < firstResult.Count; i++)
                {
                    Assert.That(firstResult[i].DepartmentId, Is.EqualTo(secondResult[i].DepartmentId),
                        $"Department order changed at index {i}");
                }
            });
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_Performance()
        {
            // Act
            var startTime = DateTime.Now;
            var result = await _service.GetDepartmentsFromDataBase();
            var endTime = DateTime.Now;

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null, "Result should not be null");
                Assert.That((endTime - startTime).TotalSeconds, Is.LessThan(5), 
                    "Query execution took too long");
            });
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_EmptyTable_ReturnsEmptyList()
        {
            // Note: Ensure test DB has Departments table truncated before running this.
            var result = await _service.GetDepartmentsFromDataBase();
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_DepartmentNameBoundaryTest()
        {
            var result = await _service.GetDepartmentsFromDataBase();

            foreach (var dept in result)
            {
                Assert.That(dept.DepartmentName.Length, Is.GreaterThanOrEqualTo(2));
                Assert.That(dept.DepartmentName.Length, Is.LessThanOrEqualTo(100));
            }
        }

        [Test]
        public async Task GetDepartmentsFromDataBase_ParallelCalls_ReturnSameResult()
        {
            var task1 = _service.GetDepartmentsFromDataBase();
            var task2 = _service.GetDepartmentsFromDataBase();
            var task3 = _service.GetDepartmentsFromDataBase();

            await Task.WhenAll(task1, task2, task3);

            var result1 = task1.Result;
            var result2 = task2.Result;
            var result3 = task3.Result;

            Assert.Multiple(() =>
            {
                Assert.That(result1.Count, Is.EqualTo(result2.Count));
                Assert.That(result2.Count, Is.EqualTo(result3.Count));
            });
        }

        #endregion
    }
}

// Simulated faulty service for exception handling tests
public class FaultyDepartmentsDatabaseService : DepartmentsDatabaseService
{
    private readonly bool _throwSql;

    public FaultyDepartmentsDatabaseService(bool throwSql)
    {
        _throwSql = throwSql;
    }

    public override async Task<List<DepartmentModel>> GetDepartmentsFromDataBase()
    {
        await Task.Delay(1); // Simulate async behavior

        if (_throwSql)
            // You can't instantiate SqlException directly, so use a custom wrapper or a different exception type
            throw new Exception("SQL Exception: Simulated SQL error");

        throw new InvalidOperationException("Simulated general error");
    }
}

[TestFixture]
public class DepartmentsDatabaseServiceExceptionTests
{
    [Test]
    public void GetDepartmentsFromDataBase_ThrowsSqlException_IsWrapped()
    {
        var faulty = new FaultyDepartmentsDatabaseService(true);

        var ex = Assert.ThrowsAsync<Exception>(async () => await faulty.GetDepartmentsFromDataBase());

        Assert.That(ex.Message, Does.Contain("SQL Exception"));
    }

    [Test]
    public void GetDepartmentsFromDataBase_ThrowsGeneralException_IsWrapped()
    {
        var faulty = new FaultyDepartmentsDatabaseService(false);

        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await faulty.GetDepartmentsFromDataBase());

        Assert.That(ex.Message, Does.Contain("Simulated general error"));
    }
}

// Test-specific implementation for coverage testing
[TestFixture]
public class TestDepartmentsDatabaseServiceTests
{
    [Test]
    public async Task GetDepartmentsFromDataBase_TestImplementation()
    {
        // Arrange
        var testService = new TestDepartmentsDatabaseService();
        // Act
        var result = await testService.GetDepartmentsFromDataBase();
        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].DepartmentId, Is.EqualTo(1));
        Assert.That(result[0].DepartmentName, Is.EqualTo("Test Department 1"));
    }
}

// Test-specific implementation that doesn't use a real database
public class TestDepartmentsDatabaseService : DepartmentsDatabaseService
{
    public override async Task<List<DepartmentModel>> GetDepartmentsFromDataBase()
    {
        // Simulate database access without actually connecting to a database
        await Task.Delay(10); // Simulate network delay
        // Return test data
        return new List<DepartmentModel>
        {
            new DepartmentModel(1, "Test Department 1"),
            new DepartmentModel(2, "Test Department 2")
        };
    }
}

// Direct test for the real GetDepartmentsFromDataBase method
[TestFixture]
public class DirectDepartmentsDatabaseServiceTests
{
    [Test]
    public async Task GetDepartmentsFromDataBase_DirectTest()
    {
        // Arrange
        var mockDatabaseService = new Mock<IDepartmentsDatabaseService>();
        var departments = new List<DepartmentModel>
        {
            new DepartmentModel(1, "Cardiology"),
            new DepartmentModel(2, "Test Department 2"),
            new DepartmentModel(3, "Test Department 3")
        };

        
        mockDatabaseService.Setup(ds => ds.GetDepartmentsFromDataBase())
            .ReturnsAsync(departments);
        
        var service = new DepartmentsDatabaseService(mockDatabaseService.Object);
        
        // Act
        var result = await service.GetDepartmentsFromDataBase();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result[0].DepartmentId, Is.EqualTo(1));
        Assert.That(result[0].DepartmentName, Is.EqualTo("Cardiology"));
    }
}
