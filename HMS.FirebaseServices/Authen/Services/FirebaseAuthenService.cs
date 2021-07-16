using FirebaseAdmin.Auth;
using HMS.FirebaseServices.Authen.Requests;
using HMS.FirebaseServices.Authen.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HMS.FirebaseServices.Authen.Services
{
    public partial interface IFirebaseAuthenService
    {
        Task<FirebaseAuthenticateResponse> LoginByExternalAsysnc(FirebaseAuthenticateRequest model);
        Task<FirebaseAuthenticateResponse> LoginByIdTokenAsync(string idToken);
    }
    public partial class FirebaseAuthenService : IFirebaseAuthenService
    {
        public async Task<FirebaseAuthenticateResponse> LoginByExternalAsysnc(FirebaseAuthenticateRequest model)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://identitytoolkit.googleapis.com");
            string uri = "/v1/accounts:signInWithPassword?key=" + "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4";
            var payload = new
            {
                email = model.Email,
                password = model.Password,
            };
            string postbody = JsonConvert.SerializeObject(payload).ToString();
            StringContent content = new StringContent(postbody);
            var firebaseAuthRequest = await client.PostAsync(uri, content);
            var response = JObject.Parse(await firebaseAuthRequest.Content.ReadAsStringAsync()); ;
            if (!firebaseAuthRequest.IsSuccessStatusCode)
            {
                return new FirebaseAuthenticateResponse
                {
                    Message = response.SelectToken("$.error.message").ToString(),
                    IsSuccess = false
                };
            }
            else
            {
                if (!model.UserId.Equals(response.SelectToken("localId").ToString()))
                {
                    return new FirebaseAuthenticateResponse
                    {
                        Message = "Invalid UserId!",
                        IsSuccess = false
                    };
                }
                
            }
            return new FirebaseAuthenticateResponse
            {
                UserId = response.SelectToken("localId").ToString(),
                Message = "Login succesfully",
                IsSuccess = true
            };
        }

        public async Task<FirebaseAuthenticateResponse> LoginByIdTokenAsync(string idToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://identitytoolkit.googleapis.com");
            string uri = "/v1/accounts:lookup?key=" + "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4";
            var payload = new
            {
                idToken = idToken,
            };
            string postbody = JsonConvert.SerializeObject(payload).ToString();
            StringContent content = new StringContent(postbody);
            var firebaseAuthRequest = await client.PostAsync(uri, content);
            var response = JObject.Parse(await firebaseAuthRequest.Content.ReadAsStringAsync());
            if (!firebaseAuthRequest.IsSuccessStatusCode)
            {
                return new FirebaseAuthenticateResponse
                {
                    Message = response.SelectToken("$.error.message").ToString(),
                    IsSuccess = false
                };
            }
            else
            {
                return new FirebaseAuthenticateResponse
                {
                    UserId = response.SelectToken("localId").ToString(),
                    Message = "Login succesfully",
                    IsSuccess = true
                };
            }
        }
    }
}
