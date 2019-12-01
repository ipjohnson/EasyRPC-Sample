using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuppaServices.Interfaces.Personnel;
using SuppaServices.Server.DataAccess;
using SuppaServices.Server.Repository;

namespace SuppaServices.Server.Services
{
    public class PersonnelService : IPersonnelService
    {
        private readonly IPersonnelRepository _personnelRepository;

        public PersonnelService(IPersonnelRepository personnelRepository)
        {
            _personnelRepository = personnelRepository;
        }

        public Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries(string searchString)
        {
            return _personnelRepository.GetPersonnelListEntries(searchString);
        }

        public Task<PersonnelEntry> GetPersonnelEntry(int personnelId)
        {
            return _personnelRepository.GetPersonnelEntry(personnelId);
        }

        [Transaction]
        public Task<int> AddPersonnelEntry(PersonnelEntry personnelEntry)
        {
            return _personnelRepository.AddPersonnelEntry(personnelEntry);
        }

        [Transaction]
        public Task UpdatePersonnelEntry(PersonnelEntry updateEntry)
        {
            return _personnelRepository.UpdatePersonnelEntry(updateEntry);
        }
    }
}
