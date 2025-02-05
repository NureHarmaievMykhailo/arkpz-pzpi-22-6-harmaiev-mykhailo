using Microsoft.EntityFrameworkCore;
using RoadMonitoringSystem.Data;
using RoadMonitoringSystem.DTO;
using RoadMonitoringSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoadMonitoringSystem.Services
{
    public interface IRoadSectionService
    {
        Task<IEnumerable<RoadSection>> GetAllRoadSectionsAsync();
        Task<RoadSection?> GetRoadSectionByIdAsync(int id);
        Task<RoadSection> CreateRoadSectionAsync(RoadSection roadSection);
        Task<bool> UpdateRoadSectionAsync(int id, RoadSectionDto roadSectionDto);
        Task<bool> DeleteRoadSectionAsync(int id);
    }

    public class RoadSectionService : IRoadSectionService
    {
        private readonly ApplicationDbContext _context;

        public RoadSectionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoadSection>> GetAllRoadSectionsAsync()
        {
            return await _context.RoadSections.Include(r => r.Sensors).ToListAsync();
        }

        public async Task<RoadSection?> GetRoadSectionByIdAsync(int id)
        {
            return await _context.RoadSections
                .Include(r => r.Sensors)
                .FirstOrDefaultAsync(r => r.RoadSectionID == id);
        }

        public async Task<RoadSection> CreateRoadSectionAsync(RoadSection roadSection)
        {
            roadSection.CreatedDate = DateTime.UtcNow;

            _context.RoadSections.Add(roadSection);
            await _context.SaveChangesAsync();

            return roadSection;
        }


        public async Task<bool> UpdateRoadSectionAsync(int id, RoadSectionDto roadSectionDto)
        {
            var roadSection = await _context.RoadSections.FindAsync(id);
            if (roadSection == null)
            {
                return false;
            }

            // Оновлення полів
            roadSection.Name = roadSectionDto.Name;
            roadSection.Location = roadSectionDto.Location;
            roadSection.CreatedDate = roadSectionDto.CreatedDate;

            _context.Entry(roadSection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.RoadSections.AnyAsync(r => r.RoadSectionID == id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> DeleteRoadSectionAsync(int id)
        {
            var roadSection = await _context.RoadSections.FindAsync(id);
            if (roadSection == null)
            {
                return false;
            }

            _context.RoadSections.Remove(roadSection);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
