using System.Data;
using DataAccessLayer;

namespace BusinessLayer
{
    public class clsLicenseClass
    {
        public int LicenseClassID { get; set; }
        public string ClassName { get; set; }
        public decimal DefaultFees { get; set; }

        public clsLicenseClass()
        {
            LicenseClassID = -1;
            ClassName = string.Empty;
            DefaultFees = 0;
        }

        private clsLicenseClass(int licenseClassID, string className, decimal defaultFees)
        {
            this.LicenseClassID = licenseClassID;
            this.ClassName = className;
            this.DefaultFees = defaultFees;
        }

        public static DataTable GetAllLicenseClasses()
        {
            return clsLicenseClassesData.GetAllLicenseClasses();
        }

        public static clsLicenseClass FindLicenseClassByID(int LicenseClassID)
        {
            string className = string.Empty;
            decimal defaultFees = 0;

            if (clsLicenseClassesData.FindLicenseClassByID(LicenseClassID, ref className, ref defaultFees))
            {
                return new clsLicenseClass(LicenseClassID, className, defaultFees);
            }
            return null;
        }
    }
}