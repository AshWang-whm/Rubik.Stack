using IdentityModel.Client;

while (true)
{
    Console.WriteLine("任意键开始!");
    Console.ReadKey();


    using var client = new HttpClient();

    var disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
    {
        Address = "http://localhost:5000",
    });

    if (disco.IsError)
    {
        Console.WriteLine(disco.Error);
        return;
    }

    var token = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
    {
        Address = disco.TokenEndpoint,
        // ClientID 和 Secret 未发送到 auth server ??
        ClientId = "console_password_test",
        ClientSecret = "client_password_test_client_password_test",
        Scope = "openid profile scope1",
        UserName = "8024221",
        Password = "8024221",
        
        ClientCredentialStyle= ClientCredentialStyle.AuthorizationHeader,
    });

    if (token.IsError)
    {
        Console.WriteLine(token.Error);
    }
    else
    {
        Console.WriteLine($"access token:[{token.AccessToken}]");
        Console.WriteLine($"------------------------");
        Console.WriteLine($"------------------------");
        Console.WriteLine($"id token:[{token.IdentityToken}]");

        Console.WriteLine($"------------------------");
        Console.WriteLine($"------------------------");

        var userinfo = await client.GetUserInfoAsync(new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token = token.AccessToken
        });

        if (userinfo.IsError)
        {
            Console.WriteLine(userinfo.Error);
        }
        else
        {
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(userinfo.Json));
        }
    }
}








