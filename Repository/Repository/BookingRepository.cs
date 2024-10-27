using BusinessObject.Model;
using Microsoft.EntityFrameworkCore;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository() { }

        public BookingRepository(HairSalonBookingContext context) => _context = context;

        public async Task<Booking?> GetBookingByIdAsync(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(u => u.BookingId == id);
        }

        public async Task<int> CreateBookingAsync(Booking entity)
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetBookingHistoryByCustomerIdAsync(int customerId)
        {
            return await _context.Bookings
                .Where(b => b.CustomerId == customerId)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Service)
                .Include(b => b.BookingDetails)  
                    .ThenInclude(bd => bd.Stylist)
                .Include(b => b.Schedule)
                .ToListAsync();
        }

        public async Task<List<Booking>> GetBookingByStylistIdAsync(int stylistId)
        {
            var result = await _context.Bookings
                .Where(b => b.BookingDetails.Any(bd => bd.StylistId == stylistId))
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Service)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Stylist)
                .Include(b => b.Schedule)
                .ToListAsync();
            return result;
        }
    }
}
