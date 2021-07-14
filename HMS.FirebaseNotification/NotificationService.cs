using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HMS.FirebaseNotification
{
    public partial interface INotificationService
    {
        Task<bool> PushNotificationAsync(MobileNotification firebaseNotification);
    }
    public partial class NotificationService : INotificationService
    {
        public NotificationService()
        {
            var defaultApp = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "key.json")),
            });

        }
        public async Task<bool> PushNotificationAsync(MobileNotification firebaseNotification)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = firebaseNotification.Title,
                    Body = firebaseNotification.Body
                },
                Topic = firebaseNotification.Topic
            };
            var messaging = FirebaseMessaging.DefaultInstance;
            var result = await messaging.SendAsync(message);
            if(result != null)
            {
                return true;
            }
            return false;
        }
    }
}
