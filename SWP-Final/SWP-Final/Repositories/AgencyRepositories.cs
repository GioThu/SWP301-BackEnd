using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP_Final.Entities;
using SWP_Final.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SWP_Final.Repositories
{
    public class AgencyRepositories : IAgencyRepositories
    {
        private readonly RealEasteSWPContext _context;
        private readonly IMapper _mapper;

        public AgencyRepositories(RealEasteSWPContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AgencyModel>> GetAllAgenciesAsync()
        {
            var agencies = await _context.Agencies.ToListAsync();
            return _mapper.Map<List<AgencyModel>>(agencies);
        }

        public async Task<AgencyModel> GetAgencyByIdAsync(string id)
        {
            var agency = await _context.Agencies.FindAsync(id);
            return _mapper.Map<AgencyModel>(agency);
        }

        public async Task AddAgencyAsync(AgencyModel agency)
        {
            var newAgency = _mapper.Map<Agency>(agency);
            _context.Agencies.Add(newAgency);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAgencyAsync(string id, AgencyModel agencyModel)
        {
            var agency = await _context.Agencies.FindAsync(id);
            if (agency != null)
            {
                _mapper.Map(agencyModel, agency);
                _context.Agencies.Update(agency);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAgencyAsync(string id)
        {
            var agency = await _context.Agencies.FindAsync(id);
            if (agency != null)
            {
                _context.Agencies.Remove(agency);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<AgencyModel>> GetListAgencyByNameAsync(string name)
        {
            // Chuyển đổi tên cần tìm kiếm thành chữ thường để so sánh không phân biệt chữ hoa chữ thường
            string searchName = name.ToLower();

            // Tìm kiếm danh sách đại lý dựa trên tên (FirstName hoặc LastName)
            var agencies = await _context.Agencies
                .Where(a => a.FirstName.ToLower().Contains(searchName) || a.LastName.ToLower().Contains(searchName))
                .ToListAsync();

            // Ánh xạ danh sách đại lý thành danh sách AgencyModel và trả về
            return _mapper.Map<List<AgencyModel>>(agencies);
        }

    }
}
