using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CoreWcfAuthorization.Contracts
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Test1();
    }
}
