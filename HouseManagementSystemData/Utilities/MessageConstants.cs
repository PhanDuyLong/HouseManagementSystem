using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Data.Utilities
{
    public class MessageConstants
    {
        //200 OK
        public const string OK01 = "{0} created successfully!";
        public const string OK02 = "{0} deleted successfully!";
        public const string OK03 = "{0} updated successfully!";
        public const string OK04 = "{0} successfully";

        //404 Not Found
        public const string NF01 = "{0}(s) is/are not found!";
        public const string NF02 = "{0} is not found!";

        //400 Bad Request
        public const string BR01 = "Some properties are not valid!";
        public const string BR02 = "{0} creation failed! Please check details and try again!";
        public const string BR03 = "{0} already exists!";
        public const string BR04 = "{0} is {1}!";
        public const string BR05 = "{0} update failed! Please check details and try again!";
        public const string BR06 = "There is no user with that {0}!";
        public const string BR07 = "Invalid {0}!";
        public const string BR08 = "{0} was {1}!";

        //500 Internal Server Error
        public const string ISE01 = "Something wrong with server!";
    }
}
