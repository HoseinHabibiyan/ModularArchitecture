using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ModularArchitecture.Identity.Core.Result;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ModularArchitecture.Identity.Core
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(IConfiguration config, ILogger<AccountService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ILoginResult> LoginAsync(ILoginModel login)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "oauth/token")
            };
            var response = await client.PostAsync(_config["IdentityServerUrl"] + "oauth/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", login.Username),
                    new KeyValuePair<string, string>("password", login.Password),
                    new KeyValuePair<string, string>("client_id", _config["ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _config["ClientSecret"])
                }));

            if (response.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                var getUserResult = await GetUserByNameAsync(login.Username);
                if (getUserResult.Success)
                    return new LoginResult(new AuthToken { RefreshToken = r.refresh_token, AccessToken = r.access_token }, getUserResult.User);

                return new LoginResult(new AuthToken { RefreshToken = r.refresh_token, AccessToken = r.access_token }, null);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new LoginResult("Error Connecting to Identity Server");

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new LoginResult(JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync())
                        .error_description);

            return new LoginResult("Error in Identity Server");
        }

        public async Task<IRefreshTokenResult> RefreshTokenAsync(string refreshToken)
        {
            var client = new HttpClient { BaseAddress = new Uri(_config["IdentityServerUrl"] + "oauth/token") };
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var response = await client.PostAsync(_config["IdentityServerUrl"] + "oauth/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("client_id", _config["ClientId"]),
                    new KeyValuePair<string, string>("client_secret", _config["ClientSecret"])
                }));

            var data = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<dynamic>(data);
                var token = new AuthToken { RefreshToken = r.refresh_token, AccessToken = r.access_token };
                return new RefreshTokenResult (token);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new RefreshTokenResult("Error Connecting to Identity Server (404)");

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new RefreshTokenResult(data);

            return new RefreshTokenResult("Error in Identity Server");
        }

        public async Task<IGenerateRegistrationCodeResult> GenerateRegistrationCodeAsync(string cell)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/celltoken")
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/celltoken/" + cell);

            if (response.IsSuccessStatusCode)
            {
                var r = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                return new GenerateRegistrationCodeResult
                { Success = true, Code = r.code, Cell = r.cell, Hashed = r.hashed };
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GenerateRegistrationCodeResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GenerateRegistrationCodeResult
                { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GenerateRegistrationCodeResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GenerateRegistrationCodeResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> VerifyRegistrationCodeAsync(IVerifyRegistrationCodeModel model)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/celltoken/verify")
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/celltoken/verify",
                new
                {
                    model.Cell,
                    model.Code
                });

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "Error Connecting to Identity Server" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> AddRolesToUserAsync(string userId, string[] roleNames)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response =
                await client.PutAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/user/" + userId + "/roles",
                    roleNames);

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };

            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> RemoveRolesFromUserAsync(string userId, string[] roleNames)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response =
                await client.PutAsJsonAsync(
                    _config["IdentityServerUrl"] + "api/accounts/user/" + userId + "/removeroles", roleNames);

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };


            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IRegisterResult> RegisterAsync(IRegisterModel model)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/create")
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/create", new
            {
                model.Email,
                model.Username,
                model.FullName,
                model.PhoneNumber,
                model.Password,
                ConfirmPassword = model.Password
            });

            if (response.IsSuccessStatusCode)
                return new RegisterResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),
                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new RegisterResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new RegisterResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new RegisterResult { Success = false, Message = "status code 401. Unauthorized" };


            return new RegisterResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IGetUserResult> GetUserByNameAsync(string userName)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/user/" + userName + "/");

            if (response.IsSuccessStatusCode)
                return new GetUserResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };


            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUserResult> GetUserByPhoneAsync(string phone)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/phone/" + phone + "/");

            if (response.IsSuccessStatusCode)
                return new GetUserResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };


            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUsersResult> SearchUsersByNameAsync(string name)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/users/" + name + "/");

            if (response.IsSuccessStatusCode)
            {
                var x = await response.Content.ReadAsStringAsync();
                return new GetUsersResult
                {
                    Success = true,
                    Users = new List<IUser>(JsonConvert.DeserializeObject<List<User>>(x)),

                };
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUsersResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUsersResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUsersResult { Success = false, Message = "status code 401. Unauthorized" };


            return new GetUsersResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUserResult> ConfirmEmailAsync(string userId, string code)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/ConfirmEmail?userId=" +
                                                 userId + "&" + "code=" + code);

            if (response.IsSuccessStatusCode)
                return new GetUserResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };


            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGeneratePasswordResetTokenResult> GeneratePasswordResetTokenAsync(IGeneratePasswordResetTokenModel model)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/GeneratePasswordResetToken")
            };
            var response = await client.PostAsJsonAsync(
                _config["IdentityServerUrl"] + "api/accounts/GeneratePasswordResetToken", new
                {
                    model.UserName
                });

            if (response.IsSuccessStatusCode)
                return new GeneratePasswordResetTokenResult
                { Success = true, Code = await response.Content.ReadAsStringAsync() };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GeneratePasswordResetTokenResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GeneratePasswordResetTokenResult
                { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GeneratePasswordResetTokenResult
                { Success = false, Message = "status code 401. Unauthorized" };


            return new GeneratePasswordResetTokenResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> ResetPasswordAsync(IResetPasswordModel model)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/ResetPassword")
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/ResetPassword", new
            {
                model.Username,
                model.Password,
                model.ConfirmPassword,
                model.Code
            });

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };


            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IGetUsersResultTextValue> GetUsersByNameAsync(string name)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/users/" + name + "/");

            if (response.IsSuccessStatusCode)
                return new GetUsersResultTextValue
                {
                    Success = true,
                    Users = JsonConvert.DeserializeObject<List<ITextValue>>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUsersResultTextValue { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUsersResultTextValue { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUsersResultTextValue { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUsersResultTextValue { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetRolesResult> GetAllRolesAsync()
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/roles");

            if (response.IsSuccessStatusCode)
                return new GetRolesResult
                {
                    Success = true,
                    Roles = new List<IRole>(JsonConvert.DeserializeObject<List<Role>>(await response.Content.ReadAsStringAsync())),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetRolesResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetRolesResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetRolesResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetRolesResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetRoleResult> GetRoleByNameAsync(string name)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/roles/role/" + name)
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/roles/role/" + name);

            if (response.IsSuccessStatusCode)
                return new GetRoleResult
                {
                    Success = true,
                    Role = JsonConvert.DeserializeObject<Role>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetRoleResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetRoleResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetRoleResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetRoleResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetRoleResult> GetRoleByIdAsync(string id)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/roles/role/" + id)
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/roles/role/" + id);

            if (response.IsSuccessStatusCode)
                return new GetRoleResult
                {
                    Success = true,
                    Role = JsonConvert.DeserializeObject<Role>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetRoleResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetRoleResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetRoleResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetRoleResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IIdentityResult> CreateRoleAsync(ICreateRoleModel model)
        {
            var applicationId = _config["ApplicationId"];

            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/roles/create", new
            {
                ApplicationId = applicationId,
                model.Name,
                model.SortOrder,
                model.ParentId
            });

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };

            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> UpdateRoleAsync(IUpdateRoleModel model)
        {
            var applicationId = _config["ApplicationId"];

            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/roles/update", new
            {
                ApplicationId = applicationId,
                model.Id,
                model.Name,
                model.SortOrder,
                model.ParentId
            });

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };

            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IGetUsersResult> GetUsersAsync(List<string> userIds)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/users/", userIds);

            if (response.IsSuccessStatusCode)
                return new GetUsersResult
                {
                    Success = true,
                    Users = new List<IUser>(JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync())),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUsersResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUsersResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUsersResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUsersResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUsersResult> GetUsersAsync()
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/users/");

            if (response.IsSuccessStatusCode)
                return new GetUsersResult
                {
                    Success = true,
                    Users = new List<IUser>(JsonConvert.DeserializeObject<List<User>>(await response.Content.ReadAsStringAsync())),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUsersResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUsersResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUsersResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUsersResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUserResult> GetUserAsync(string userId)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/user/" + userId);

            if (response.IsSuccessStatusCode)
                return new GetUserResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),

                };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IIdentityResult> UpdateUserAsync(IUpdateUserModel model)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/update/", model);

            if (response.IsSuccessStatusCode)
                return new GetUserResult { Success = true };
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new RegisterResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> UpdateUserByEmailAsync(IUpdateUserModel model)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            model.Id = _httpContextAccessor.GetUserId().ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response =
                await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/UpdateUserByEmail/", model);

            if (response.IsSuccessStatusCode)
                return new GetUserResult { Success = true };
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new RegisterResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> ChangePasswordAsync(IChangePasswordModel model)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();
            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response =
                await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/ChangePassword/", model);

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new RegisterResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new RegisterResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new RegisterResult { Success = false, Message = "status code 401. Unauthorized" };

            return new RegisterResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> ResetPasswordByAdminAsync(IResetPasswordByAdminModel model)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response =
                await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/ResetPasswordByAdmin/",
                    model);

            if (response.IsSuccessStatusCode)
                return new IdentityResult { Success = true };
            if (response.StatusCode == HttpStatusCode.NotFound)
                return new RegisterResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new RegisterResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new RegisterResult { Success = false, Message = "status code 401. Unauthorized" };

            return new RegisterResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IIdentityResult> ForgetPasswordAsync(IForgetPasswordModel model)
        {
            using var client = new HttpClient
            {
                DefaultRequestHeaders = { Accept = { new MediaTypeWithQualityHeaderValue("application/json") } },
                BaseAddress = new Uri(_config["IdentityServerUrl"] + "api/accounts/forgetpassword")
            };
            var response = await client.PostAsJsonAsync(_config["IdentityServerUrl"] + "api/accounts/forgetpassword",
                new
                {
                    model.Email,
                    model.CallbackUrl
                });

            if (response.IsSuccessStatusCode)
                return new RegisterResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new IdentityResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new IdentityResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new IdentityResult { Success = false, Message = "status code 401. Unauthorized" };

            return new IdentityResult { Success = false, Message = "Error in Identity Server" };
        }

        public async Task<IGetUserResult> DeleteUserAsync(Guid userId)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.DeleteAsync(_config["IdentityServerUrl"] + "api/accounts/user/" + userId);

            if (response.IsSuccessStatusCode)
                return new GetUserResult
                {
                    Success = true,
                    User = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync()),

                };


            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };


            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IIdentityResult> DeleteRoleAsync(Guid roleId)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.DeleteAsync(_config["IdentityServerUrl"] + "api/roles/" + roleId);

            if (response.IsSuccessStatusCode)
                return new GetUserResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUserResult> EnableUserAsync(string userId)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };
            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/EnableUser/" + userId);

            if (response.IsSuccessStatusCode)
                return new GetUserResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        public async Task<IGetUserResult> DisableUserAsync(string userId)
        {
            var auth = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            using var client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")},
                    Authorization = new AuthenticationHeaderValue(auth.Split(' ')[0], auth.Split(' ')[1])
                }
            };

            var response = await client.GetAsync(_config["IdentityServerUrl"] + "api/accounts/DisableUser/" + userId);

            if (response.IsSuccessStatusCode)
                return new GetUserResult { Success = true };

            if (response.StatusCode == HttpStatusCode.NotFound)
                return new GetUserResult { Success = false, Message = "status code 404. NotFound" };

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return new GetUserResult { Success = false, Message = await GetBadRequestErrorAsync(response) };

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new GetUserResult { Success = false, Message = "status code 401. Unauthorized" };

            return new GetUserResult { Success = false, Message = "Error Connecting or in Identity Server" };
        }

        private async Task<string> GetBadRequestErrorAsync(HttpResponseMessage response)
        {
            var result = string.Empty;
            var obj = JsonConvert.DeserializeObject<BadRequestErrorResult>(await response.Content.ReadAsStringAsync());
            if (obj.ModelState != null)
            {
                foreach (var item in obj.ModelState)
                    result += item.Value.Aggregate((x, y) => x + " " + y + Environment.NewLine);
            }
            else
            {
                result += obj.Message;
            }

            return result;
        }
    }
}