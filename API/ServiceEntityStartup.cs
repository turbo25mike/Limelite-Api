using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Api.Controllers;
using Business.DataSources;
using Business.Validators;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Api;

public static class ServiceEntityStartup
{
    public static void AddEntityControllers(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        CreateEntityControllers<DBEntityAttribute>(assemblies, services);
    }

    private static void CreateEntityControllers<T>(Assembly[] assemblies, IServiceCollection services)
        where T : Attribute
    {
        var entities = from assembly in assemblies
                       from t in assembly.GetTypes()
                       where t.GetCustomAttributes<T>().Any()
                       where t.Name != nameof(Model)
                       select t;

        foreach (var entity in entities)
        {
            var validator = AssemblyReference.Get($"{entity.Name}Validator");
            var datasource = AssemblyReference.Get($"{entity.Name}DataSource");
            var idatasource = AssemblyReference.Get($"I{entity.Name}DataSource");

            services.AddTransient(typeof(IValidator<>).MakeGenericType(entity),
                validator ?? typeof(Validator<>).MakeGenericType(entity));
            services.AddTransient(idatasource ?? typeof(IDataSource<>).MakeGenericType(entity),
                datasource ?? typeof(DataSource<>).MakeGenericType(entity));
        }

        services
            .AddMvc(o => o.Conventions.Add(new GenericControllerRouteConvention()))
            .ConfigureApplicationPartManager(m =>
                m.FeatureProviders.Add(new GenericTypeControllerFeatureProvider(entities)));
    }
}

public class AssemblyReference
{
    public static Type Get(string name)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return (from assembly in assemblies
                from t in assembly.GetTypes()
                where t.Name == name
                select t).FirstOrDefault();
    }
}

public class GenericTypeControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly IEnumerable<Type> _types;

    public GenericTypeControllerFeatureProvider(IEnumerable<Type> types)
    {
        _types = types;
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var candidate in _types)
            if (AssemblyReference.Get($"{candidate.Name}Controller") == null)
                feature.Controllers.Add(typeof(BaseController<>).MakeGenericType(candidate).GetTypeInfo());
    }
}

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType)
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = genericType.Name;
            controller.Selectors[0].AttributeRouteModel.Template = genericType.Name.ToLower();
        }
    }
}