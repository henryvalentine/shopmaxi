using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Ajax.Utilities;
using Shopkeeper.Datacontracts.CustomizedDataObjects;

namespace ShopKeeper.GenericHelpers
{
    public static class DBCreator
    {
        public static bool CreateDB(DBObject dbObject)
        {
            try
            {
                if (dbObject == null || dbObject.DBSize < 1)
                {
                    return false;
                }

                var fileContent = File.ReadAllText(dbObject.ScriptFilePath);

                var logfilesize = dbObject.DBSize*60/100;
                var dbsize = dbObject.DBSize - logfilesize;

                fileContent = fileContent.Replace("MAXSIZE = UNLIMITED", "MAXSIZE = " + dbsize + "MB");
                fileContent = fileContent.Replace("MAXSIZE = 2048GB", "MAXSIZE = " + logfilesize + "MB");
                fileContent = fileContent.Replace("ShopKeeper", dbObject.DBName);

                var regex = new Regex(@"\r{0,1}\nGO\r{0,1}\n");
                var commands = regex.Split(fileContent);
                
                using (var con = new SqlConnection(dbObject.ConnectionString))
                {
                    var cmd = new SqlCommand("query", con);
                    con.Open();
                    foreach (var query in commands)
                    {
                        if (string.IsNullOrEmpty(query)) continue;

                        var command = query.Trim();

                        if (command.Contains("GO"))
                        {
                            command = command.Replace("GO", "").Trim();
                            if (string.IsNullOrEmpty(command))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(command))
                            {
                                continue;
                            }
                        }

                        cmd.CommandText = command;
                        cmd.ExecuteNonQuery();
                    }

                }

                return true;
            }
            catch (Exception)
            {
              return false;
            }
        }

        public static long GetDatabaseSize(string connectionString, string dbName)
        {
            try
            {
                var con = new SqlConnection(connectionString);
                var cmd = new SqlCommand("query", con)
                {
                    CommandText =
                        "SELECT DB_NAME(database_id) AS DBName,Name AS Logical_Name, Physical_Name,(size*8)/1024 SizeMB FROM sys.master_files WHERE DB_NAME(database_id) = " + dbName
                };
               
                con.Open();
                var jj = cmd.ExecuteNonQuery();
                return jj;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        
    }
}