using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Construct.Core.Attribute;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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
                Log.Trace($"Found {pathAttributes.Count} Path attributes for {method.ReflectedType?.Name}.{method.Name}");
                
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
            Log.Trace("Setting up ASP.NET Core MVC and assemblies.");
            var mvc = services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                mvc.AddApplicationPart(assembly);
            }
        }

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
                Log.Trace("Enabling exception pages for developing.");
            }
            app.UseDeveloperExceptionPage();

            // Set up the static files.
            var staticFiles = Path.GetFullPath("web");
            if (Directory.Exists(staticFiles))
            {
                Log.Debug($"Setting up static files at {staticFiles}.");
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(staticFiles),
                    RequestPath = "",
                });
            }

            // Add the MVC controllers.
            app.UseMvc(routes =>
            {
                var handlers = GetRequestHandlerMethods();
                Log.Debug($"Registering {handlers.Count} API request handlers.");
                foreach (var (path, handlerMethod) in handlers)
                {
                    // Get the base controller name.
                    var controllerType = handlerMethod.ReflectedType;
                    var fullControllerNamespace = controllerType?.Namespace ?? "";
                    var fullControllerName = controllerType?.Name ?? "";
                    if (!fullControllerName.EndsWith("Controller"))
                    {
                        Log.Warn($"Controller class must end in Controller (can't register): {fullControllerNamespace}.{fullControllerName}");
                        continue;
                    }
                    var controllerName = fullControllerName.Substring(0, fullControllerName.LastIndexOf("Controller", StringComparison.Ordinal));
                    
                    // Add the route.
                    Log.Debug($"Registering {path} with handler {fullControllerNamespace}.{controllerName}.{handlerMethod.Name}");
                    routes.MapRoute( $"{fullControllerName}_{handlerMethod.Name}", path,
                        new {controller = controllerName, action = handlerMethod.Name});
                }
            });

            // Set up returning index.html for paths.
            // Done after the MVC setup so that controllers can handle requests first.
            if (Directory.Exists(staticFiles))
            {
                app.Use(async (context, next) =>
                {
                    // Get the index.html for the path.
                    var path = context.Request.Path.ToString().Substring(1);
                    var indexHtml = Path.Combine(staticFiles, path, "index.html");
                    
                    // Send the file if it is found.
                    if (File.Exists(indexHtml))
                    {
                        context.Response.Headers.Add("Content-Type", "text/html");
                        await context.Response.Body.WriteAsync(await File.ReadAllBytesAsync(indexHtml));
                        return;
                    }
                    
                    // Continue to the next route.
                    await next.Invoke();
                });
            }
        }
    }
}