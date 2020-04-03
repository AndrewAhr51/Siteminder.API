using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface ITerminalRepository
    {

        Terminal GetTerminal(Guid siteId, Guid terminalId);
        public PagedList<Terminal> GetTerminals(TerminalResourceParameters terminalResourseParameters);
        void AddTerminal(Terminal terminal);
        void AddTerminal(Guid siteId, Guid terminalId, Terminal terminal);
        void UpdateTerminal(Guid terminalId, Terminal terminal);
        void DeleteTerminal(Terminal terminal);
        bool TerminalExists(Guid terminalId);
        bool Save();
    }
}
