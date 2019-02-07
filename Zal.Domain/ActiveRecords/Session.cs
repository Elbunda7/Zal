using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zal.Bridge;
using Zal.Domain.Consts;
using Zal.Bridge.Models.ApiModels;
using Zal.Bridge.Models;
using Zal.Domain.Models;

namespace Zal.Domain.ActiveRecords
{
    public class Session
    {
        public event SessionStateDelegate UserStateChanged;

        public User CurrentUser { get; set; }
        private string RefreshToken { get; set; }
        internal string Token { get; set; }//exception when token = null?

        public bool StayLogged => !string.IsNullOrEmpty(RefreshToken);
        public bool IsUserLogged => CurrentUser != null;
        public ZAL.Rank UserRank => IsUserLogged ? CurrentUser.Rank : ZAL.Rank.Novacek;

        private static SessionGateway gateway;
        private static SessionGateway Gateway => gateway ?? (gateway = new SessionGateway());


        private void ClearSession() {
            CurrentUser = null;
            RefreshToken = null;
            Token = null;
        }

        internal void Stop() {
            CurrentUser = null;
            Token = null;
        }

        public async Task<LoginErrorModel> LoginAsync(string email, string password, bool stayLogged) {
            var requestModel = new LoginRequestModel {
                Email = email,
                Password = password, 
                StayLogged = stayLogged
            };
            var respondModel = await Gateway.LoginAsync(requestModel);
            if (!respondModel.HasAnyErrors) {
                CurrentUser = new User(respondModel.UserModel);
                Token = respondModel.Token;
                RefreshToken = respondModel.RefreshToken;
                RaisSessionStateChanged();
            }
            return new LoginErrorModel(respondModel);
        }

        public async Task<bool> RegisterAsync(string name, string surname, string phone, string email, string password) {
            var requestModel = new RegistrationRequestModel {
                Name = name,
                Surname = surname,
                Phone = phone,
                Email = email,
                Password = password
            };
            bool isRegistered = await Gateway.RegisterAsync(requestModel);
            if (isRegistered) {
                CurrentUser = new User(new UserModel(requestModel));
            }
            return isRegistered;
        }

        public async Task BecomeMember(string phone, DateTime birth, bool isBoy)
        {
            CurrentUser = await CurrentUser.BecomeMember(phone, birth, isBoy);
        }

        public async Task<bool> TryLoginWithTokenAsync() {
            bool isLogged = StayLogged && CurrentUser != null;
            if (isLogged) {
                var requestModel = new TokenRequestModel {
                    IdUser = CurrentUser.Id,
                    RefreshToken = RefreshToken,
                };
                var respondModel = await Gateway.RefreshTokenAsync(requestModel);
                isLogged = !respondModel.IsExpired;
                if (isLogged) {
                    Token = respondModel.Token;
                    RaisSessionStateChanged();
                }
                else {
                    ClearSession();
                }
            }
            return isLogged;
        }

        public async Task Logout() {
            var requestModel = new LogoutRequestModel {
                IdUser = CurrentUser.Id,
                Token = Token,
            };
            await Gateway.LogoutAsync(requestModel);
            ClearSession();
            RaisSessionStateChanged();
        }

        public void RaisSessionStateChanged() {
            if (UserStateChanged != null) {
                UserStateChanged.Invoke(this);
            }
        }

        internal JObject GetJson() {
            JObject json = new JObject {
                { "StayLogged", StayLogged }
            };
            if (StayLogged) {
                json.Add("RefreshToken", RefreshToken);
                json.Add("CurrentUser", CurrentUser.GetJson());
            }            
            return json;
        }

        internal void LoadFrom(JToken jToken) {
            bool stayLogged = jToken.Value<bool>("StayLogged");
            if (stayLogged) {
                RefreshToken = jToken.Value<string>("RefreshToken");
                CurrentUser = User.LoadFrom(jToken.SelectToken("CurrentUser"));
            }
        }
    }
}
