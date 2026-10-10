using System.Data;
using DataAccessLayer;


namespace BusinessLayer
{
    public class clsOrderDetails
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int OrderID { get; set; }
        public int PersonID { get; set; }
        public int LicensesClassID { get; set; }
        public byte AdminID { get; set; }
        public int PassedTests { get; set; }
        public enum enOrderStatus { New = 1, Cancelled = 2, Completed = 3 };
        public enOrderStatus Status { get; set; }

        public clsOrderDetails()
        {
            OrderID = -1;
            PersonID = -1;
            LicensesClassID = -1;
            AdminID = 1;
            PassedTests = 0;
            Status = enOrderStatus.New;
            Mode = enMode.AddNew;
        }

        private clsOrderDetails(int orderID, int personID, int LicensesClassID, byte adminID, int passedTests, enOrderStatus status)
        {
            this.OrderID = orderID;
            this.PersonID = personID;
            this.LicensesClassID = LicensesClassID;
            this.AdminID = adminID;
            this.PassedTests = passedTests;
            this.Status = status;
            Mode = enMode.Update;
        }

        public static DataTable GetAllOrders()
        {
            return clsOrderDetailsData.GetAllOrders();
        }

        public static bool DeleteOrderByID(int OrderID)
        {
            return clsOrderDetailsData.DeleteOrderByID(OrderID);
        }

        private bool _UpdateOrder()
        {
            return clsOrderDetailsData.UpdateOrder(this.OrderID, this.PersonID, this.LicensesClassID, this.AdminID, this.PassedTests, (byte)this.Status);
        }

        private bool _AddNewOrder()
        {
            this.OrderID = clsOrderDetailsData.AddNewOrder(this.PersonID, this.LicensesClassID, this.AdminID, this.PassedTests, (byte)this.Status);
            return (this.OrderID != -1);
        }

        // دالة التحقق من الطلب المسبق
        public static bool IsLicenseRequestedBefore(int personID, int LicensesClassID)
        {
            return clsOrderDetailsData.IsLicenseRequestedBefore(personID, LicensesClassID);
        }

        public static clsOrderDetails FindOrderByID(int OrderID)
        {
            int personID = -1;
            int LicensesClassID = -1;
            byte adminID = 1;
            int passedTests = 0;
            byte status = 1;

            if (clsOrderDetailsData.FindOrderByID(OrderID, ref personID, ref LicensesClassID, ref adminID, ref passedTests, ref status))
            {
                return new clsOrderDetails(OrderID, personID, LicensesClassID, adminID, passedTests, (enOrderStatus)status);
            }
            else
            {
                return null;
            }
        }

        public static DataTable GetOrdersByNationalID(string nationalID)
        {
            return clsOrderDetailsData.GetOrdersByNationalID(nationalID);
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewOrder())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateOrder();
            }
            return false;
        }
    }
}