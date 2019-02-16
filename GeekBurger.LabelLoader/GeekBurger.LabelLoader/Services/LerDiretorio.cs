using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeekBurger.LabelLoader.Services
{
    public class LerDiretorio : ILerDiretorio
    {
        ILogger<LerDiretorio> Logger { get; }
        public LerDiretorio(ILogger<LerDiretorio> logger)
        {
            Logger = logger;
        }
        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}
