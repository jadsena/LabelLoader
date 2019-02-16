using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public interface IEnviaParaApi
    {
        Task<bool> EnviarAsync(FileInfo arquivo);
    }
}
