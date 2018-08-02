using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wallet.Helpers
{
    public class Utils
    {

        public static void LogWriter(string msg)
        {
            string logPath = "/logs";
            string fileName = string.Format("/ErrorLog.txt_{0}.txt", DateTime.Today.Date.ToString("MM/dd/yyyy").Replace("/", "_"));
            string message = string.Format("{0} - {1}" + Environment.NewLine, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt",
                                        System.Globalization.CultureInfo.InvariantCulture),  msg);
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);

            if (!File.Exists(logPath+ fileName))
            {
                    using (StreamWriter sw = File.CreateText(logPath + fileName))
                    {  
                        sw.WriteLine(string.Format("{0}", message));
                    }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(logPath + fileName))
                {
                    sw.WriteLine(string.Format("{0}", message));
                }
            }
        }
        }
    }

