namespace cmsUserManagment.Application.DTO;

public class UpdateUserDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsAdmin { get; set; }
}
