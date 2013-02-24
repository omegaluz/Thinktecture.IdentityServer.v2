namespace Thinktecture.IdentityServer.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExternalProvidersOAuthMembershipClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExternalProvidersOAuthMemberships",
                c => new
                    {
                        Provider = c.String(nullable: false, maxLength: 128),
                        ProviderUserId = c.String(nullable: false, maxLength: 128),
                        UserId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Provider, t.ProviderUserId });
            
            //DropTable("dbo.GlobalConfiguration");
            //DropTable("dbo.WSFederationConfiguration");
            //DropTable("dbo.KeyMaterialConfiguration");
            //DropTable("dbo.WSTrustConfiguration");
            //DropTable("dbo.FederationMetadataConfiguration");
            //DropTable("dbo.OAuth2Configuration");
            //DropTable("dbo.SimpleHttpConfiguration");
            //DropTable("dbo.DiagnosticsConfiguration");
            //DropTable("dbo.ClientCertificates");
            //DropTable("dbo.Delegation");
            //DropTable("dbo.RelyingParties");
            //DropTable("dbo.IdentityProvider");
            //DropTable("dbo.Client");
            //DropTable("dbo.CodeToken");
        }
        
        public override void Down()
        {
            //CreateTable(
            //    "dbo.CodeToken",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Code = c.String(),
            //            ClientId = c.Int(nullable: false),
            //            UserName = c.String(),
            //            Scope = c.String(),
            //            Type = c.Int(nullable: false),
            //            TimeStamp = c.DateTime(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Client",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            Description = c.String(nullable: false),
            //            ClientId = c.String(nullable: false),
            //            ClientSecret = c.String(nullable: false),
            //            RedirectUri = c.String(),
            //            AllowRefreshToken = c.Boolean(nullable: false),
            //            AllowImplicitFlow = c.Boolean(nullable: false),
            //            AllowResourceOwnerFlow = c.Boolean(nullable: false),
            //            AllowCodeFlow = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.IdentityProvider",
            //    c => new
            //        {
            //            ID = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            DisplayName = c.String(nullable: false),
            //            Type = c.Int(nullable: false),
            //            ShowInHrdSelection = c.Boolean(nullable: false),
            //            WSFederationEndpoint = c.String(),
            //            IssuerThumbprint = c.String(),
            //            ClientID = c.String(),
            //            ClientSecret = c.String(),
            //            OAuth2ProviderType = c.Int(),
            //            Enabled = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "dbo.RelyingParties",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Name = c.String(nullable: false),
            //            Enabled = c.Boolean(nullable: false),
            //            Realm = c.String(nullable: false),
            //            TokenLifeTime = c.Int(nullable: false),
            //            ReplyTo = c.String(),
            //            EncryptingCertificate = c.String(),
            //            SymmetricSigningKey = c.String(),
            //            ExtraData1 = c.String(),
            //            ExtraData2 = c.String(),
            //            ExtraData3 = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.Delegation",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserName = c.String(nullable: false),
            //            Realm = c.String(nullable: false),
            //            Description = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.ClientCertificates",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            UserName = c.String(nullable: false),
            //            Thumbprint = c.String(nullable: false),
            //            Description = c.String(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.DiagnosticsConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            EnableFederationMessageTracing = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.SimpleHttpConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Enabled = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.OAuth2Configuration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Enabled = c.Boolean(nullable: false),
            //            EnableConsent = c.Boolean(nullable: false),
            //            EnableResourceOwnerFlow = c.Boolean(nullable: false),
            //            EnableImplicitFlow = c.Boolean(nullable: false),
            //            EnableCodeFlow = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.FederationMetadataConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Enabled = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.WSTrustConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Enabled = c.Boolean(nullable: false),
            //            EnableMessageSecurity = c.Boolean(nullable: false),
            //            EnableMixedModeSecurity = c.Boolean(nullable: false),
            //            EnableClientCertificateAuthentication = c.Boolean(nullable: false),
            //            EnableFederatedAuthentication = c.Boolean(nullable: false),
            //            EnableDelegation = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.KeyMaterialConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            SigningCertificateName = c.String(),
            //            DecryptionCertificateName = c.String(),
            //            RSASigningKey = c.String(),
            //            SymmetricSigningKey = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.WSFederationConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            Enabled = c.Boolean(nullable: false),
            //            EnableAuthentication = c.Boolean(nullable: false),
            //            EnableFederation = c.Boolean(nullable: false),
            //            EnableHrd = c.Boolean(nullable: false),
            //            AllowReplyTo = c.Boolean(nullable: false),
            //            RequireReplyToWithinRealm = c.Boolean(nullable: false),
            //            RequireSslForReplyTo = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            //CreateTable(
            //    "dbo.GlobalConfiguration",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            SiteName = c.String(nullable: false),
            //            IssuerUri = c.String(nullable: false),
            //            IssuerContactEmail = c.String(nullable: false),
            //            DefaultWSTokenType = c.String(nullable: false),
            //            DefaultHttpTokenType = c.String(nullable: false),
            //            DefaultTokenLifetime = c.Int(nullable: false),
            //            MaximumTokenLifetime = c.Int(nullable: false),
            //            SsoCookieLifetime = c.Int(nullable: false),
            //            RequireEncryption = c.Boolean(nullable: false),
            //            RequireRelyingPartyRegistration = c.Boolean(nullable: false),
            //            EnableClientCertificateAuthentication = c.Boolean(nullable: false),
            //            EnforceUsersGroupMembership = c.Boolean(nullable: false),
            //            HttpPort = c.Int(nullable: false),
            //            HttpsPort = c.Int(nullable: false),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            DropTable("dbo.ExternalProvidersOAuthMemberships");
        }
    }
}
