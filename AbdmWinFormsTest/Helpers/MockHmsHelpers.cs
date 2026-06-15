using System;
using System.Data;

namespace HMS.abdm
{
    public static class MyClass
    {
        public static DataTable GetDataTable(string query, CommandType cmdType, object? parameters)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("AbhaNumber", typeof(string));
            dt.Columns.Add("AbhaAddress", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Gender", typeof(string));
            dt.Columns.Add("DOB", typeof(string));
            dt.Columns.Add("Mobile", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("City", typeof(string));
            dt.Columns.Add("State", typeof(string));
            dt.Columns.Add("FatherName", typeof(string));

            // Add a mock scan-share patient record
            dt.Rows.Add(1, "Pending", "99-9999-9999-99", "testpatient@sbx", "Ramesh Kumar", "Male", "1990", "8683916682", "123 Main St", "Delhi", "Delhi", "Suresh Kumar");
            return dt;
        }

        public static void Execute(string query, CommandType cmdType, object? parameters)
        {
            // Mock Execute
        }
    }

    public static class SessionStore
    {
        public static object Create(object? arg1, string key, object value)
        {
            return new object();
        }

        public static void Save(object session)
        {
            // Mock Save
        }
    }
}
