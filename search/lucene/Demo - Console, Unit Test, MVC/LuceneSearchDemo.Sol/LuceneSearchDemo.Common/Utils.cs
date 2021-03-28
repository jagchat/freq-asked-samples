using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuceneSearchDemo.Common
{
    public class Utils
    {
        public static string GetConnectionString()
        {
            //TODO: configurable
            return @"Data Source=.\sql2k16;Database=SampleDb;User Id=sa;Password=eXpress2016";
        }

        public static string GetLuceneIndexFolder()
        {
            //TODO: configurable
            return @"C:\t\LuceneIndexes";
        }
    }
}
