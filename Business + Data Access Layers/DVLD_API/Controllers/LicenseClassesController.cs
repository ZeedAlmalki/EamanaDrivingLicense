using Microsoft.AspNetCore.Mvc;
using System.Data;
using BusinessLayer;
using System.Collections.Generic;

namespace DVLD_API.Controllers
{
    [Route("api/license-classes")]
    [ApiController]
    public class LicenseClassesController : ControllerBase
    {
        [HttpGet("all")]
        public IActionResult GetAllLicenseClasses()
        {
            DataTable dt = clsLicenseClass.GetAllLicenseClasses();
            if (dt == null || dt.Rows.Count == 0) return NotFound(new { message = "No license classes found." });

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
    }
}