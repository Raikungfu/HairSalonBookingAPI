using BusinessObject.Model;
using Repository.IRepository;
using Repository.Repository;


namespace Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private IUserRepository _userRepository;
        private IHairServiceRepository _hairServiceRepository;

        private HairSalonBookingContext _context;

        public UnitOfWork()
        {
            _userRepository ??= new UserRepository();
            _hairServiceRepository ??= new HairServiceRepository();
        }

        public IUserRepository UserRepository
        {
            get
            {
                return _userRepository ??= new UserRepository(_context);
            }
        }

        public IHairServiceRepository HairServiceRepository
        {
            get
            {
                return _hairServiceRepository ??= new HairServiceRepository(_context);
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}