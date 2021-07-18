using FirebaseAdmin.Auth;
using HMS.FirebaseServices.Authen.Models;
using HMS.FirebaseServices.Authen.Requests;
using HMS.FirebaseServices.Authen.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HMS.FirebaseServices.Authen.Services
{
    public partial interface IFirebaseAuthenService
    {
        Task<FirebaseAuthenticateResponse> LoginByEmailAndPasswordAsysnc(string email, string password);
        Task<FirebaseAuthenticateResponse> LoginByIdTokenAsync(string idToken);
    }
    public partial class FirebaseAuthenService : IFirebaseAuthenService
    {
        private readonly HttpClient _httpClient;

        public FirebaseAuthenService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://identitytoolkit.googleapis.com")
            };
        }
        public async Task<FirebaseAuthenticateResponse> LoginByEmailAndPasswordAsysnc(string email, string password)
        {
            string uri = "/v1/accounts:signInWithPassword?key=" + "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4";
            var payload = new
            {
                email,
                password,
            };
            var firebaseAuthRequest = await _httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(payload).ToString()));
            var response = JObject.Parse(await firebaseAuthRequest.Content.ReadAsStringAsync()); ;
            if (!firebaseAuthRequest.IsSuccessStatusCode)
            {
                return new FirebaseAuthenticateResponse
                {
                    Message = response.SelectToken("$.error.message").ToString(),
                    IsSuccess = false
                };
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
            string uri = "/v1/accounts:lookup?key=" + "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4";
            var payload = new
            {
                idToken,
            };
            var firebaseAuthRequest = await _httpClient.PostAsync(uri, new StringContent(JsonConvert.SerializeObject(payload).ToString()));
            var response = JObject.Parse(await firebaseAuthRequest.Content.ReadAsStringAsync());
            if (!firebaseAuthRequest.IsSuccessStatusCode)
            {
                return new FirebaseAuthenticateResponse
                {
                    Message = response.SelectToken("$.error.message").ToString(),
                    IsSuccess = false
                };
            }

            var firebaseResponse = JsonConvert.DeserializeObject<FirebaseResponse>(response.ToString()); ;
            var userId = firebaseResponse.Users.FirstOrDefault().LocalId;

            return new FirebaseAuthenticateResponse
            {
                UserId = userId,
                IsSuccess = true
            };
        }
    }
}
