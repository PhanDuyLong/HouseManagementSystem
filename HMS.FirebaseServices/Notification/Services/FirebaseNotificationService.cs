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
    public partial interface IFirebaseNotificationService
    {
        Task<bool> PushNotificationAsync(MobileNotification firebaseNotification);
    }
    public partial class FirebaseNotificationService : IFirebaseNotificationService
    {
        public FirebaseNotificationService() { }

        public async Task<bool> PushNotificationAsync(MobileNotification firebaseNotification)
        {
            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = firebaseNotification.Title,
                    Body = firebaseNotification.Body
                },
                Data = firebaseNotification.Data,
                Condition = "'" + firebaseNotification.UserId + "' in topics"
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
