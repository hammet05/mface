using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2
{
    public static class Logger
    {
        private static readonly string logPath = @"C:\Logs\errores.log";

        public static void Log(Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(logPath));

                using (StreamWriter writer = new StreamWriter(logPath, true))
                {
                    writer.WriteLine("=======================================");
                    writer.WriteLine("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    writer.WriteLine("Mensaje: " + ex.Message);
                    writer.WriteLine("StackTrace: " + ex.StackTrace);
                    writer.WriteLine("InnerException: " + ex.InnerException?.Message);
                    writer.WriteLine("=======================================");
                    writer.WriteLine();
                }
            }
            catch {  }
        }
    }
}
