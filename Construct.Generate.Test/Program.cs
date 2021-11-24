using System;
using System.Collections.Generic;
using System.Linq;
using Construct.Core.Configuration;
using Construct.Core.Database.Context;
using Construct.Core.Database.Model;
using Construct.Core.Logging;
using Construct.Generate.Test.Random;
using Microsoft.Extensions.Logging;

namespace Construct.Generate.Test
{
    public class Program
    {
        /// <summary>
        /// Random users to generate.
        /// </summary>
        public static readonly IntRange RandomUsers = new IntRange(2000, 5000);

        /// <summary>
        /// Random generator for the strings.
        /// </summary>
        public static readonly RandomString RandomStrings = new RandomString(new IntRange(3, 30));

        /// <summary>
        /// Random visits to generate for each user.
        /// </summary>
        public static readonly IntRange RandomVisits = new IntRange(0, 500);
        
        /// <summary>
        /// Random prints to generate for each user.
        /// </summary>
        public static readonly IntRange RandomPrints = new IntRange(0, 100);
        
        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">Arguments from the command line.</param>
        public static void Main(string[] args)
        {
            // Load the configuration.
            Log.Initialize("Generate");
            ConstructConfiguration.LoadDefaultAsync().Wait();
            Log.SetMinimumLogLevel(LogLevel.Information);

            // Open the context and cancel if the database is not empty.
            using var context = new ConstructContext();
            context.EnsureUpToDateAsync().Wait();
            if (context.Users.Count() != 0)
            {
                Log.Error("Database is not empty. Generating data is not safe.");
                return;
            }
            
            // Create the random materials.
            var random = new System.Random();
            var materials = new List<PrintMaterial>();
            for (var i = 0; i < 10; i++)
            {
                var material = new PrintMaterial()
                {
                    Name = RandomStrings.Next(),
                    CostPerGram = i * 0.01f,
                };
                context.PrintMaterials.Add(material);
                materials.Add(material);
            }
            
            // Create the random users.
            var totalUsers = RandomUsers.Next();
            var boolRandomizer = new RandomBool();
            var dateRandomizer = new RandomDateTime();
            Log.Info("Creating " + totalUsers + " users.");
            for (var i = 0; i < totalUsers; i++)
            {
                // Add the user.
                var user = new User()
                {
                    HashedId = Guid.NewGuid().ToString(),
                    Name = RandomStrings.Next() + " " + RandomStrings.Next(),
                    Email = RandomStrings.NextAscii() + "@" + RandomStrings.NextAscii(),
                    Permissions = new List<Permission>(),
                    PrintLogs = new List<PrintLog>(),
                    VisitLogs = new List<VisitLog>(),
                };
                context.Users.Add(user);
                
                // Randomly add the sign-up time.
                if (boolRandomizer.Next())
                {
                    user.SignUpTime = dateRandomizer.Next();
                }
                
                // Randomly add the student information.
                if (boolRandomizer.Next())
                {
                    context.Students.Add(new Student()
                    {
                        User = user,
                        College = RandomStrings.Next(),
                        Year = RandomStrings.Next(),
                    });
                }
                
                // Randomly add the visit logs.
                var totalRandomVisits = RandomVisits.Next();
                for (var j = 0; j < totalRandomVisits; j++)
                {
                    user.VisitLogs.Add(new VisitLog()
                    {
                        User = user,
                        Time = dateRandomizer.Next(),
                        Source = RandomStrings.Next(),
                    });
                }

                // Randomly add print logs.
                var totalRandomPrints = RandomPrints.Next();
                for (var j = 0; j < totalRandomPrints; j++)
                {
                    user.PrintLogs.Add(new PrintLog()
                    {
                        User = user,
                        Time = dateRandomizer.Next(),
                        FileName = RandomStrings.Next() + ".gcode",
                        Material = materials[random.Next(0, materials.Count)],
                        WeightGrams = (float) (100 * random.NextDouble()),
                        Purpose = RandomStrings.Next(),
                        BillTo = (boolRandomizer.Next() ? RandomStrings.Next() : null),
                        Cost = (float) (100 * random.NextDouble()),
                        Owed = boolRandomizer.Next(),
                    });
                }
                
                // Log the user.
                Log.Info("Creating user " + (i + 1) + " " + user.Name + " with " + totalRandomVisits + " visits and " + totalRandomPrints + " prints.");
            }
            
            // Save the users.
            Log.Info("Saving changes.");
            context.SaveChanges();
            Log.Info("Data generated.");
        }
    }
}