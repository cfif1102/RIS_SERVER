using RIS_SERVER.src.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public class FileHelper
    {
        public string HandleFileWrite(string base64, string fileName)
        {
            try
            {
                byte[] fileBytes = Convert.FromBase64String(base64);

                string mediaDirectory = Path.Combine(AppContext.BaseDirectory, "media");

                if (!Directory.Exists(mediaDirectory))
                {
                    Directory.CreateDirectory(mediaDirectory);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                string filePath = Path.Combine(mediaDirectory, uniqueFileName);

                File.WriteAllBytes(filePath, fileBytes);

                return filePath;
            }
            catch
            {
                throw new WsException(400, "Can't write file...");
            }
        }

        public string HandleFileRead(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new WsException(400, "No file found...");
            }

            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                string base64String = Convert.ToBase64String(fileBytes);

                return base64String;
            }
            catch
            {
                throw new WsException(400, "Can't read file...");
            }
        }

        public void RemoveFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new WsException(400, "No file found...");
            }

            try
            {
                File.Delete(filePath);
            } 
            catch
            {
                throw new WsException(400, "Can't delete file...");
            }
        }
    }
}
