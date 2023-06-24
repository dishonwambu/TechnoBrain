using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    internal class EmployeeDAL
    {
        private MySqlConnection connection;

        public EmployeeDAL(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public void UpdateEmployeeSalary(int empID, int newSalary, out int oldSalary)
        {
            oldSalary = 0;

            // Retrieve the old salary
            using (var command = new MySqlCommand("SELECT salary FROM employees WHERE id = @empID", connection))
            {
                command.Parameters.AddWithValue("@empID", empID);

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    oldSalary = Convert.ToInt32(result);
                }
            }

            // Update the salary
            using (var command = new MySqlCommand("UPDATE employees SET salary = @newSalary WHERE id = @empID", connection))
            {
                command.Parameters.AddWithValue("@newSalary", newSalary);
                command.Parameters.AddWithValue("@empID", empID);

                command.ExecuteNonQuery();
            }
        }
    }
}
