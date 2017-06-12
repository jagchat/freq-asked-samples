using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using System.Text.RegularExpressions;

namespace LuceneSearchDemo.Service.Library
{
    public class Utils
    {
        public static BooleanQuery QueryMaker(string searchString, string[] searchfields)
        {
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, searchfields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            var finalQuery = new BooleanQuery();
            string searchText;
            searchText = searchString.Replace("+", "");
            searchText = searchText.Replace("\"", "");
            searchText = searchText.Replace("\'", "");
            //Split the search string into separate search terms by word
            string[] terms = searchText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string term in terms)
            {
                if (searchString.Contains("+"))
                {
                    finalQuery.Add(parser.Parse(term.Replace("*", "") + "*"), Occur.MUST);
                }
                else
                {
                    finalQuery.Add(parser.Parse(term.Replace("*", "") + "*"), Occur.SHOULD);
                }
            }
            return finalQuery;
        }

        public static Query ParseQuery(string searchQuery, QueryParser parser)
        {
            parser.AllowLeadingWildcard = true;
            Query q;
            q = parser.Parse(searchQuery);

            if (q == null || string.IsNullOrEmpty(q.ToString()))
            {
                string cooked = Regex.Replace(searchQuery, @"[^\w\.@-]", " ");
                q = parser.Parse(cooked);
            }

            return q;
        }
    }
}
