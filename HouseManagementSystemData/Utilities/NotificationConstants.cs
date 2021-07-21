using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Utilities
{
    public class NotificationConstants
    {
        //Contract
        //Tenant
        public const string TENANT_CONTRACT_WILL_EXPIRE_IN_1_WEEK = "Hợp đồng của {0} trong vòng 1 tuần sẽ hết hạn!";
        public const string TENANT_CONTRACT_HAS_EXPIRED = "Hợp đồng của {0} đã hết hạn!";
        public const string TENANT_CONTRACT_IS_DELETED = "Hợp đồng của {0} đã bị hủy!";
        public const string TENANT_CONTRACT_IS_UPDATED = "Hợp đồng của {0} có thay đổi!";

        //Owner
        public const string ONWER_CONTRACT_HAS_EXPIRED = "Hợp đồng của {0} đã hết hạn!";
        public const string ONWER_CONTRACT_WILL_EXPIRE_IN_1_WEEK = "Hợp đồng của {0} trong vòng 1 tuần sẽ hết hạn!";


        //Bill
        //Tenant
        public const string HAVE_NEW_BILL = "Có hóa đơn mới!!!";
        public const string APPROVE_PAYMENT = "Chủ trọ đã xác nhân việc thanh toán hóa đơn!";
        public const string DENY_PAYMENT = "Chủ trọ đã từ chối việc thanh toán hóa đơn!";
        public const string TODAY_IS_ROOM_BILL_PAID_DEADLINE = "Hôm nay là hạn chót đóng tiền!!!";

        //Owner
        public const string HAVE_PAYMENT = "Hóa đơn {0} đã được thanh toán! Cần xác nhận";
        //public const string THIS_MONTH_HAS_NOT_CREATED_BILL = "Hôm nay là ngày chốt hóa đơn {0}";
        public const string ROOM_BILL_PAID_DEADLINE_IS_PASSED = "Hóa đơn {0} đã quá hạn thanh toán";
    }
}
