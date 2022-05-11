using System.Net;
using System.Text.RegularExpressions;
using Blog.Data;
using Blog.Dtos.UserDtos;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers;

[ApiController]
[Route("/api/user/")]
[Authorize]

public class UserController : HomeController
{
    [HttpPost]
    [Route("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegisterUserDto modelRequest,
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context)
    {
        if(!ModelState.IsValid)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                ModelState.GetErrors());

        var isInvalidUser = await context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Email == modelRequest.Email);

        if(isInvalidUser)
            return FailureResponse(
                statusCode: (int)HttpStatusCode.BadRequest,
                message: "Email already registered. Please, enter a valid email.");

        var user = new User
        {
            Name = modelRequest.Name,
            Email = modelRequest.Email,
            Slug = modelRequest.Email
                    .Replace(oldValue: "@", newValue: "-")
                    .Replace(oldValue: ".", newValue: "-")
        };

        var password = PasswordGenerator.Generate(
            length: 25,
            includeSpecialChars: true,
            upperCase: false);
        
        user.PasswordHash = PasswordHasher.Hash(password: password);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // emailService.Send(
        //     toName: user.Name,
        //     toEmail: user.Email,
        //     subject: "Welcome to LP world :)",
        //     body: $"Your password is <strong>{password}</strong>");

        return SuccessResponse<dynamic>(
            (int)HttpStatusCode.OK,
            new { Email = user.Email, password});
    }

    [HttpPost]
    [Route("login")]
    [AllowAnonymous]
    public async Task<IActionResult> AuthenticateAsync(
        [FromBody] AuthenticationDto modelRequest,
        [FromServices] TokenService tokenService,
        [FromServices] BlogDataContext context)
    {
        if(!ModelState.IsValid)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                ModelState.GetErrors());

        var user = await context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(user => user.Email == modelRequest.Email);

        if(user is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "User or Password does not exist. Please, enter a valid User or Password.");

        var isPasswordValid = PasswordHasher.Verify(
            hash: user.PasswordHash,
            password: modelRequest.Password);

        if(!isPasswordValid)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "User or Password does not exist. Please, enter a valid User or Password.");

        var token = tokenService.GenerateToken(user);
        
        return SuccessResponse<dynamic>(
            (int)HttpStatusCode.OK,
            new { token });
    }

    [HttpGet]
    [Route("get-users")]
    public async Task<IActionResult> GetAllAsync(
        [FromServices] BlogDataContext context)
    {
        var users = await context.Users
            .AsNoTracking().ToListAsync();

        if(users is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "No users found.");

        return SuccessResponse<IEnumerable<User>>(
            (int)HttpStatusCode.OK,
            users);
    }

    [HttpGet]
    [Route("get-user/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        int? id,
        [FromServices] BlogDataContext context)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);
        
        if(user is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "No users found with the id specified.");
        
        return SuccessResponse<User>(
            (int)HttpStatusCode.OK,
            user);
    }

    [HttpPut]
    [Route("update-user/{id:int}")]
    public async Task<IActionResult> UpdateByIdAsync(
        int? id,
        EditorUserDto modelRequest,
        [FromServices] BlogDataContext context)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        if(user is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "No users found with the id specified.");
        
        user.Name = modelRequest.Name;
        user.Email = modelRequest.Email;

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return SuccessResponse(
            statusCode: (int)HttpStatusCode.OK,
            message: "User updated.");
    }

    [HttpDelete]
    [Route("delete-user/{id:int}")]
    public async Task<IActionResult> DeleteByIdAsync(
        int? id,
        [FromServices] BlogDataContext context)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id);

        if(user is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "No users found with the id specified.");

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return SuccessResponse(
            statusCode: (int)HttpStatusCode.OK,
            message: "User deleted.");
    }

    [HttpPost]
    [Route("upload-profile-image")]
    public async Task<IActionResult> UploadProfileImageAsync(
        [FromBody] UploadProfileImageDto modelRequest,
        [FromServices] BlogDataContext context)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.jpg";
        
        var data = new Regex(@"^data:image\/[a-z]+;base64,")
            .Replace(
                input: modelRequest.Base64Image,
                replacement: "");

        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync(path: $"wwwroot/images/{fileName}", bytes);
        }
        catch
        {
            return FailureResponse(
                (int)HttpStatusCode.InternalServerError,
                "Internal Server Error.");
        }

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == User.Identity.Name);

        if(user is null)
            return FailureResponse(
                (int)HttpStatusCode.BadRequest,
                "User was not found.");

        user.Image = $"https://localhost:0000/images/{fileName}";

        context.Users.Update(user);
        await context.SaveChangesAsync();

        return SuccessResponse(
            (int)HttpStatusCode.OK,
            "Image successfully modified.");
    }
}