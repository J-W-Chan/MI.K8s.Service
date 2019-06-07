using IdentityModel.Client;
using MI.Untity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MI.APIClientService
{
    public class ApiHelperService : IApiHelperService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ApiHelperService> _logger;
        public IConfiguration _configuration;

        public ApiHelperService(ILogger<ApiHelperService> _logger, IHttpClientFactory _httpClientFactory, IConfiguration configuration)
        {
            this._httpClientFactory = _httpClientFactory;
            this._logger = _logger;
            _configuration = configuration;
        }


        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        public async Task<T> PostAsync<T>(string url, object Model)
        {
            var http = _httpClientFactory.CreateClient("MI.Web");
            //添加Token
            var token = await GetToken();
            http.SetBearerToken(token);
            //使用FormUrlEncodedContent做HttpContent
            var httpContent = new StringContent(JsonConvert.SerializeObject(Model), Encoding.UTF8, "application/json");
            //await异步等待回应
            var response = await http.PostAsync(url, httpContent);

            //确保HTTP成功状态值
            response.EnsureSuccessStatusCode();

            //await异步读取
            string Result = await response.Content.ReadAsStringAsync();

            var Item = JsonConvert.DeserializeObject<T>(Result);

            return Item;
        }

        /// <summary>
        /// HttpClient实现Post请求（用于MQ发布功能 无返回）
        /// </summary>
        public async Task PostAsync(string url, string requestMessage)
        {
            var http = _httpClientFactory.CreateClient();
            //添加Token
            var token = await GetToken();
            http.SetBearerToken(token);
            //使用FormUrlEncodedContent做HttpContent
            var httpContent = new StringContent(requestMessage, Encoding.UTF8, "application/json");
            //await异步等待回应
            var response = await http.PostAsync(url, httpContent);

            //确保HTTP成功状态值
            response.EnsureSuccessStatusCode();
        }


        /// <summary>
        /// HttpClient实现Get请求
        /// </summary>
        public async Task<T> GetAsync<T>(string url)
        {
            var http = _httpClientFactory.CreateClient("MI.Web");
            //添加Token
            var token = await GetToken();
            http.SetBearerToken(token);
            //await异步等待回应
            var response = await http.GetAsync(url);
            //确保HTTP成功状态值
            response.EnsureSuccessStatusCode();

            var Result = await response.Content.ReadAsStringAsync();

            var Items = JsonConvert.DeserializeObject<T>(Result);

            return Items;
        }


        /// <summary>
        /// 转换URL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str);
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }
            return (sb.ToString());
        }

        //获取Token
        //获取Token
        public async Task<string> GetToken()
        {
            var client = _httpClientFactory.CreateClient("MI.Web");
            string token = await Untity.StackRedis.Current.Get("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                return token;
            }
            try
            {
                //DiscoveryClient类：IdentityModel提供给我们通过基础地址（如：http://localhost:5000）就可以访问令牌服务端；
                //当然可以根据上面的restful api里面的url自行构建；上面就是通过基础地址，获取一个TokenClient;（对应restful的url：token_endpoint   "http://localhost:5000/connect/token"）
                //RequestClientCredentialsAsync方法：请求令牌；
                //获取令牌后，就可以通过构建http请求访问API接口；这里使用HttpClient构建请求，获取内容；

                DiscoveryPolicy discoveryPolicy = new DiscoveryPolicy { RequireHttps = false };
                var cache = new DiscoveryCache(_configuration["ServiceAddress:Service.Identity"], client, discoveryPolicy);
                var disco = await cache.GetAsync();
                if (disco.IsError) throw new Exception(disco.Error);
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "MI.Web",
                    ClientSecret = "miwebsecret",
                    Scope = "MI.Service"
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception(tokenResponse.Error);

                }
                token = tokenResponse.AccessToken;
                int minute = tokenResponse.ExpiresIn / 60;
                await Untity.StackRedis.Current.Set("ApiToken", token, minute);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return token;
        }
    }
}
