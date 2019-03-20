﻿using Autowired.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppServiceExtensions
    {
        /// <summary>
        /// 注册应用程序域中所有有AppService特性的类
        /// </summary>
        /// <param name="services"></param>
        public static void AddAppServices(this IServiceCollection services)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    var serviceAttribute = type.GetCustomAttribute<AppServiceAttribute>();

                    if (serviceAttribute != null)
                    {
                        var serviceType = serviceAttribute.ServiceType;
                        if (serviceType == null && serviceAttribute.InterfaceServiceType)
                        {
                            serviceType = type.GetInterfaces().FirstOrDefault();
                        }
                        if (serviceType == null)
                        {
                            serviceType = type;
                        }
                        switch (serviceAttribute.Lifetime)
                        {
                            case ServiceLifetime.Singleton:
                                services.AddSingleton(serviceType, type);
                                break;
                            case ServiceLifetime.Scoped:
                                services.AddScoped(serviceType, type);
                                break;
                            case ServiceLifetime.Transient:
                                services.AddTransient(serviceType, type);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

        }
    }
}