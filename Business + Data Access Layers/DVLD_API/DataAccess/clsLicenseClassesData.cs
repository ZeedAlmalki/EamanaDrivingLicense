using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer
{
    public class clsLicenseClassesData
    {
        public static DataTable GetAllLicenseClasses()
        {
            DataTable dt = new DataTable();
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT LicenseClassID, ClassName, DefaultFees FROM LicenseClasses";
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

        public static bool FindLicenseClassByID(int LicenseClassID, ref string ClassName, ref decimal DefaultFees)
        {
            bool IsFound = false;
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT ClassName, DefaultFees FROM LicenseClasses WHERE LicenseClassID = @LicenseClassID";
            SqlCommand Command = new SqlCommand(query, Connection);
            Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                Connection.Open();
                SqlDataReader reader = Command.ExecuteReader();

                if (reader.Read())
                {
                    ClassName = reader["ClassName"].ToString();
                    DefaultFees = Convert.ToDecimal(reader["DefaultFees"]);
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
    }
}