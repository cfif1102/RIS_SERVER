using RIS_SERVER.server;
using RIS_SERVER.src.common;
using RIS_SERVER.src.file.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS_SERVER.src.file
{
    public class FileHandler
    {
        private readonly FileService _fileService;
        private readonly FileHelper _fileHelper;

        public FileHandler(FileService fileService, FileHelper fileHelper)
        {
            _fileService = fileService;
            _fileHelper = fileHelper;
        }

        public object Create(ClientRequest request)
        {
            var createFileDto = DtoValidator.Validate<CreateFileDto>(request.Data.ToString());
            var filePath = _fileHelper.HandleFileWrite(createFileDto.File, createFileDto.Name);

            createFileDto.Path = filePath;

            var file = _fileService.Create(createFileDto);

            return new
            {
                Action = "file/create-response",
                Data = file
            };
        }

        public object Update(ClientRequest request)
        {
            var updateFileDto = DtoValidator.Validate<UpdateFileDto>(request.Data.ToString());
            var file = _fileService.Update(updateFileDto.FileId, updateFileDto);

            return new
            {
                Action = "file/update-response",
                Data = file
            };
        }

        public object Delete(ClientRequest request)
        {
            var fileParamsDto = DtoValidator.Validate<FileParamsDto>(request.Data.ToString());
            var file = _fileService.FindById(fileParamsDto.FileId);

            _fileService.Delete(file.Id);
            _fileHelper.RemoveFile(file.Path);

            return new
            {
                Action = "file/delete-response",
                Data = new
                {
                    Message = "File is deleted..."
                }
            };
        }

        public object DownloadFile(ClientRequest request)
        {
            var fileParamsDto = DtoValidator.Validate<FileParamsDto>(request.Data.ToString());
            var file = _fileService.FindById(fileParamsDto.FileId);
            var fileData = _fileHelper.HandleFileRead(file.Path);

            return new
            {
                Action = "file/download-response",
                Data = new
                {
                    file.Name,
                    file.Size,
                    File = fileData
                }
            };
        }
    }
}
