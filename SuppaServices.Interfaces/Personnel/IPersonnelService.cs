using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SuppaServices.Interfaces.Personnel
{
    public interface IPersonnelService
    {
        Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries(string searchString);

        Task<PersonnelEntry> GetPersonnelEntry(int personnelId);

        Task<int> AddPersonnelEntry(PersonnelEntry personnelEntry);

        Task UpdatePersonnelEntry(PersonnelEntry personnelEntry);
    }

}
