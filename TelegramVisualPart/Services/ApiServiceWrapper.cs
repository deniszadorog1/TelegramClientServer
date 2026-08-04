using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegramLib.Interfaces;

namespace TelegramVisualPart.Services
{
    public class ApiServiceWrapper : IApiService
    {
        public async Task<byte[]> GetFileBytesAsync(string fileName)
        {
            //Just a check

            const int checkIndex = 12;

            return new byte[checkIndex];
        }
    }
}
