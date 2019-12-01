using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SuppaServices.Interfaces.Personnel;
using SuppaServices.Server.DataAccess;

namespace SuppaServices.Server.Repository
{
    public interface IPersonnelRepository
    {
        Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries(string searchString);

        Task<PersonnelEntry> GetPersonnelEntry(int personnelId);

        Task<int> AddPersonnelEntry(PersonnelEntry personnelEntry);

        Task UpdatePersonnelEntry(PersonnelEntry updateEntry);
    }

    public class PersonnelRepository : IPersonnelRepository
    {
        private readonly IConnectionManager _connectionManager;

        public PersonnelRepository(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public async Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries(string searchString)
        {
            using var _ = _connectionManager.GetConnection();

            return await _.Connection.QueryAsync<PersonnelListEntry>("SELECT PersonnelId, FirstName, LastName FROM Personnel WHERE LastName LIKE @searchString || '%' OR FirstName LIKE @searchString || '%';", new { searchString });
        }

        public async Task<PersonnelEntry> GetPersonnelEntry(int personnelId)
        {
            using var _ = _connectionManager.GetConnection();

            return await _.Connection.QueryFirstAsync<PersonnelEntry>("SELECT * FROM Personnel WHERE PersonnelId = @personnelId;", new { personnelId });
        }

        public async Task<int> AddPersonnelEntry(PersonnelEntry personnelEntry)
        {
            using var _ = _connectionManager.GetConnection(transactionRequired: true);

            return await _.Connection.QuerySingleAsync<int>(
                "INSERT INTO Personnel VALUES (NULL, @FirstName, @LastName, @DateOfBirth, @EmploymentDate); SELECT last_insert_rowid();", personnelEntry, _.Transaction);
        }

        public async Task UpdatePersonnelEntry(PersonnelEntry personnelEntry)
        {
            using var _ = _connectionManager.GetConnection(transactionRequired: true);

            await _.Connection.ExecuteAsync("UPDATE Personnel SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, EmploymentDate = @EmploymentDate WHERE PersonnelId = @PersonnelId;", personnelEntry, _.Transaction);
        }
    }
}
