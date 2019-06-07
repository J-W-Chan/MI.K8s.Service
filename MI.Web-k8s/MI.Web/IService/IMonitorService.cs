using MI.Service.Monitor.Model.Request;
using MI.Service.Monitor.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MI.Web.IService
{
    public interface IMonitorService
    {
        Task<QueryRabbitMqRoutingKeyInfoResponse> QueryRabbitMqRoutingKeyInfoAsync(QueryRabbitMqRoutingKeyInfoRequest request);
        Task<RoutingKeyRegisterResponse> RoutingKeyRegisterAsync(RoutingKeyRegisterRequest request);
    }
}
