using Microsoft.AspNetCore.Mvc;
using System.Data;
using BusinessLayer;
using System.Collections.Generic;

namespace DVLD_API.Controllers
{
    [Route("api/countries")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        [HttpGet("all")]
        public IActionResult GetAllCountries()
        {
            DataTable dtCountries = clsCountry.GetAllCountries();
            
            if (dtCountries == null || dtCountries.Rows.Count == 0)
            {
                return NotFound(new { message = "No countries found." });
            }

            // تحويل الـ DataTable إلى قائمة من القواميس (List of Dictionaries) لحل مشكلة الـ JSON Serialization
            var listCountries = new List<Dictionary<string, object>>();
            foreach (DataRow row in dtCountries.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dtCountries.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                listCountries.Add(dict);
            }

            return Ok(listCountries);
        }
    }
}