using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SuppaServices.Interfaces.Personnel;

namespace SuppaServices.Server.DataAccess
{
    public interface IPersonnelRepository
    {
        Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries();

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

        public async Task<IEnumerable<PersonnelListEntry>> GetPersonnelListEntries()
        {
            using var c = _connectionManager.GetConnection();

            return await c.Connection.QueryAsync<PersonnelListEntry>("SELECT PersonnelId, FirstName, LastName FROM Personnel");
        }

        public async Task<PersonnelEntry> GetPersonnelEntry(int personnelId)
        {
            using var c = _connectionManager.GetConnection();

            return await c.Connection.QueryFirstAsync<PersonnelEntry>("SELECT * FROM Personnel WHERE PersonnelId = @personnelId", new { personnelId });
        }

        public async Task<int> AddPersonnelEntry(PersonnelEntry personnelEntry)
        {
            using var c = _connectionManager.GetConnection();

            return await c.Connection.QuerySingleAsync<int>(
                "INSERT INTO Personnel VALUES (NULL, @FirstName,@LastName,@DateOfBirth,@EmploymentDate); SELECT last_insert_rowid();", personnelEntry, c.Transaction);
        }

        public async Task UpdatePersonnelEntry(PersonnelEntry personnelEntry)
        {
            using var _ = _connectionManager.GetConnection();

            await _.Connection.ExecuteAsync("UPDATE Personnel SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, EmploymentDate = @EmploymentDate WHERE PersonnelId = @PersonnelId", personnelEntry, _.Transaction);
        }
    }
}
