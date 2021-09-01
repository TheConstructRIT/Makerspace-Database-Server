using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Construct.Core.Attribute;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Construct.Core.Server
{
    public class Startup
    {
        /// <summary>
        /// Returns the request handler methods in given type.
        /// The returned list of pairs is the path and method for the handler.
        /// </summary>
        /// <param name="type">Type to get the handlers of.</param>
        public static List<(string, MethodInfo)> GetRequestHandlerMethods(Type type)
        {
            // Iterate over methods and add the request handlers.
            var handlers = new List<(string, MethodInfo)>();
            foreach (var method in type.GetMethods())
            {
                // Ignore the method if there are no paths specified.
                var pathAttributes = method.GetCustomAttributes<PathAttribute>().ToList();
                if (pathAttributes.Count == 0) continue;
                
                // Add the method for all the paths.
                foreach (var pathAttribute in pathAttributes)
                {
                    handlers.Add((pathAttribute.Path, method));
                }
            }
            
            // Return the handlers.
            return handlers;
        }
    
        /// <summary>
        /// Returns the request handler methods in the current assembly.
        /// The returned list of pairs is the path and method for the handler.
        /// </summary>
        public static List<(string, MethodInfo)> GetRequestHandlerMethods()
        {
            // Iterate over the types and add the request handlers.
            var handlers = new List<(string, MethodInfo)>();
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()))
            {
                handlers.AddRange(GetRequestHandlerMethods(type));
            }
            
            // Return the handlers.
            return handlers;
        }
        
        /// <summary>
        /// Adds services to the application.
        /// </summary>
        /// <param name="services">Collection of services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC for controllers and add the assemblies for controllers.
            var mvc = services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                mvc.AddApplicationPart(assembly);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the application and HTTP request handling. 
        /// </summary>
        /// <param name="app">App to configure.</param>
        /// <param name="environment">Environment to configure.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            // Set up developer exceptions when in development.
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add the MVC controllers.
            app.UseMvc(routes =>
            {
                foreach (var (path, handlerMethod) in GetRequestHandlerMethods())
                {
                    // Get the base controller name.
                    var controllerType = handlerMethod.ReflectedType;
                    var fullControllerName = controllerType?.Name ?? "";
                    if (!fullControllerName.EndsWith("Controller")) continue;
                    var controllerName = fullControllerName.Substring(0, fullControllerName.LastIndexOf("Controller", StringComparison.Ordinal));
                    
                    // Add the route.
                    routes.MapRoute(fullControllerName + "_" + handlerMethod.Name, path,
                        new {controller = controllerName, action = handlerMethod.Name});
                }
            });
        }
    }
}