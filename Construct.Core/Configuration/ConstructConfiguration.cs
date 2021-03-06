using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Construct.Core.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Construct.Core.Configuration
{
    public class Logging
    {
        /// <summary>
        /// Minimum log level for the console.
        /// </summary>
        public LogLevel ConsoleLevel { get; set; } = LogLevel.Information;
    }
    
    public class Database
    {
        /// <summary>
        /// Database provider to use.
        /// </summary>
        public string Provider { get; set; } = "sqlite";
        
        /// <summary>
        /// Non-default data source to use. Can be a URL or a file name.
        /// For URLs, the port must not be included.
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// Port to connect to the host (source). Not used with Sqlite.
        /// </summary>
        public int? SourcePort { get; set; }
        
        /// <summary>
        /// Database in the host (source). Not used with Sqlite.
        /// </summary>
        public string SourceDatabase { get; set; }
        
        /// <summary>
        /// Username to access the data source.
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// Password to access the data source.
        /// </summary>
        public string Password { get; set; }
    }
    
    public class Email
    {
        /// <summary>
        /// Emails that are valid for users.
        /// </summary>
        public List<string> ValidEmails { get; set; }= new List<string>();

        /// <summary>
        /// Changes to make to emails to make them valid.
        /// </summary>
        public Dictionary<string, string> EmailCorrections { get; set; } = new Dictionary<string, string>();
    }

    public class PrintReceipt
    {
        /// <summary>
        /// Provider of the print receipt providers.
        /// </summary>
        public string Provider { get; set; } = "GoogleAppScripts";

        /// <summary>
        /// Script id to use with Google App Scripts.
        /// </summary>
        public string GoogleAppScriptId { get; set; } = "script_id";
    }

    public class Admin
    {
        /// <summary>
        /// Maximum sessions a user can have with the admin user interface.
        /// </summary>
        public int MaximumUserSessions { get; set; } = 5;

        /// <summary>
        /// Maximum duration of a session that a user can have with the admin user interface.
        /// </summary>
        public int MaximumUserSessionDuration { get; set; } = 60 * 60;

        /// <summary>
        /// Permissions that are shown in the admin user interface.
        /// </summary>
        public List<string> ConfigurablePermissions { get; set; } = new List<string>() { "LabManager", };
    }
    
    public class ConstructConfiguration
    {
        /// <summary>
        /// Default file location of the configuration.
        /// </summary>
        public const string FileLocation = "configuration.json";

        /// <summary>
        /// Static configuration used for the application.
        /// </summary>
        public static ConstructConfiguration Configuration = new ConstructConfiguration();

        /// <summary>
        /// Database configuration of the application.
        /// </summary>
        public Database Database { get; } = new Database();

        /// <summary>
        /// Logging configuration of the application.
        /// </summary>
        public Logging Logging { get; } = new Logging();

        /// <summary>
        /// Email configuration of the application.
        /// </summary>
        public PrintReceipt PrintReceipt { get; } = new PrintReceipt();

        /// <summary>
        /// Print receipt configuration of the application.
        /// </summary>
        public Email Email { get; } = new Email();

        /// <summary>
        /// Admin configuration of the application.
        /// </summary>
        public Admin Admin { get; } = new Admin();

        /// <summary>
        /// Ports used by the services.
        /// </summary>
        public Dictionary<string, int> Ports { get; } = new Dictionary<string, int>()
        {
            { "Combined", 8000 },
            { "User", 8001 },
            { "Swipe", 8002 },
            { "Admin", 8003 },
            { "Print", 8004 },
            { "Compatibility", 8005 },
        };

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        /// <param name="fileLocation">Location of the configuration.</param>
        public async Task SaveAsync(string fileLocation = FileLocation)
        {
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
            var defaultConfiguration = JsonConvert.SerializeObject(this,
                new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>() { new StringEnumConverter() },
                });
            await File.WriteAllTextAsync(fileLocation, defaultConfiguration);
        }

        /// <summary>
        /// Loads a configuration.
        /// </summary>
        /// <param name="fileLocation">Location of the configuration.</param>
        /// <returns>The loaded configuration.</returns>
        public static async Task<ConstructConfiguration> LoadAsync(string fileLocation = FileLocation)
        {
            // Write the default configuration if none exists.
            if (!File.Exists(fileLocation))
            {
                Log.Warn($"Configuration file {fileLocation} not found. Writing default.");
                await new ConstructConfiguration().SaveAsync();
            }
            
            // Read and return the configuration.
            var configurationData = await File.ReadAllTextAsync(fileLocation);
            return JsonConvert.DeserializeObject<ConstructConfiguration>(configurationData);
        }

        /// <summary>
        /// Loads the static configuration.
        /// </summary>
        /// <param name="fileLocation">Location of the configuration.</param>
        public static async Task LoadDefaultAsync(string fileLocation = FileLocation)
        {
            Configuration = await LoadAsync(fileLocation);
        }
    }
}