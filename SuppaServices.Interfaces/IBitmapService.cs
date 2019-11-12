using System.Threading.Tasks;

namespace SuppaServices.Interfaces
{
    public interface IBitmapService
    {
        Task<byte[]> Create(byte[] imageBytes, float rotation);
    }
}