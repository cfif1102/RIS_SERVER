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

        public object DownloadChunk(ClientRequest request)
        {
            var downloadChunkDto = DtoValidator.Validate<DownloadFileChunkDto>(request.Data.ToString());
            var file = _fileService.FindById(downloadChunkDto.FileId);

            var result = _fileHelper.HandleFileRead(file.Path, downloadChunkDto.Offset, 10 * 1024 * 1024);

            return new
            {
                Action = "file/download-chunk-response",
                Data = new
                {
                    Offset = result.newOffset,
                    Chunk = result.base64Chunk,
                    IsOver = result.isOver
                }
            };
        }

        public object UploadChunk(ClientRequest request)
        {
            var uploadChunkDto = DtoValidator.Validate<UploadFileChunkDto>(request.Data.ToString());

            _fileHelper.HandleFileWrite(uploadChunkDto.Chunk, uploadChunkDto.FileName);

            return new
            {
                Action = "file/upload-chunk-response"
            };
        }

        public object UploadFullFile(ClientRequest request)
        {
            var uploadFullDto = DtoValidator.Validate<UploadFullFileDto>(request.Data.ToString());
            string filePath = _fileHelper.CombinePath(uploadFullDto.FileName);

            uploadFullDto.FileName = filePath;

            var file = _fileService.Create(uploadFullDto);

            return new
            {
                Action = "file/upload-full-response",
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
    }
}
