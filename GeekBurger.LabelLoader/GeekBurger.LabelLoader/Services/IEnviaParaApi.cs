using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GeekBurger.LabelLoader.Services
{
    public interface IEnviaParaApi
    {
        /// <summary>
        /// Enviar arquivo para API
        /// </summary>
        /// <param name="arquivo">Dados do arquivo que será enviado</param>
        /// <returns>bool</returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.PathTooLongException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        Task<bool> EnviarAsync(FileInfo arquivo);
    }
}
