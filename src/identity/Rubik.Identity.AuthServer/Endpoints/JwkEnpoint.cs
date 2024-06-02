
namespace Server.Endpoints
{
    public class JwkEnpoint
    {
        public static IResult GetJwks(DevKeys devKeys)
        {
            return Results.Content(devKeys.JwkJson);
        }

    }


}
