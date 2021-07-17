using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.FirebaseServices.Authen.Models
{
    public class FirebaseResponse
    {
        public string Kind { get; set; }
        public List<FirebaseUser> Users { get; set; }
    }
    public class FirebaseUser
    {
        public string LocalId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool EmailVerified { get; set; }
        public long PasswordUpdatedAt { get; set; }
        public List<ProviderFirebaseUserInfo> ProviderUserInfo { get; set; }
        public string ValidSince { get; set; }
        public string LastLoginAt { get; set; }
        public string CreatedAt { get; set; }
        public bool CustomAuth { get; set; }
        public DateTime LastRefreshAt { get; set; }
    }
    public class ProviderFirebaseUserInfo
    {
        public string ProviderId { get; set; }
        public string FederatedId { get; set; }
        public string Email { get; set; }
        public string RawId { get; set; }
    }
}
