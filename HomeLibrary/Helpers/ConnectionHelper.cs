using System;
using System.Collections.Generic;
using System.Text;

namespace HomeLibrary.Helpers
{
    public static class ConnectionHelper
    {
        // Замените на имя вашего экземпляра SQL Server Local
        public static string ConnectionString { get; } =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=HomeLibrary;Integrated Security=True;TrustServerCertificate=True";
    }
}
