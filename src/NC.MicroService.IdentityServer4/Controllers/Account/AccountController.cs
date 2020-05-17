// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NC.MicroService.IdentityServer4.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NC.MicroService.IdentityServer4.Controllers
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        //private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        private UserManager<User> _userManager; // 1. 用户相关操作
        private SignInManager<User> _signInManager; // 2. 登录相关操作

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        //TestUserStore users = null)
        {
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            //_users = users ?? new TestUserStore(TestUsers.Users);

            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }

            return View(vm);
        }

        /// <summary>
        /// 登录（持久化）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // 检查当前请求是否处于授权请求的上下文中
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // 用户点击了 “cancel” 取消按钮，重定向访问地址
            if (button != "login")
            {
                if (context != null)
                {
                    // 如果用户取消，则将结果发送回IdentityServer，就像他们拒绝同意一样(即使此客户端不需要同意)。这将向客户端发回一个拒绝访问OIDC错误响应。
                    await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                    // model.ReturnUrl 是可信的，因为GetAuthorizationContextAsync返回非空
                    if (await _clientStore.IsPkceClientAsync(context.ClientId))
                    {
                        // 如果客户端是PKCE，那么我们假设它是本地的，所以这种响应返回方式的改变是为了最终用户更好的于用户体验。
                        // PKCE：https://oauth.net/2/pkce/
                        // PKCE (RFC 7636)是授权代码流的扩展，以防止某些攻击，并能够安全地执行来自公共客户端的OAuth交换。
                        // 它主要用于移动和JavaScript应用程序，但该技术也可以应用于任何客户机。
                        // 利用授权码授权的OAuth 2.0公共客户端容易受到授权码拦截攻击。该规范描述了攻击以及一种通过使用代码交换的Proof Key (PKCE，发音为“pixy”)来减轻威胁的技术。
                        return this.LoadingPage("Redirect", model.ReturnUrl);
                    }

                    return Redirect(model.ReturnUrl);
                }

                // 如果没有有效的上下文，返回到主页
                return Redirect("~/");
            }

            // 登录核心逻辑
            if (ModelState.IsValid)
            {
                // 验证用户名是否存在
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(nameof(model.Username), $"用户名{model.Username} 不存在");
                }
                else
                {
                    // 验证密码是否一致
                    if (await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        AuthenticationProperties props = null;
                        if (model.RememberLogin)
                        {
                            props = new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromMinutes(30))
                            };
                        }

                        var idsUser = new IdentityServerUser(user.Id.ToString())
                        {
                            DisplayName = user.UserName
                        };

                        await HttpContext.SignInAsync(idsUser, props);

                        if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        return Redirect("~/");
                    }

                    ModelState.AddModelError(nameof(model.Password), "密码错误");
                }
            }

            // 输出异常信息
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        #region 原 Login 方法（内存存储测试）
        ///// <summary>
        ///// Handle postback from username/password login
        ///// </summary>
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginInputModel model, string button)
        //{
        //    // check if we are in the context of an authorization request
        //    var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

        //    // the user clicked the "cancel" button
        //    if (button != "login")
        //    {
        //        if (context != null)
        //        {
        //            // if the user cancels, send a result back into IdentityServer as if they 
        //            // denied the consent (even if this client does not require consent).
        //            // this will send back an access denied OIDC error response to the client.
        //            await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

        //            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
        //            if (await _clientStore.IsPkceClientAsync(context.ClientId))
        //            {
        //                // if the client is PKCE then we assume it's native, so this change in how to
        //                // return the response is for better UX for the end user.
        //                return this.LoadingPage("Redirect", model.ReturnUrl);
        //            }

        //            return Redirect(model.ReturnUrl);
        //        }
        //        else
        //        {
        //            // since we don't have a valid context, then we just go back to the home page
        //            return Redirect("~/");
        //        }
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // validate username/password against in-memory store
        //        if (_users.ValidateCredentials(model.Username, model.Password))
        //        {
        //            var user = _users.FindByUsername(model.Username);
        //            await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.ClientId));

        //            // only set explicit expiration here if user chooses "remember me". 
        //            // otherwise we rely upon expiration configured in cookie middleware.
        //            AuthenticationProperties props = null;
        //            if (AccountOptions.AllowRememberLogin && model.RememberLogin)
        //            {
        //                props = new AuthenticationProperties
        //                {
        //                    IsPersistent = true,
        //                    ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
        //                };
        //            };

        //            // issue authentication cookie with subject ID and username
        //            var isuser = new IdentityServerUser(user.SubjectId)
        //            {
        //                DisplayName = user.Username
        //            };

        //            await HttpContext.SignInAsync(isuser, props);

        //            if (context != null)
        //            {
        //                if (await _clientStore.IsPkceClientAsync(context.ClientId))
        //                {
        //                    // if the client is PKCE then we assume it's native, so this change in how to
        //                    // return the response is for better UX for the end user.
        //                    return this.LoadingPage("Redirect", model.ReturnUrl);
        //                }

        //                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
        //                return Redirect(model.ReturnUrl);
        //            }

        //            // request for a local page
        //            if (Url.IsLocalUrl(model.ReturnUrl))
        //            {
        //                return Redirect(model.ReturnUrl);
        //            }
        //            else if (string.IsNullOrEmpty(model.ReturnUrl))
        //            {
        //                return Redirect("~/");
        //            }
        //            else
        //            {
        //                // user might have clicked on a malicious link - should be logged
        //                throw new Exception("invalid return URL");
        //            }
        //        }

        //        await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId:context?.ClientId));
        //        ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
        //    }

        //    // something went wrong, show form with error
        //    var vm = await BuildLoginViewModelAsync(model);
        //    return View(vm);
        //}
        #endregion

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
    }
}
