using DataLayer.Entities.Files;

namespace Core.Files
{
    public interface IFileService
    {
        Task<PDF> SaveFileAsync();
    }
}
