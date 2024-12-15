using RIS_SERVER.src.common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.server
{
    public class FileHelper
    {
        public string CombinePath(string fileName)
        {
            string mediaDirectory = Path.Combine(AppContext.BaseDirectory, "media");

            if (!Directory.Exists(mediaDirectory))
            {
                Directory.CreateDirectory(mediaDirectory);
            }

            string filePath = Path.Combine(mediaDirectory, fileName);

            return filePath;
        }

        public void HandleFileWrite(string base64, string fileName)
        {
            try
            {
                byte[] fileBytes = Convert.FromBase64String(base64);
                string filePath = CombinePath(fileName);

                using (var stream = new FileStream(filePath, FileMode.Append))
                {
                    stream.Write(fileBytes, 0, fileBytes.Length);
                }
            }
            catch
            {
                throw new WsException(400, "Can't write file...");
            }
        }

        public (long newOffset, bool isOver, string base64Chunk) HandleFileRead(string filePath, long offset, int chunkSize)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл {filePath} не найден.");
            }

            byte[] buffer = new byte[chunkSize];

            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (offset >= fs.Length)
                    {
                        return (0, true, "");
                    }

                    fs.Seek(offset, SeekOrigin.Begin);

                    int bytesRead = fs.Read(buffer, 0, chunkSize);

                    if (bytesRead < chunkSize)
                    {
                        Array.Resize(ref buffer, bytesRead);
                    }

                    long newOffset = offset + bytesRead;
                    bool isOver = newOffset >= fs.Length;

                    string base64Chunk = Convert.ToBase64String(buffer);

                    return (newOffset, isOver, base64Chunk);
                }
            }
            catch (Exception ex)
            {
                throw new WsException(400, "No file found...");
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
