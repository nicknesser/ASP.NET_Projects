using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MySql.Data.MySqlClient;
using AOEASPNET1.Models;


namespace AOEASPNET1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DepartmentController(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        
        /*Written Get method using MySql.Data.MySqlClient. This required adding a reference
         * to the project from my SQL Connector MySql.Data.dll file (found through MySql Installer)
         * I also needed to import System.Data and Microsoft.AspNetCore.Mvc (from NuGet)*/

        [HttpGet]

        public JsonResult Get()
        {
            string query = @"select DepartmentId,DepartmentName from mytestdb.Department";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }    
            }

            return new JsonResult(table);
        }



        /* Created Post method. Added Department object as an input and changed the string query to
         * reflect the MySQL query. Needed to add "@DepartmentName" which seems to be the direct call to
         * the SQL Table. All in all, similar to Get method*/
      
        [HttpPost]

        public JsonResult Post(Department dep)
        {
            string query = @"
            insert into mytestdb.Department (DepartmentName) values 
            (@DepartmentName);
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpPut]

        public JsonResult Put(Department dep)
        {
            string query = @"
            update mytestdb.Department set 
            DepartmentName =  @DepartmentName
            where DepartmentId = @DepartmentId;
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentId", dep.DepartmentId);
                    myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        [HttpDelete("{id}")]

        public JsonResult Delete(int id)
        {
            string query = @"
            delete from mytestdb.Department
            where DepartmentId = @DepartmentId;
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@DepartmentId", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }





    }
}
