using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NC.MicroService.IdentityClient.Models;
using Newtonsoft.Json.Linq;

namespace NC.MicroService.IdentityClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            #region token模式
            {
                //// 1. 生成 Access Token
                ////    1.1 客户端模式
                ////    1.2 客户端用户密码模式
                ////    1.3 客户端code状态码模式 --> 类似微信网页授权中的code
                //string accessToken = await GetAccessToken();

                //// 2. 使用 AccessToken 进行资源访问
                //string result = await UseAccessToken(accessToken);

                //// 3. 相应结果到页面
                //ViewData.Add("Json", result);
            }
            #endregion

            #region openid connect 协议
            {
                // 1、获取token(id-token , access_token ,refresh_token)
                var accessToken = await HttpContext.GetTokenAsync("access_token"); // 以及 id_token
                Console.WriteLine($"accessToken:{accessToken}");
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                Console.WriteLine($"refresh_token:{refreshToken}");
                
                var client = _httpClientFactory.CreateClient("disableHttpsValidation");

                // 这两种方式都可以，设置请求头accesstoken
                client.SetBearerToken(accessToken);
                //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                // 2、使用token
                // 直接访问teamservice
                // var result = await client.GetStringAsync("https://192.168.2.105:5001/teams");
                // 集成ocelot网关
                //var result = await client.GetStringAsync("https://192.168.2.105:5004/ocelot/teams");
                // 集成ocelot网关，配置动态路由后的请求方式，注意所有请求的第一层目录必须是对应着Consul中的服务名
                var result = await client.GetStringAsync("https://192.168.2.105:5004/TeamService/teams"); // 最终会被转换为 https://192.168.2.105:5001/Teams

                // 3、响应结果到页面
                ViewData.Add("Json", result);
            }
            #endregion

            return View();
        }

        /// <summary>
        /// 1. 获取 access token
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAccessToken()
        {
            // 1. 建立连接
            DiscoveryDocumentResponse discovery;
            TokenResponse tokenResponse;

            // ==> 使用 IHttpClientFactory 代替
            //var handler = new HttpClientHandler();
            //handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, x509, x509Chain, errors) =>
            //{
            //    return true;
            //};
            //var client = new HttpClient(handler);


            var client = _httpClientFactory.CreateClient("disableHttpsValidation");
            string serverUrl = "https://192.168.2.105:5005";
            discovery = await client.GetDiscoveryDocumentAsync(serverUrl);
            if (discovery.IsError)
            {
                Console.WriteLine($"[DiscoveryDocumentResponse Error]: {discovery.Error}");
            }

            // 1.1、通过客户端获取AccessToken
            //tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            //{
            //    Address = discovery.TokenEndpoint, // 1、生成AccessToken中心
            //    ClientId = "client", // 2、客户端编号
            //    ClientSecret = "secret",// 3、客户端密码
            //    Scope = "TeamService,MemberService" // 4、客户端需要访问的API
            //});

            // 1.2 通过客户端用户密码获取 AccessToken
            //tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            //{
            //    Address = discovery.TokenEndpoint,
            //    ClientId = "client-password",
            //    ClientSecret = "secret",
            //    Scope = "TeamService",
            //    UserName = "leo11",
            //    Password = "123456"
            //});

            // 1.3 通过授权code获取AccessToken[需要进行登录] --> 刷新token client.RequestRefreshTokenAsync(xxx)
            tokenResponse = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = "client-code",
                ClientSecret = "secret",
                Code = "12",
                RedirectUri = "https://192.168.2.105:5005"
            });


            if (tokenResponse.IsError)
            {
                //ClientId 与 ClientSecret 错误，报错：invalid_client
                //Scope 错误，报错：invalid_scope
                //UserName 与 Password 错误，报错：invalid_grant
                string errorDescription = tokenResponse.ErrorDescription;
                if (string.IsNullOrEmpty(errorDescription)) errorDescription = "";
                if (errorDescription.Equals("invalid_username_or_password"))
                {
                    Console.WriteLine("用户名或密码错误，请重新输入！");
                }
                else
                {
                    Console.WriteLine($"[TokenResponse Error]: {tokenResponse.Error}, [TokenResponse Error Description]: {errorDescription}");
                }
            }
            else
            {
                Console.WriteLine($"Access Token: {tokenResponse.Json}");
                Console.WriteLine($"Access Token: {tokenResponse.RefreshToken}");
                Console.WriteLine($"Access Token: {tokenResponse.ExpiresIn}");
            }
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// 2、使用token
        /// </summary>
        public async Task<string> UseAccessToken(string AccessToken)
        {
            var apiClient = _httpClientFactory.CreateClient("disableHttpsValidation");
            apiClient.SetBearerToken(AccessToken); // 1、设置token到请求头
            // 直接访问teamservice
            //HttpResponseMessage response = await apiClient.GetAsync("https://192.168.2.105:5001/teams");
            // 集成ocelot网关
            HttpResponseMessage response = await apiClient.GetAsync("https://192.168.2.105:5004/ocelot/teams");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"API Request Error, StatusCode is : {response.StatusCode}");
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("");
                Console.WriteLine($"Result: {JArray.Parse(content)}");

                // 3、输出结果到页面
                return JArray.Parse(content).ToString();
            }
            return "";

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
