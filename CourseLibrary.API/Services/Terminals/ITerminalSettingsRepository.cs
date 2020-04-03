using Siteminder.API.Entities;
using Siteminder.API.Helper;
using Siteminder.API.ResourceParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Siteminder.API.Services
{
    public interface ITerminalSettingsRepository
    {
        TerminalSettings GetTerminalSettings(Guid terminalId);
        public PagedList<TerminalSettings> GetTerminalSettings(TerminalSettingsParameters terminalSettingsParameters);
        void AddTerminalSettings(TerminalSettings terminalSettings);
        void AddTerminalSettings(Guid terminalId, TerminalSettings terminalSettings);
        void UpdateTerminalSettings(Guid terminalId, TerminalSettings terminalSettings);
        void DeleteTerminalSettings(TerminalSettings terminalSettings);
        bool TerminalSettingExists(Guid terminalId);
        bool TerminalHasFillupOption(Guid terminalId);
        bool Save();
    }
}
