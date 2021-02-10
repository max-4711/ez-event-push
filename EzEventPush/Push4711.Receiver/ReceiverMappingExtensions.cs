using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Push4711.Receiver
{
    public static class ClientMappingExtensions
    {
        public static void MapClientMethods<T>(this HubConnection hub, T hubClient)
        {
            if (hubClient == null)
                return;

            var methods = typeof(T).GetMethods();
            hub.MapClientMethods(hubClient, methods);
        }

        private static void MapClientMethods(this HubConnection hub, object hubClient, IEnumerable<MethodInfo> methodInfos)
        {
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.IsSpecialName || !methodInfo.Name.StartsWith("On"))
                    continue;

                var methodParameters = methodInfo.GetParameters();
                var methodParameterTypes = methodParameters.Select(x => x.ParameterType).ToArray();

                //Console.WriteLine($"Configuring callback for {methodInfo.Name} with expected event type(s) {string.Join(",", methodParameterTypes.Select(x => x.ToString()))}");
                if (methodParameterTypes.Length > 0)
                {
                    hub.On($"{methodInfo.Name}_{string.Join(',', methodParameterTypes.Select(x => x.ToString()))}", methodParameterTypes, parameters => Invoke(hubClient, methodInfo, parameters));
                }
            }
        }

        private static Task Invoke(object hubClient, MethodInfo method, object[] parameters)
        {
            return Task.Run(() =>
            {
                var typedParameters = new object[parameters.Length];
                var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();

                for (var i = 0; i < typedParameters.Length; i++)
                {
                    var targetType = methodParameterTypes[i];
                    if (parameters[i].GetType() != targetType)
                    {
                        return;
                        //Console.WriteLine($"Attempting to invoke callback for event type: {targetType.Name} / Current event type: {parameters[i].GetType()}");
                        //var serialized = JsonConvert.SerializeObject(parameters[i]);                        
                        //typedParameters[i] = JsonConvert.DeserializeObject(serialized, targetType);
                    }
                    else
                    {
                        typedParameters[i] = parameters[i];
                    }

                }

                method.Invoke(hubClient, typedParameters);
            });
        }
    }
}
