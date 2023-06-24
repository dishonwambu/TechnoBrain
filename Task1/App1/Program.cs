using App1;

//internal class Program
//{
// private static void Main(string[] args)
// {
//  EmployeeDataAccessTests tests = new EmployeeDataAccessTests();
//  tests.TestUpdateEmployeeSalary();
//creating an instance of the EmployeeDataAccessTests class and calling
//the TestUpdateEmployeeSalary method to execute the unit test.
//    }
//}
using NUnitLite;

class Program
{
    static int Main(string[] args)
    {
        return new AutoRun().Execute(args);
    }
}
