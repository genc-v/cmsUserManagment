namespace cmsUserManagment.Application.Common.ErrorCodes;

public class GeneralErrorCodes(int code, string message) : Exception
{
    public int code = code;
    public string message = message;

    public static readonly GeneralErrorCodes notFound = new(1, "User not found");

}
