namespace OrderManager.WebApi.Models.User;

public class UpdateUserResponseDto
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}
