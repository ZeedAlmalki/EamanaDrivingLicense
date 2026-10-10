using System.Data;
using DataAccessLayer;

namespace BusinessLayer
{
    public class clsPerson
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int PersonID { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string NationalID { get; set; }
        public string City { get; set; }
        public short Gender { get; set; } // 0: Male, 1: Female
        public int CountryID { get; set; }
        public string Password { get; set; }

        public clsPerson()
        {
            this.PersonID = -1;
            this.FullName = "";
            this.PhoneNumber = "";
            this.Email = "";
            this.NationalID = "";
            this.City = "";
            this.Gender = 0;
            this.CountryID = -1;
            this.Password = "";
            Mode = enMode.AddNew;
        }

        private clsPerson(int personID, string fullName, string phoneNumber, string email, string nationalID, string city, short gender, int countryID, string password)
        {
            this.PersonID = personID;
            this.FullName = fullName;
            this.PhoneNumber = phoneNumber;
            this.Email = email;
            this.NationalID = nationalID;
            this.City = city;
            this.Gender = gender;
            this.CountryID = countryID;
            this.Password = password;
            Mode = enMode.Update;
        }

        private bool _AddNewPerson()
        {
            this.PersonID = clsPersonData.AddNewPerson(this.FullName, this.PhoneNumber, this.Email, this.NationalID, this.City, this.Gender, this.CountryID, this.Password);
            return (this.PersonID != -1);
        }

        private bool _UpdatePerson()
        {
            return clsPersonData.UpdatePerson(this.PersonID, this.FullName, this.PhoneNumber, this.Email, this.NationalID, this.City, this.Gender, this.CountryID, this.Password);
        }

        public static DataTable GetAllPeople()
        {
            return clsPersonData.GetAllPeople();
        }

        public static bool DeletePerson(int PersonID)
        {
            return clsPersonData.DeletePerson(PersonID);
        }

        public static bool IsPersonExist(string NationalID)
        {
            return clsPersonData.IsPersonExist(NationalID);
        }

        public static clsPerson FindByPersonID(int PersonID)
        {
            string fullName = "", phoneNumber = "", email = "", nationalID = "", city = "", password = "";
            short gender = 0;
            int countryID = -1;

            if (clsPersonData.GetPersonInfoByID(PersonID, ref fullName, ref phoneNumber, ref email, ref nationalID, ref city, ref gender, ref countryID, ref password))
            {
                return new clsPerson(PersonID, fullName, phoneNumber, email, nationalID, city, gender, countryID, password);
            }
            return null;
        }

        public static clsPerson FindByNationalID(string NationalID)
        {
            int personID = -1;
            string fullName = "", phoneNumber = "", email = "", city = "", password = "";
            short gender = 0;
            int countryID = -1;

            if (clsPersonData.GetPersonInfoByNationalID(NationalID, ref personID, ref fullName, ref phoneNumber, ref email, ref city, ref gender, ref countryID, ref password))
            {
                return new clsPerson(personID, fullName, phoneNumber, email, NationalID, city, gender, countryID, password);
            }
            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewPerson())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdatePerson();
            }
            return false;
        }

public static clsPerson FindByNationalIDAndPassword(string nationalID, string password)
{
    // استدعاء دالة طبقة البيانات (DAL) للبحث في قاعدة البيانات
    // تأكد من أنك قمت ببرمجة دالة مقابلة في DataAccessLayer باسم FindByNationalIDAndPassword
    DataRow dtPersona = clsPersonData.FindByNationalIDAndPassword(nationalID, password);

    if (dtPersona != null)
    {
        // إرجاع كائن الشخص مملوءاً بالبيانات إذا تم العثور عليه
        return new clsPerson(
            Convert.ToInt32(dtPersona["PersonID"]),
            dtPersona["FullName"].ToString(),
            dtPersona["PhoneNumber"].ToString(),
            dtPersona["Email"].ToString(),
            dtPersona["NationalID"].ToString(),
            dtPersona["City"].ToString(),
            Convert.ToInt16(dtPersona["Gender"]),
            Convert.ToInt32(dtPersona["CountryID"]),
            dtPersona["Password"].ToString()
        );
    }

    return null; // في حال لمطابقة غير صحيحة أو عدم وجود الحساب
}
    }
}