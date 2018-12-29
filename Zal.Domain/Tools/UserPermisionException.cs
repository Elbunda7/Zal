using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Zal.Domain.Tools
{
    public class UserPermisionException : Exception
    {
        public UserPermisionException() {
        }

        public UserPermisionException(string message) : base(message) {
        }

        public UserPermisionException(string message, Exception innerException) : base(message, innerException) {
        }

        protected UserPermisionException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }
    }
}
