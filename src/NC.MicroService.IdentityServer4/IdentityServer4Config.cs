using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NC.MicroService.IdentityServer4
{
    /// <summary>
    /// IdentityServer4 测试配置
    /// </summary>
    public class IdentityServer4Config
    {
        /// <summary>
        /// 1. 微服务API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TeamService", "TeamService api需要被保护")
            };
        }

        /// <summary>
        /// 2. 客户端资源
        /// 配置允许访问IdentityServer的客户端列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientId = "client",

                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    // 1、client认证模式
                    // 2、client用户密码认证模式
                    // 3、授权码认证模式(code)
                    // 4、简单认证模式(js)
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // 用于认证的密码
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes = { "TeamService","MemberService" }
                },
                new Client
                {
                    ClientId = "client-password",
	                // 使用用户名密码交互式验证
	                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

	                // 用于认证的密码
	                ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
	                // 客户端有权访问的范围（Scopes）
	                AllowedScopes = { "TeamService" }
                },
                // openid客户端
                new Client
                {
                    ClientId="client-code",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Code,
                    RequireConsent=false,
                    RequirePkce=true,

                    RedirectUris={ "https://192.168.2.102:5006/signin-oidc"}, // 1、客户端地址

                    PostLogoutRedirectUris={ "https://192.168.2.102:5006/signout-callback-oidc"},// 2、登录退出地址

                    AllowedScopes=new List<string>{
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TeamService" // 启用服务授权支持
                    },

                    // 增加授权访问
                    AllowOfflineAccess=true
                }
            };
        }

        /// <summary>
        /// 2.1 openid身份资源
        /// </summary>
        public static IEnumerable<IdentityResource> Ids => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        /// <summary>
        /// 3. 测试用户
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>()
            {
                new TestUser
                {
                    SubjectId="1",
                    Username="leo",
                    Password="123456"
                },
                // openid 身份验证
                new TestUser{SubjectId = "818727", Username = "leo-1", Password = "123456",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "leo-1"),
                        new Claim(JwtClaimTypes.GivenName, "leo-1"),
                        new Claim(JwtClaimTypes.FamilyName, "leo-1"),
                        new Claim(JwtClaimTypes.Email, "leo-1@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://leo-1.com"),
                        // new Claim(JwtClaimTypes.Address, @"{ '城市': '杭州', '邮政编码': '310000' }",IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    }
                }
            };
        }
    }
}
