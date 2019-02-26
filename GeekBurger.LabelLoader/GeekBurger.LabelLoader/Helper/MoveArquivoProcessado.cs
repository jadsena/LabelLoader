using GeekBurger.LabelLoader.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GeekBurger.LabelLoader.Helper
{
    public class MoveArquivoProcessado
    {
        private string _path;
        private Exception _exception;
        private LabelImagesOptions _options;

        public MoveArquivoProcessado(string path, LabelImagesOptions option) : this(path, option, null)
        {
        }
        public MoveArquivoProcessado(string path, LabelImagesOptions option, Exception exception)
        {
            _path = path;
            _exception = exception;
            _options = option;
        }
        /// <summary>
        /// Move o arquivo processado para as pastas corretas
        /// </summary>
        /// <returns>bool</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public bool Mover()
        {
            bool bRet = false;
            string fileName = FileName();


            File.Move(_path, fileName);
            if (_exception != null)
            {
                File.AppendAllText(Path.Combine(_options.Processados, Path.GetFileNameWithoutExtension(fileName) + ".err"), $"Erro ao enviar o arquivo.{Environment.NewLine}{_exception.ToString()}");
            }

            return bRet;
        }
        /// <summary>
        /// Calcula o nome do arquivo no diretorio destino
        /// </summary>
        /// <returns>Nome do arquivo no diretorio destino</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private string FileName()
        {
            string baseFile = Path.GetFileNameWithoutExtension(_path);
            string baseExtension = Path.GetExtension(_path);
            string basePath = Path.Combine(_options.Processados, $"{baseFile}{baseExtension}");
            if (!File.Exists(basePath)) return basePath;
            string[] a = Directory.GetFiles(Path.GetDirectoryName(basePath),$"{baseFile}.*{baseExtension}");

            basePath = Path.Combine(_options.Processados, $"{baseFile}.{a.Length}{baseExtension}");

            return basePath;
        }

    }
}
