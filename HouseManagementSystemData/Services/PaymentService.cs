using AutoMapper;
using FirebaseAdmin.Messaging;
using HMS.Data.Models;
using HMS.Data.Repositories;
using HMS.Data.Services.Base;
using HMS.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Data.Services
{
    public partial interface IPaymentService : IBaseService<Payment>
    {
        Task<string> Payment(BillPaymentViewModel model);
    }
    public partial class PaymentService : BaseService<Payment>, IPaymentService
    {
        private readonly IMapper _mapper;
        public PaymentService(DbContext dbContext, IPaymentRepository repository, IMapper mapper) : base(dbContext, repository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<string> Payment(BillPaymentViewModel model)
        {
            /*string noti = getAndroidMessage("Hello", "Hi", "");
            Send(noti);*/
            return null;
        }
        /*
                public async Task Send(string notification)
                {
                    *//*            var fcmKey = "AAAAmDePzT8:APA91bFd1CTUG8MKkU9F9BSnxjWDfni59n2tNCw5m4wTUVG_u_NKu1ynqamwEZ1Y1yTtNQSCeZh_hYgYlzrgxNnZxeTfl5Eutiqub1FbUTMRqb8zZOr7L-3MeEv-6Z4WLdGl-Be39D0W";
                                var http = new HttpClient();
                                http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + fcmKey);
                                http.DefaultRequestHeaders.TryAddWithoutValidation("content-length", notification.Length.ToString());
                                var content = new StringContent(notification, System.Text.Encoding.UTF8, "application/json");

                                var response = await http.PostAsync("https://fcm.googleapis.com/fcm/send", content);
                    *//*



                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                    tRequest.Method = "post";
                    //serverKey - Key from Firebase cloud messaging server  
                    tRequest.Headers.Add(string.Format("Authorization: key={0}", "AIzaSyBOUnY4MomlWzp-8pMw2QNStK-k6Q27FB4"));
                    //Sender Id - From firebase project setting  
                    tRequest.Headers.Add(string.Format("Sender: id={0}", "333645216941"));
                    tRequest.ContentType = "application/json";
                    var payload = new
                    {
                        to = "/topics/all",
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = "Test",
                            title = "Test",
                            badge = 1
                        },
                        data = new
                        {
                            key1 = "value1",
                            key2 = "value2"
                        }

                    };

                    string postbody = JsonConvert.SerializeObject(payload).ToString();
                    Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
                    tRequest.ContentLength = byteArray.Length;
                    using Stream dataStream = tRequest.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using WebResponse tResponse = tRequest.GetResponse();
                    using Stream dataStreamResponse = tResponse.GetResponseStream();
                    if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                        {
                            String sResponseFromServer = tReader.ReadToEnd();
                            //result.Response = sResponseFromServer;
                        }
                }
                public static string getAndroidMessage(string title, object data, string regId)
                {
                    Dictionary<string, object> androidMessageDic = new Dictionary<string, object>();
                    androidMessageDic.Add("collapse_key", title);
                    androidMessageDic.Add("title", title);
                    androidMessageDic.Add("data", data);
                    androidMessageDic.Add("to", regId);
                    androidMessageDic.Add("delay_while_idle", true);
                    androidMessageDic.Add("time_to_live", 125);
                    androidMessageDic.Add("dry_run", false);
                    return JsonConvert.SerializeObject(androidMessageDic);

                }*/
    }
}
