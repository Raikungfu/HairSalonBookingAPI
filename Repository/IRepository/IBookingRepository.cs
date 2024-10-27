using BusinessObject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<Booking> GetBookingByIdAsync(int id);
        Task<int> CreateBookingAsync(Booking entity);

        Task<List<Booking>> GetBookingHistoryByCustomerIdAsync(int customerId);
        Task<List<Booking>> GetBookingByStylistIdAsync(int stylistId);
    }
}
