using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

public class CustomDependencyResolver : IDependencyResolver
{
    private readonly IServiceProvider _serviceProvider;

    public CustomDependencyResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object GetService(Type serviceType)
    {
        return _serviceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return _serviceProvider.GetServices(serviceType);
    }
}
