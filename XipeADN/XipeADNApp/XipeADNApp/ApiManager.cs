using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using XipeADNApp.Models;

namespace XipeADNApp
{
    public interface IApiManager
    {
        Task<bool> TryLogin(string email, string passwd);
        Task<bool> TryRegister(string email, string name, string passwd);
        Task<bool> CheckToken();
        Task<bool> Logout();

        Task<bool> TryEditProfile(string name, string phone, string email,
            string company, string companyRole);
    }

    public class ApiManager : IApiManager
    {
        static readonly bool _production = true;
        static readonly string _baseUrl = _production ? "https://xipeadn.gear.host/" :  "https://localhost:5001/";
        readonly RestClient _client;
        public static UserModel _currentUser;

        public ApiManager()
        {
            try { _client = new RestClient(_baseUrl); }
            catch (Exception ex) { LogException(ex); }
        }

        #region User
        public async Task<bool> TryLogin(string email, string passwd)
        {
            try
            {
                _currentUser = null;
                var req = new RestRequest("api/account/authenticate") { Method = Method.POST };
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Content-Type", "application/json");
                req.RequestFormat = DataFormat.Json;
                //req.AddBody(new { email, password = passwd });
                req.AddJsonBody(new { email, password = passwd });

                var httpResp = await _client.ExecutePostTaskAsync(req);
                var respJson = httpResp.Content;
                var respObj = JsonConvert.DeserializeObject<UserModel>(respJson);
                if (respObj != null)
                    _currentUser = respObj;
                return true;
            }
            catch (Exception ex)
            { LogException(ex); }
            return false;
        }

        public async Task<bool> TryRegister(string email, string name, string passwd)
        {
            try
            {
                var req = new RestRequest("api/account/register") { Method = Method.POST };
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Content-Type", "application/json");
                req.RequestFormat = DataFormat.Json;
                req.AddJsonBody(new { email, firstName = name, password = passwd });
                var httpResp = await _client.ExecutePostTaskAsync(req);
                return httpResp.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            { LogException(ex); }
            return false;
        }

        public async Task<bool> CheckToken()
        {
            try
            {
                var req = new RestRequest("api/account/check") { Method = Method.GET };
                req.AddHeader("Authorize", $"bearer {_currentUser.Token}");
                req.AddHeader("Accept", "application/json");
                var resp = await _client.ExecuteGetTaskAsync(req);
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                await Logout();
            }
            catch (Exception ex)
            { LogException(ex); }
            return false;
        }

        public async Task<bool> Logout()
        {
            try
            {
                await Prism.PrismApplicationBase.Current.SavePropertiesAsync();
                if (Prism.PrismApplicationBase.Current.Properties.ContainsKey("session"))
                    Prism.PrismApplicationBase.Current.Properties.Remove("session");
                _currentUser = null;
                return true;
            }
            catch (Exception ex)
            { LogException(ex); }
            return false;
        }
        #endregion

        #region Profile
        public async Task<bool> TryEditProfile(string name, string phone, string email,
            string company, string companyRole)
        {
            try
            {
                if (_currentUser == null)
                    throw new ArgumentNullException();

                var req = new RestRequest("api/account/edit") { Method = Method.POST };
                req.AddHeader("Accept", "application/json");
                req.AddHeader("Authorization", $"bearer {_currentUser.Token}");
                req.AddJsonBody(new { firstName = name, phone, email, company, companyRole });

                var httpResp = await _client.ExecutePostTaskAsync(req);
                return httpResp.IsSuccessful;
            }
            catch (Exception ex)
            { LogException(ex); }
            return false;
        }
        #endregion

        public UserModel GetCurrentUser => _currentUser;

        void LogException(Exception ex) =>
            Debugger.Log(ex.HResult, ex.Source, ex.Message);
    }
}
