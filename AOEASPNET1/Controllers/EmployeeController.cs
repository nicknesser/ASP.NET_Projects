using Microsoft.AspNetCore.Mvc;

//imported these packages for controller
using Microsoft.AspNetCore.Http;
using System.Data;
using MySql.Data.MySqlClient;
using AOEASPNET1.Models;
using System.Runtime.Intrinsics.Arm;

namespace AOEASPNET1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        //added private readonly Iconfiguration
        private readonly IConfiguration _configuration;

        //added dependency injection to create the post to Photo
        private readonly IWebHostEnvironment _env;

        //constructed the Employee Controller
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        /*Written Get method using MySql.Data.MySqlClient. This required adding a reference
         * to the project from my SQL Connector MySql.Data.dll file (found through MySql Installer)
         * I also needed to import System.Data and Microsoft.AspNetCore.Mvc (from NuGet)*/

        [HttpGet]

        public JsonResult Get()
        {
            string query = @"select EmployeeId,EmployeeName,Department,
            DATE_FORMAT(DateOfJoining,'%Y-%m-%d') as DateOfJoining,
            PhotoFileName
            from mytestdb.Employee";

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

        public JsonResult Post(Employee emp)
        {
            string query = @"
            insert into mytestdb.Employee (EmployeeName, Department, 
            DateOfJoining, PhotoFileName) values 
            (@EmployeeName, @Department, @DateOfJoining, @PhotoFileName);
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpPut]

        public JsonResult Put(Employee emp)
        {
            string query = @"
            update mytestdb.Employee set 
            EmployeeName =  @EmployeeName
            Department =  @Department
            DateOfJoining =  @DateOfJoining
            PhotoFileName =  @PhotoFileName
            where EmployeeId = @EmployeeId;
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }

        // Adding ("{id}") forces user input for an id
        [HttpDelete("{id}")]

        public JsonResult Delete(int id)
        {
            string query = @"
            delete from mytestdb.Employee
            where EmployeeId = @EmployeeId;
            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            MySqlDataReader myReader;
            using (MySqlConnection mycon = new MySqlConnection(sqlDataSource))
            {
                mycon.Open();
                using (MySqlCommand myCommand = new MySqlCommand(query, mycon))
                {
                    //myCommand with parameters being pulled from the id inputted
                    myCommand.Parameters.AddWithValue("@EmployeeIdId", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);

                    myReader.Close();
                    mycon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        //create API method that posts to Photos directory
        [Route("SaveFile")]
        [HttpPost]


        //method using httpRequest, _env var from IConfiguration | IWebHostEnvironment
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath+ "/Photos/" + filename;

                using(var stream = new FileStream(physicalPath,FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);

            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }

    }
}
