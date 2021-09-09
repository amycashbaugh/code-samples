using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sabio.Data;
using Sabio.Data.Providers;
using Sabio.Models;
using Sabio.Models.AppSettings;
using Sabio.Models.Domain;
using Sabio.Models.Enums;
using Sabio.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = Sabio.Models.Domain.File;

namespace Sabio.Services
{
    public class FilesService : IFilesService
    {
        IDataProvider _dataProvider = null;
        private AppKeys _appKeys;
        public FilesService(IOptions<AppKeys> appKeys, IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _appKeys = appKeys.Value;
        }


        public List<File> UploadMutliple(List<IFormFile> files, int userId)
        {
            List<Models.Domain.File> filesList = new List<File>();
            FileAddRequest fileAddRequest = new FileAddRequest();
            AmazonS3Client client = new AmazonS3Client(_appKeys.AWSAccessKey, _appKeys.AWSSecretKey, RegionEndpoint.USWest2);
            var fileTransferUtility = new TransferUtility(client);

            foreach(IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    string keyName = "kitchmet/" + Guid.NewGuid() + "/" + file.FileName;
                    string bucketName = _appKeys.AWSBucketName;
                    string url = _appKeys.AWSDomain + keyName;

                    fileTransferUtility.UploadAsync(file.OpenReadStream(), bucketName, keyName).Wait();

                    fileAddRequest.Url = url;
                    fileAddRequest.FileTypeId = GetFileType(file.ContentType);
                 

                    int fileId = Create(fileAddRequest, userId);

                    File fileObject = new File()
                    {
                        Id = fileId,
                        Url = url,
                        FileTypeId = fileAddRequest.FileTypeId,
                        CreatedBy = userId
                    };

                    filesList.Add(fileObject);
                }
            }

            return filesList;
        }

        public int GetFileType(string fileType)
        {
            int id = 0;

            switch (fileType)
            {
                case "image/png": 
                    id = (int)FileType.Png;
                    break;
                case "image/jpeg": 
                    id = (int)FileType.Jpeg;
                    break;
                case "application/pdf":
                    id = (int)FileType.Pdf;
                    break;
                default:
                    id = (int)FileType.NotSet;
                    break;
            }
            return id;
        }


        public int Create(FileAddRequest model, int userId)
        {
            int id = 0;
            string procName = "dbo.Files_Insert";

            _dataProvider.ExecuteNonQuery(procName, inputParamMapper: delegate (SqlParameterCollection collection)
            {
                AddCommonParams(model, collection);
                collection.AddWithValue("@CreatedBy",userId);

                SqlParameter idOut = new SqlParameter("@Id", SqlDbType.Int);
                idOut.Direction = ParameterDirection.Output;

                collection.Add(idOut);
            }, returnParameters: delegate (SqlParameterCollection returnCollection)
            {
                object objectId = returnCollection["@Id"].Value;
                int.TryParse(objectId.ToString(), out id);
            }
            );

            return id;
        }

        public Paged<File> SelectAll(int pageIndex, int pageSize)
        {
            Paged<File> pagedList = null;
            List<File> list = null;
            int totalCount = 0;
            string procName = "dbo.Files_SelectAll";


            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
                {
                    paramCollection.AddWithValue("@PageIndex", pageIndex);
                    paramCollection.AddWithValue("@PageSize", pageSize);

                }, (reader, recordSetIndex) =>
                {
                    int idx = 0;
                    File singleFile = MapFile(reader, ref idx);
                    totalCount = reader.GetSafeInt32(idx);

                    if (list == null)
                    {
                        list = new List<File>();
                    }
                    list.Add(singleFile);
                }
                );
            if (list != null)
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }
        public File SelectById(int id)
        {
            string procName = "dbo.Files_SelectById";
            File singleFile = null;

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@Id", id);
            }, delegate (IDataReader reader, short set)
            {
                int idx = 0;
                singleFile = MapFile(reader, ref idx);
            }
            );

            return singleFile;
        }

        public Paged<File> SelectByCreatedBy(int createdBy, int pageIndex, int pageSize)
        {
            Paged<File> pagedList = null;
            List<File> list = null;
            int totalCount = 0;
            string procName = "dbo.Files_SelectByCreatedBy";

            _dataProvider.ExecuteCmd(procName, delegate (SqlParameterCollection paramCollection)
            {
                paramCollection.AddWithValue("@CreatedBy", createdBy);
                paramCollection.AddWithValue("@PageIndex", pageIndex);
                paramCollection.AddWithValue("@PageSize", pageSize);

            }, (reader, recordSetIndex) =>
            {
                int idx = 0;
                File singleFile = MapFile(reader, ref idx);
                totalCount = reader.GetSafeInt32(idx);

                if (list == null)
                {
                    list = new List<File>();
                }
                list.Add(singleFile);
            }
            );
            if (list != null)
            {
                pagedList = new Paged<File>(list, pageIndex, pageSize, totalCount);
            }

            return pagedList;
        }

        private static void AddCommonParams(FileAddRequest model, SqlParameterCollection collection)
        {
            collection.AddWithValue("@Url", model.Url);
            collection.AddWithValue("@FileTypeId", model.FileTypeId);

        }

        private static File MapFile(IDataReader reader, ref int startingIdx)
        {
            File singleFile = new File();

            singleFile.Id = reader.GetSafeInt32(startingIdx++);
            singleFile.Url = reader.GetSafeString(startingIdx++);
            singleFile.FileTypeId = reader.GetSafeInt32(startingIdx++);
            singleFile.CreatedBy = reader.GetSafeInt32(startingIdx++);
            singleFile.DateCreated = reader.GetSafeUtcDateTime(startingIdx++);

            return singleFile;
        }
}
}
