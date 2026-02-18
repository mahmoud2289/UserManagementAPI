using System.Text.RegularExpressions;
using UserManagementAPI.Models;

namespace UserManagementAPI.Validators
{
    public class UserValidator
    {
        private const string EmailPattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        private const string PhonePattern = @"^\d{3}-\d{4}$|^\d{10}$|^\+?[\d\s\-()]{10,}$";

        public static (bool IsValid, string? ErrorMessage) ValidateUser(User user)
        {
            if (user == null)
                return (false, "User object cannot be null");

            // Check required fields
            if (string.IsNullOrWhiteSpace(user.FirstName))
                return (false, "FirstName is required");

            if (string.IsNullOrWhiteSpace(user.LastName))
                return (false, "LastName is required");

            if (string.IsNullOrWhiteSpace(user.Email))
                return (false, "Email is required");

            if (string.IsNullOrWhiteSpace(user.PhoneNumber))
                return (false, "PhoneNumber is required");

            if (string.IsNullOrWhiteSpace(user.Department))
                return (false, "Department is required");

            // Validate FirstName length
            if (user.FirstName.Length > 100)
                return (false, "FirstName cannot exceed 100 characters");

            // Validate LastName length
            if (user.LastName.Length > 100)
                return (false, "LastName cannot exceed 100 characters");

            // Validate email format
            if (!Regex.IsMatch(user.Email, EmailPattern))
                return (false, "Email format is invalid");

            if (user.Email.Length > 255)
                return (false, "Email cannot exceed 255 characters");

            // Validate phone number format
            if (!Regex.IsMatch(user.PhoneNumber, PhonePattern))
                return (false, "PhoneNumber format is invalid (e.g., 555-0101 or +1-234-567-8900)");

            if (user.PhoneNumber.Length > 20)
                return (false, "PhoneNumber cannot exceed 20 characters");

            // Validate Department length
            if (user.Department.Length > 100)
                return (false, "Department cannot exceed 100 characters");

            return (true, null);
        }
    }
}
