using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class clsPersonData
    {
        public static int AddNewPerson(string FullName, string PhoneNumber, string Email, string NationalID, string City, short Gender, int CountryID, string Password)
        {
            int PersonID = -1;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO People (FullName, PhoneNumber, Email, NationalID, City, Gender, CountryID, Password)
                             VALUES (@FullName, @PhoneNumber, @Email, @NationalID, @City, @Gender, @CountryID, @Password);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand Command = new SqlCommand(query, Connection);

            Command.Parameters.AddWithValue("@FullName", FullName);
            Command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);
            
            if (!string.IsNullOrEmpty(Email))
                Command.Parameters.AddWithValue("@Email", Email);
            else
                Command.Parameters.AddWithValue("@Email", DBNull.Value);

            Command.Parameters.AddWithValue("@NationalID", NationalID);
            Command.Parameters.AddWithValue("@City", City);
            Command.Parameters.AddWithValue("@Gender", Gender);
            Command.Parameters.AddWithValue("@CountryID", CountryID);
            Command.Parameters.AddWithValue("@Password", Password);

            try
            {
                Connection.Open();
                object result = Command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    PersonID = InsertedID;
                }
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return PersonID;
        }

        public static bool UpdatePerson(int PersonID, string FullName, string PhoneNumber, string Email, string NationalID, string City, short Gender, int CountryID, string Password)
        {
            int RowsAffected = 0;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE People 
                             SET FullName = @FullName,
                                 PhoneNumber = @PhoneNumber,
                                 Email = @Email,
                                 NationalID = @NationalID,
                                 City = @City,
                                 Gender = @Gender,
                                 CountryID = @CountryID,
                                 Password = @Password
                             WHERE PersonID = @PersonID";

            SqlCommand Command = new SqlCommand(query, Connection);

            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@FullName", FullName);
            Command.Parameters.AddWithValue("@PhoneNumber", PhoneNumber);

            if (!string.IsNullOrEmpty(Email))
                Command.Parameters.AddWithValue("@Email", Email);
            else
                Command.Parameters.AddWithValue("@Email", DBNull.Value);

            Command.Parameters.AddWithValue("@NationalID", NationalID);
            Command.Parameters.AddWithValue("@City", City);
            Command.Parameters.AddWithValue("@Gender", Gender);
            Command.Parameters.AddWithValue("@CountryID", CountryID);
            Command.Parameters.AddWithValue("@Password", Password);

            try
            {
                Connection.Open();
                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return (RowsAffected > 0);
        }

        public static bool DeletePerson(int PersonID)
        {
            int RowsAffected = 0;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"DELETE FROM People WHERE PersonID = @PersonID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                Connection.Open();
                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return (RowsAffected > 0);
        }

        public static bool GetPersonInfoByID(int PersonID, ref string FullName, ref string PhoneNumber, ref string Email, ref string NationalID, ref string City, ref short Gender, ref int CountryID, ref string Password)
        {
            bool IsFound = false;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM People WHERE PersonID = @PersonID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                Connection.Open();
                SqlDataReader reader = Command.ExecuteReader();

                if (reader.Read())
                {
                    IsFound = true;
                    FullName = reader["FullName"].ToString();
                    PhoneNumber = reader["PhoneNumber"].ToString();
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                    NationalID = reader["NationalID"].ToString();
                    City = reader["City"].ToString();
                    Gender = Convert.ToInt16(reader["Gender"]);
                    CountryID = Convert.ToInt32(reader["CountryID"]);
                    Password = reader["Password"].ToString();
                }
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static bool GetPersonInfoByNationalID(string NationalID, ref int PersonID, ref string FullName, ref string PhoneNumber, ref string Email, ref string City, ref short Gender, ref int CountryID, ref string Password)
        {
            bool IsFound = false;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM People WHERE NationalID = @NationalID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@NationalID", NationalID);

            try
            {
                Connection.Open();
                SqlDataReader reader = Command.ExecuteReader();

                if (reader.Read())
                {
                    IsFound = true;
                    PersonID = Convert.ToInt32(reader["PersonID"]);
                    FullName = reader["FullName"].ToString();
                    PhoneNumber = reader["PhoneNumber"].ToString();
                    Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : "";
                    City = reader["City"].ToString();
                    Gender = Convert.ToInt16(reader["Gender"]);
                    CountryID = Convert.ToInt32(reader["CountryID"]);
                    Password = reader["Password"].ToString();
                }
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static bool IsPersonExist(string NationalID)
        {
            bool IsFound = false;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 1 FROM People WHERE NationalID = @NationalID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@NationalID", NationalID);

            try
            {
                Connection.Open();
                object result = Command.ExecuteScalar();
                if (result != null)
                    IsFound = true;
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT People.*, Countries.CountryName FROM People 
                             INNER JOIN Countries ON People.CountryID = Countries.CountryID";
            SqlCommand Command = new SqlCommand(query, Connection);

            try
            {
                Connection.Open();
                SqlDataReader reader = Command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                SharedDVLD.RegisterLogInEventHandler(ex);
            }
            finally
            {
                Connection.Close();
            }
            return dt;
        }
    


public static DataRow FindByNationalIDAndPassword(string nationalID, string password)
        {
            DataRow row = null;
            DataTable dt = new DataTable();
            
            // استخدام اتصال قاعدة البيانات القياسي لمشروعك
            string connectionString = clsDataAccessSettings.ConnectionString; // تأكد من مطابقة اسم كلاس إعدادات الاتصال لديك

            string query = "SELECT * FROM People WHERE NationalID = @NationalID AND Password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NationalID", nationalID);
                    command.Parameters.AddWithValue("@Password", password);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                dt.Load(reader);
                                if (dt.Rows.Count > 0)
                                {
                                    row = dt.Rows[0];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // يمكنك تسجيل الخطأ هنا إن أردت
                    }
                }
            }
        
            return row;
        }
    }
}