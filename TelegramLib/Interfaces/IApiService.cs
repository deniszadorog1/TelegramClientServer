using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramLib.Interfaces
{
    public interface IApiService
    {
        Task<byte[]> GetFileBytesAsync(string fileName);
    }
}
