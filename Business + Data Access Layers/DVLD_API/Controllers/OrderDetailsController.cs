using Microsoft.AspNetCore.Mvc;
using System.Data;
using BusinessLayer;
using System.Collections.Generic;

namespace DVLD_API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderDetailsController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateOrder([FromBody] OrderDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { message = "بيانات الطلب غير صالحة." });
            }

            try
            {
                clsOrderDetails newOrder = new clsOrderDetails();
                newOrder.PersonID = dto.PersonID;
                newOrder.LicensesClassID = dto.LicenseClassID; 
                newOrder.AdminID = dto.AdminID > 0 ? dto.AdminID : (byte)1;
                newOrder.PassedTests = 0;
                newOrder.Status = 0; // قيمة ثابتة مباشرة بدون تعقيد

                if (newOrder.Save())
                {
                    return Ok(new { message = "تم تقديم طلب الرخصة بنجاح!" });
                }
                else
                {
                    return StatusCode(500, new { message = "فشل الحفظ في قاعدة البيانات." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "خطأ قاعدة بيانات: " + ex.Message });
            }
        }

           

   [HttpGet("national/{nationalID}")]
public IActionResult GetOrdersByNationalID(string nationalID)
{
    try
    {
        DataTable dt = clsOrderDetails.GetOrdersByNationalID(nationalID);

        if (dt == null || dt.Rows.Count == 0)
        {
            return NotFound(new { message = "لا توجد طلبات مسجلة برقم الهوية هذا." });
        }

        // تحويل DataTable إلى قائمة نظيفة من الكواميس لتفادي خطأ الـ Serialization
        var list = new List<Dictionary<string, object>>();
        foreach (DataRow row in dt.Rows)
        {
            var dict = new Dictionary<string, object>();
            foreach (DataColumn col in dt.Columns)
            {
                dict[col.ColumnName] = row[col] == DBNull.Value ? null : row[col];
            }
            list.Add(dict);
        }

        return Ok(list);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { message = "خطأ في السيرفر: " + ex.Message });
    }
}
    }

    

    public class OrderDto
    {
        public int PersonID { get; set; }
        public int LicenseClassID { get; set; }
        public byte AdminID { get; set; }
        public byte PassedTests { get; set; }
        public byte Status { get; set; }
    }
}