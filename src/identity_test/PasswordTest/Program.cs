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

    var userinfo = await client.GetUserInfoAsync(new UserInfoRequest
    {
        Address= disco.UserInfoEndpoint,
        Token= "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI4MDI0MjIxIiwic2NvcGUiOiJhcGkudGVzdC5zY29wZTEiLCJyb2xlIjoiIiwiam9iIjoiU3IuU29mdHdhcmUgRW5naW5lZXIiLCJhdWQiOlsibXZjX2NsaWVudCIsImFwaS50ZXN0Il0sImV4cCI6MTc0NjgwMzczNywiaXNzIjoicnViaWsub2lkYyJ9.IYPPameIEKlMoqZ9WyqzucpBo1Fiow9LKDQZq8I0diQ0ilbFM1SYIjvTpenXAhhUoYvXZQSu6w9U3dhlt2zofCfxFZp6h-pXsx1oeYMVl8PmdFGAD6eaGdg1j4gKriCNAoL2mrOC0CeaVC3o_txfPBci2rDc17MAjRlrDgHWZZRU9JmFX6KDG88CQoQyifUP8yll536Ugk1UY4TTTQV83AwxxPhgPr5v5cH04wsGX21mnPvPhhQTBrWRqJvCGh_K9elGrsSvdXcziCbGGH9LuzUW17Cmt0tESOizRfP3MdA-QOMmEzZboHQQGwNlul2ngOlIFj__J96BufT1MPHFPA"
    });

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
        Console.WriteLine(token.AccessToken);
}








