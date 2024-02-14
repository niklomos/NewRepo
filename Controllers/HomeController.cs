using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Serilog;
using System.Diagnostics;
using TestAdminLTE1.Models;

namespace TestAdminLTE1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
            
        {
            _logger = logger;
            _configuration = configuration;
            
        }

        public IActionResult TestModel2()
        {
            var students = GetStudentsFromDatabase();
            return View(students);
        }

        public IActionResult TestModel()
        {
            var names = GetNames2();
            return View(names);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Insert()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult SelectStudent()
        {
           
            return View();

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IEnumerable<string> SelectData()
            
        {
            List<string> kong = new List<string>(); 
            string conn = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connect = new MySqlConnection(conn);
            connect.Open();

            var sqlSelect = "SELECT Username,Password FROM testlogin ";
            MySqlCommand cmd = new MySqlCommand(sqlSelect, connect);
            MySqlDataReader reader = cmd.ExecuteReader();
            {
                while (reader.Read())
                {
                    kong.Add(reader["Username"].ToString());
                    kong.Add(reader["Password"].ToString());
                }
            }

            return kong;
        }
        public IEnumerable<string> GetNames()
        {


            List<string> names = new List<string>();
            string con = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(con);
            connection.Open();

            var sqlSelect = "SELECT id,fname,lname FROM Students";
            MySqlCommand nice = new MySqlCommand(sqlSelect, connection);

            MySqlDataReader reader = nice.ExecuteReader();
            {
                while (reader.Read())
                {
                   
                    names.Add(reader["id"].ToString());
                    names.Add(reader["fname"].ToString());
                    names.Add(reader["lname"].ToString());
                }
            }

            return names;

        }
        [HttpPost]
        public IActionResult saveName(string fname, string lname)
        {
            //àÃÕÂ¡ãªé _configuration ãËéä»àÍÒ connectionstring ·Õè connectionStr ã¹ä¿Åì appsettings.json
            string strCon = _configuration.GetConnectionString("connectionStr");

            using (MySqlConnection conn = new MySqlConnection(strCon))
            {
                try
                {

                    conn.Open();
                    //}
                    //catch (Exception ex) {

                    //}
                    //try
                    //{
                    //var sqlquery = new StringBuilder();
                    //sqlquery.AppendFormat("INSERT INTO students (fname, lname) ");
                    //sqlquery.AppendFormat(" VALUES (\"{0}\", \"{1}\")", fname, lname);
                    //var cmd = new MySqlCommand(sqlquery.ToString(), conn);
                    //cmd.ExecuteNonQuery();

                    var sqlquery = "INSERT INTO students (fname, lname) VALUES (@fname, @lname)";
                    using (MySqlCommand cmd = new MySqlCommand(sqlquery, conn))
                    {
                        cmd.Parameters.AddWithValue("@fname", fname);
                        cmd.Parameters.AddWithValue("@lname", lname);

                        cmd.ExecuteNonQuery();
                    }
                    //¡ÒÃºÑ¹·Ö¡¢éÍÁÙÅÅ§ Log
                    Log.Information($"Data saved successfully: {fname} {lname}");

                    return Ok("Data saved successfully.");
                }
                catch (Exception ex)
                {
                    //throw new Exception("cannot connect to database: " + ex);
                    //throw 

                    //¡ÒÃºÑ¹·Ö¡¢éÍÁÙÅÅ§ Log
                    Log.Error(ex, $"Error while saving data: {fname} {lname}");

                    return View("Error");
                }
                finally
                {
                    conn.Close(); // ¨Ñ´¡ÒÃ¡ÒÃ»Ô´ Connection ÀÒÂã¹ finally block
                }
            }
        }


        [HttpPost]
        public IActionResult DeleteData(int id)
        {
            string strCon = _configuration.GetConnectionString("connectionStr");

            using (MySqlConnection conn = new MySqlConnection(strCon))
            {
                try
                {
                    conn.Open();
                    var sqlquery = "DELETE FROM students WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlquery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                    }

                    Log.Information($"Data with ID {id} deleted successfully");

                    return Ok("Data deleted successfully.");
                }
                catch (Exception ex)
                {

                    Log.Error(ex, $"Error while deleting data with ID {id}");

                    return View("Error");
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public IActionResult Edit(int id)
        {
            // ดึงข้อมูลของนักเรียนโดยใช้ ID
            var student = GetStudentById(id);

            if (student != null)
            {
                // ส่งข้อมูลของนักเรียนไปยังหน้า Edit.cshtml
                return View(student);
            }
            else
            {
                // หากไม่พบข้อมูล สามารถแสดงหน้าข้อผิดพลาดหรือใช้โค้ดอื่นๆ ตามความเหมาะสม
                return NotFound();
            }
        }


        private Student GetStudentById(int id)
        {
            // สร้าง connection string สำหรับการเชื่อมต่อ MySQL database
            string connectionString = _configuration.GetConnectionString("connectionStr");

            // สร้างคำสั่ง SQL สำหรับการดึงข้อมูลนักเรียนโดยใช้ ID
            string query = "SELECT id, fname, lname FROM Students WHERE id = @Id";

            // สร้าง connection object และ command object
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // เพิ่ม parameter เพื่อป้องกัน SQL injection
                command.Parameters.AddWithValue("@Id", id);

                try
                {
                    // เปิดการเชื่อมต่อ MySQL database
                    connection.Open();

                    // สร้าง reader object เพื่ออ่านข้อมูลจาก MySQL database
                    MySqlDataReader reader = command.ExecuteReader();

                    // ตรวจสอบว่าพบข้อมูลหรือไม่
                    if (reader.Read())
                    {
                        // สร้าง object ข้อมูลนักเรียน
                        var student = new Student
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Fname = reader["fname"].ToString(),
                            Lname = reader["lname"].ToString()
                        };

                        return student;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // หากเกิดข้อผิดพลาดในการเชื่อมต่อหรือการอ่านข้อมูล
                    // สามารถจัดการข้อผิดพลาดตามที่ต้องการ
                    // เช่น แสดงหน้าข้อผิดพลาด บันทึก log เป็นต้น
                    _logger.LogError(ex, "Error while fetching student data.");
                    return null;
                }
            }
        }

        [HttpPost]
        public IActionResult Update(Student student)
        {
            string connectionString = _configuration.GetConnectionString("connectionStr");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var sqlquery = "UPDATE Students SET fname = @fname, lname = @lname WHERE id = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlquery, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", student.Id);
                        cmd.Parameters.AddWithValue("@fname", student.Fname);
                        cmd.Parameters.AddWithValue("@lname", student.Lname);

                        cmd.ExecuteNonQuery();
                    }

                    Log.Information($"Data with ID {student.Id} updated successfully");

                    return Ok("Data updated successfully.");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Error while updating data with ID {student.Id}");
                    return View("Error");
                }
                finally
                {
                    connection.Close();
                }
            }
        }




        private Student GetNameByModel()
        {
            // สร้าง connection string สำหรับการเชื่อมต่อ MySQL database
            string connectionString = _configuration.GetConnectionString("connectionStr");

            // สร้างคำสั่ง SQL สำหรับการดึงข้อมูลนักเรียนโดยใช้ ID
            string query = "SELECT Id, Username, Password FROM testlogin ";;

            // สร้าง connection object และ command object
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                // เพิ่ม parameter เพื่อป้องกัน SQL injection

                try
                {
                    // เปิดการเชื่อมต่อ MySQL database
                    connection.Open();

                    // สร้าง reader object เพื่ออ่านข้อมูลจาก MySQL database
                    MySqlDataReader reader = command.ExecuteReader();

                    // ตรวจสอบว่าพบข้อมูลหรือไม่
                    if (reader.Read())
                    {
                        // สร้าง object ข้อมูลนักเรียน
                        var student = new Student
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Fname = reader["fname"].ToString(),
                            Lname = reader["lname"].ToString()
                        };

                        return student;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    // หากเกิดข้อผิดพลาดในการเชื่อมต่อหรือการอ่านข้อมูล
                    // สามารถจัดการข้อผิดพลาดตามที่ต้องการ
                    // เช่น แสดงหน้าข้อผิดพลาด บันทึก log เป็นต้น
                    _logger.LogError(ex, "Error while fetching student data.");
                    return null;
                }
            }
        }

        public IEnumerable<string> GetNames2()
        {
            List<string> names = new List<string>();
            string con = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(con);
            connection.Open();

            var sqlSelect = "SELECT id,fname,lname FROM Students";
            MySqlCommand nice = new MySqlCommand(sqlSelect, connection);

            MySqlDataReader reader = nice.ExecuteReader();
            {
                while (reader.Read())
                {
                    names.Add($"ID: {reader["id"]}, First Name: {reader["fname"]}, Last Name: {reader["lname"]}");
                }
            }

            return names;
        }

        // ฟังก์ชันสำหรับดึงข้อมูลนักเรียนจากฐานข้อมูล
        private IEnumerable<Student> GetStudentsFromDatabase()
        {
            List<Student> students = new List<Student>();

            string connectionString = _configuration.GetConnectionString("connectionStr");
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                string sqlSelect = "SELECT id, fname, lname FROM Students";
                MySqlCommand command = new MySqlCommand(sqlSelect, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Student student = new Student
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Fname = reader["fname"].ToString(),
                        Lname = reader["lname"].ToString()
                    };

                    students.Add(student);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching student data from database.");
            }
            finally
            {
                connection.Close();
            }

            return students;
        }

    }
}
