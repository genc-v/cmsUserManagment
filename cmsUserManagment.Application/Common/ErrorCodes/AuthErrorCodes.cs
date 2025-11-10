namespace cmsUserManagment.Application.Common.ErrorCodes;

public class AuthErrorCodes : Exception
{
    public int code;
    public string message;

    public AuthErrorCodes(int code, string message)
    {
        this.code = code;
        this.message = message;
    }

    public static readonly AuthErrorCodes tokenNotFound = new(1, "Token not found");
    public static readonly AuthErrorCodes notCorrectCode = new(2, "Code is not correct");

}
