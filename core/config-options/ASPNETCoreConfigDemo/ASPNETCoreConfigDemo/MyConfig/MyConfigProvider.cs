using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASPNETCoreConfigDemo.MyConfig
{
    public class MyConfigProvider : ConfigurationProvider
    {
        public MyConfigSource Source { get; }

        public MyConfigProvider(MyConfigSource source)
        {
            Source = source;
        }

        public override void Load()
        {
            ////OPTION: can do a sql query to load config
            //using var conn = new SQLiteConnection(Source.ConnectionString);
            //conn.Open();
            //using var cmd = new SQLiteCommand(Source.Query, conn);
            //using var reader = cmd.ExecuteReader();
            //while (reader.Read())
            //{
            //    Set(reader.GetString(0), reader.GetString(1));
            //}

            Set("MyCustomOption1", $"{Source.Key}-value1"); //set some dummy config..using value from previous config.
        }

    }
}
