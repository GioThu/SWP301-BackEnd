using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SWP_Final.Models;

namespace SWP_Final.Repositories
{
    public interface IAgencyRepositories
    {
        Task<List<AgencyModel>> GetAllAgenciesAsync();
        Task<AgencyModel> GetAgencyByIdAsync(string id);
        Task<List<AgencyModel>> GetListAgencyByNameAsync(string name);
        Task AddAgencyAsync(AgencyModel agency);
        Task DeleteAgencyAsync(string id);
        Task UpdateAgencyAsync(string id, AgencyModel agencyModel);
    }
}
