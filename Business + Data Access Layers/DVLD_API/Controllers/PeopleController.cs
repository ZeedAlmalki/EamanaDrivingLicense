using Microsoft.AspNetCore.Mvc;
using System.Data;
using BusinessLayer;
using System.Collections.Generic;

namespace DVLD_API.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        // 1. جلب كل الأشخاص
        [HttpGet("all")]
        public IActionResult GetAllPeople()
        {
            DataTable dt = clsPerson.GetAllPeople();
            if (dt == null || dt.Rows.Count == 0) return NotFound(new { message = "No people found." });

            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return Ok(list);
        }

        // 2. جلب شخص عن طريق الـ ID
        [HttpGet("{id}")]
        public IActionResult GetPersonInfoByID(int id)
        {
            clsPerson person = clsPerson.FindByPersonID(id);
            if (person == null) return NotFound(new { message = $"Person with ID {id} not found." });

            return Ok(person);
        }

        // 3. إضافة شخص جديد (POST)
        [HttpPost]
        public IActionResult AddNewPerson([FromBody] clsPerson person)
        {
            if (person == null) return BadRequest("Invalid data.");

            if (person.Save())
            {
                return Ok(new { message = "Person added successfully.", personID = person.PersonID });
            }
            return StatusCode(500, new { message = "Failed to add person." });
        }

        // 4. حذف شخص (DELETE)
        [HttpDelete("{id}")]
        public IActionResult DeletePerson(int id)
        {
            if (clsPerson.DeletePerson(id))
            {
                return Ok(new { message = "Person deleted successfully." });
            }
            return NotFound(new { message = "Person not found or could not be deleted." });
        }
    

// 5. التحقق من تسجيل الدخول
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.NationalID) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest(new { message = "الرجاء إدخال رقم الهوية وكلمة السر." });
            }

            // تأكد من وجود دالة في BusinessLayer تبحث عن الشخص برقم الهوية وكلمة السر معاً
            clsPerson person = clsPerson.FindByNationalIDAndPassword(loginDto.NationalID, loginDto.Password);

            if (person != null)
            {
                return Ok(new { message = "تم تسجيل الدخول بنجاح", person });
            }

            return Unauthorized(new { message = "رقم الهوية أو كلمة السر غير صحيحة." });
        }

        // كلاس بسيط لاستقبال بيانات تسجيل الدخول
        public class LoginDto
        {
            public string NationalID { get; set; }
            public string Password { get; set; }
        }
    }
}