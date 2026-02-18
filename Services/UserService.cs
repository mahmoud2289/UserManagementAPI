using UserManagementAPI.Models;

namespace UserManagementAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<User?> UpdateUserAsync(int id, User user);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private static List<User> _users = new List<User>
        {
            new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@techhive.com",
                PhoneNumber = "555-0101",
                Department = "Engineering",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new User
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@techhive.com",
                PhoneNumber = "555-0102",
                Department = "Human Resources",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            },
            new User
            {
                Id = 3,
                FirstName = "Mike",
                LastName = "Johnson",
                Email = "mike.johnson@techhive.com",
                PhoneNumber = "555-0103",
                Department = "IT",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            }
        };

        private int _nextId = 4;

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return Task.FromResult<IEnumerable<User>>(_users.OrderBy(u => u.Id).ToList());
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User> CreateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User object cannot be null");

            // Additional validation at service level
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber) ||
                string.IsNullOrWhiteSpace(user.Department))
            {
                throw new ArgumentException("All user fields are required and cannot be empty or whitespace.");
            }

            // Check for duplicate email
            if (_users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"A user with email '{user.Email}' already exists.");

            user.Id = _nextId++;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            _users.Add(user);

            return Task.FromResult(user);
        }

        public Task<User?> UpdateUserAsync(int id, User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User object cannot be null");

            // Validate required fields
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.Email) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber) ||
                string.IsNullOrWhiteSpace(user.Department))
            {
                throw new ArgumentException("All user fields are required and cannot be empty or whitespace.");
            }

            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            
            if (existingUser == null)
                return Task.FromResult<User?>(null);

            // Check if email is being changed to one that already exists
            if (!existingUser.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase) &&
                _users.Any(u => u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"A user with email '{user.Email}' already exists.");
            }

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Department = user.Department;
            existingUser.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<User?>(existingUser);
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            
            if (user == null)
                return Task.FromResult(false);

            _users.Remove(user);
            return Task.FromResult(true);
        }
    }
}
