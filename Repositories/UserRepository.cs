using Blog.Data;
using Blog.Models;
using Blog.Repositories.Interfaces;
using Blog.Shared.Repositories;
using Ether.Outcomes;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Repositories;

public class UserRepository : IRepository<User>, IUserRepository
{
    private readonly BlogDataContext _context;

    public UserRepository(BlogDataContext context)
    {
        _context = context;
    }

    public async Task<IOutcome<User>> CreateAsync(User requestModel)
    {
        if(UserExists(requestModel.Email))
            return Outcomes
                .Failure<User>()
                .WithMessage("User already exists.");

        var user = new User
        {
            Name = requestModel.Name,
            Email = requestModel.Email,
            Slug = requestModel.Email
                    .Replace(oldValue: "@", newValue: "-")
                    .Replace(oldValue: ".", newValue: "-")
        };

        var password = PasswordGenerator.Generate(
            length: 25,
            includeSpecialChars: true,
            upperCase: false);
        
        user.PasswordHash = PasswordHasher.Hash(password: password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Outcomes
            .Success<User>()
            .WithValue(user)
            .WithMessage("User registered");
    }

    public async Task<IOutcome<IEnumerable<User>>> GetAllAsync(int page, int pageSize)
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        if(users is null)
            return Outcomes
                .Failure<IEnumerable<User>>()
                .WithMessage("No users found.");

        return Outcomes
            .Success<IEnumerable<User>>()
            .WithValue(users)
            .WithMessage("Users found succefully.");
    }

    public bool UserExists(string email)
        => _context.Users
            .AsNoTracking()
            .Any(user => user.Email == email);
}
