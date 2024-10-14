namespace Rubik.Identity.Oidc.Core.OidcEntities
{
    public class ClientEntity
    {
        public string? ClientID { get; set; }

        public string? ClientSecret { get; set; }

        public string? Scope { get; set; }

        public string[]? ScopeArr => Scope?.Split(' ');

        public string? CallbackPath { get; set; }

        public string? ResponseType { get; set; }

    }
}
