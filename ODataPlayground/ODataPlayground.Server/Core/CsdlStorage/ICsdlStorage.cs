using System.Threading.Tasks;

namespace Portal.Core.CsdlStorage
{
    public interface ICsdlStorage
    {
        Task<string> PutAsync(byte[] data);

        Task<byte[]> GetAsync(string identifier);
    }
}
