using Microsoft.AspNetCore.Http;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public interface IFilesService
    {
        public List<File> UploadMutliple(List<IFormFile> files, int userId);
        int Create(FileAddRequest model, int userId);
        Paged<File> SelectAll(int pageIndex, int pageSize);
        File SelectById(int id);
        Paged<File> SelectByCreatedBy(int createdBy, int pageIndex, int pageSize);
    }
}
