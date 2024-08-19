﻿namespace Rubik.Identity.Share.Entity
{
    [Table(Name ="tb_application")]
    [Index("uq_code","Code",IsUnique =true)]
    public class TbApplication:BaseFullEntity
    {
        [Column(IsNullable = true)]
        public string? RedirectUri { get; set; }

        [Column(IsNullable = true)]
        public string? ResponseType { get; set; }

        [Column(IsNullable = true)]
        public string? ClientSecret { get; set; }

        [Column(IsNullable =true)]
        public string? Scope { get; set; }

        [Column(IsNullable = true)]
        public string? CallbackPath { get; set; }

        [Column(IsNullable =false,MapType =typeof(sbyte))]
        public OidcAppType OidcAppType { get; set; }
    }

    public enum OidcAppType
    {
        MVC,
        Client,
        FrontEnd,
        ApiResource
    }
}
