using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace App1
{

    [TestFixture]
    public class EmployeeDALTests
    {
        private const string ConnectionString = "Server=localhost;Database=technobrain;Uid=root;Pwd=''";

        private MySqlConnection connection;
        private EmployeeDAL dal;

        [SetUp]
        public void Setup()
        {
            connection = new MySqlConnection(ConnectionString);
            connection.Open();

            dal = new EmployeeDAL(connection);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the test database
            using (var command = new MySqlCommand("DELETE FROM employees", connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        [Test]
        public void TestUpdateEmployeeSalary()
        {
            // Insert a test employee record
            using (var command = new MySqlCommand("INSERT INTO employees (name, salary) VALUES ('John Doe', 5000)", connection))
            {
                command.ExecuteNonQuery();
            }

            // Retrieve the employee ID and initial salary
            int empID = 1;
            int initialSalary = 5000;

            // Call the method to update the salary
            int newSalary = 6000;
            int oldSalary = 0;

            dal.UpdateEmployeeSalary(empID, newSalary, out oldSalary);

            // Verify that the salary has been updated
            using (var command = new MySqlCommand("SELECT salary FROM employees WHERE id = @empID", connection))
            {
                command.Parameters.AddWithValue("@empID", empID);

                int updatedSalary = Convert.ToInt32(command.ExecuteScalar());

                Assert.AreEqual(newSalary, updatedSalary);
                Assert.AreEqual(initialSalary, oldSalary);
            }
        }
    }

}
