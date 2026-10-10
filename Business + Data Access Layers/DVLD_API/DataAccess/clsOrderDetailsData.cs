using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class clsOrderDetailsData
    {
        public static DataTable GetAllOrders()
        {
            DataTable dt = new DataTable();
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM OrderDetails";
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

        public static bool FindOrderByID(int OrderID, ref int PersonID, ref int LicensesClassID, ref byte AdminID, ref int PassedTests, ref byte Status)
        {
            bool IsFound = false;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM OrderDetails WHERE OrderID = @OrderID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                Connection.Open();
                SqlDataReader reader = Command.ExecuteReader();

                if (reader.Read())
                {
                    PersonID = (int)reader["PersonID"];
                    LicensesClassID = (int)reader["LicensesClassID"];
                    AdminID = (byte)reader["AdminID"];
                    PassedTests = (int)reader["PassedTests"];
                    Status = (byte)reader["Status"];
                    IsFound = true;
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

        

        public static int AddNewOrder(int PersonID, int LicensesClassID, byte AdminID, int PassedTests, byte Status)
        {
            int OrderID = -1;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO OrderDetails (PersonID, LicensesClassID, AdminID, PassedTests, Status)
                             VALUES (@PersonID, @LicensesClassID, @AdminID, @PassedTests, @Status);
                             SELECT SCOPE_IDENTITY();";

            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@LicensesClassID", LicensesClassID);
            Command.Parameters.AddWithValue("@AdminID", AdminID);
            Command.Parameters.AddWithValue("@PassedTests", PassedTests);
            Command.Parameters.AddWithValue("@Status", Status);

            try
            {
                Connection.Open();
                object result = Command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int ID))
                {
                    OrderID = ID;
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

            return OrderID;
        }

        public static bool UpdateOrder(int OrderID, int PersonID, int LicensesClassID, byte AdminID, int PassedTests, byte Status)
        {
            int RowsAffected = 0;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE OrderDetails 
                             SET PersonID = @PersonID,
                                 LicensesClassID = @LicensesClassID,
                                 AdminID = @AdminID,
                                 PassedTests = @PassedTests,
                                 Status = @Status
                             WHERE OrderID = @OrderID";

            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@OrderID", OrderID);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@LicensesClassID", LicensesClassID);
            Command.Parameters.AddWithValue("@AdminID", AdminID);
            Command.Parameters.AddWithValue("@PassedTests", PassedTests);
            Command.Parameters.AddWithValue("@Status", Status);

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

        public static bool DeleteOrderByID(int OrderID)
        {
            int RowsAffected = 0;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"DELETE FROM OrderDetails WHERE OrderID = @OrderID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@OrderID", OrderID);

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
// 1. دالة التحقق من الطلب المسبق
public static bool IsLicenseRequestedBefore(int personID, int licenseClassID)
{
    bool isFound = false;
    string connectionString = clsDataAccessSettings.ConnectionString;
    
    // تم تصحيح اسم العمود إلى LicensesClassID
    string query = "SELECT 1 FROM OrderDetails WHERE PersonID = @PersonID AND LicensesClassID = @LicensesClassID";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PersonID", personID);
            command.Parameters.AddWithValue("@LicensesClassID", licenseClassID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    isFound = true;
                }
            }
            catch (Exception ex)
            {
                // يمكن تسجيل الخطأ هنا
            }
        }
    }
    return isFound;
}
    public static DataTable GetOrdersByNationalID(string nationalID)
{
    DataTable dt = new DataTable();
    
    // تأكد من استخدام LicensesClassID (مع حرف الـ s كما ظهر في الـ Console) ومن جلب الـ ClassName
    string query = @"SELECT OrderDetails.*, People.NationalID, People.FullName, LicenseClasses.ClassName 
                     FROM OrderDetails 
                     INNER JOIN People ON OrderDetails.PersonID = People.PersonID 
                     LEFT JOIN LicenseClasses ON OrderDetails.LicensesClassID = LicenseClasses.LicenseClassID
                     WHERE People.NationalID = @NationalID";

    using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
    {
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@NationalID", nationalID);

            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                // معالجة الخطأ
            }
        }
    }
    return dt;
}


    }
}