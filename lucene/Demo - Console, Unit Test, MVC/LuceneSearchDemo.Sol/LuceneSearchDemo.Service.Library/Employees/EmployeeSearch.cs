using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using LuceneSearchDemo.Common;
using NLog;
using Version = Lucene.Net.Util.Version;

namespace LuceneSearchDemo.Service.Library.Employees
{
    //TODO: quite a bit of refactoring is necessary
    public class EmployeeSearch
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private string indexFileLocation = Path.Combine(Common.Utils.GetLuceneIndexFolder(), @"Employees"); //TODO: configurable?
        private DataSet GetEmployeesAfterEmpno(int? lastEmpno = null)
        {
            logger.Trace("In GetEmployeesAfterEmpno - lastEmpno: " + (lastEmpno.HasValue ? lastEmpno.Value.ToString() : string.Empty));
            //TODO: refactor and implement this in a better way
            var sql = @"SELECT TOP 100 * FROM emp"; //100 rows at a time, configurable?
            if (lastEmpno.HasValue)
            {
                sql += " WHERE empno > " + lastEmpno.Value; //paging based on increasing empno in this case
            }
            return DbHelper.GetDataSet(sql);
        }

        public void CreateIndex()
        {
            logger.Trace("Create Index: STARTED..");
            var di = new DirectoryInfo(indexFileLocation);
            if (!di.Exists)
            {
                di.Create();
            }
            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(di.FullName);
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var indexWriter = new IndexWriter(dir, analyzer , IndexWriter.MaxFieldLength.UNLIMITED);

            //indexWriter.SetRAMBufferSizeMB(10.0);//TODO: configurable
            //indexWriter.UseCompoundFile = false;//TODO: configurable
            //indexWriter.MaxMergeDocs = 10000;//TODO: configurable
            //indexWriter.MergeFactor = 100;//TODO: configurable

            var ds = GetEmployeesAfterEmpno();
            while (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                var dt = ds.Tables[0];
                int lastEmpno = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    var doc = new Document();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        doc.Add(new Field(dc.ColumnName, dr[dc.ColumnName].ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    }
                    
                    //to remove duplicates if the index ran multiple times
                    var query = new TermQuery(new Term("Empno", dr["empno"].ToString()));
                    indexWriter.DeleteDocuments(query);

                    indexWriter.AddDocument(doc);
                    lastEmpno = Convert.ToInt16(dr["empno"]);
                }
                ds = GetEmployeesAfterEmpno(lastEmpno);
            }
            analyzer.Close();
            //indexWriter.Flush(true, true, true);
            indexWriter.Dispose();
            logger.Trace("Create Index: COMPLETED..");
        }

        public List<EmployeeSearchResultItem> PerformSearch(string searchString)
        {
            logger.Trace("Performing Search: STARTED..");
            // Results are collected as a List
            var searchResults = new List<EmployeeSearchResultItem>();

            // Specify the location where the index files are stored
            var di = new DirectoryInfo(indexFileLocation);
            Lucene.Net.Store.Directory dir = Lucene.Net.Store.FSDirectory.Open(di.FullName);
            string[] searchfields = new string[] { "Empno", "Ename", "Sal", "Deptno" };
            IndexSearcher indexSearcher = new IndexSearcher(dir);

            

            //Multi-field searcher
            var parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, searchfields, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30));
            
            ////Single Field Searcher
            //var parser = new QueryParser(Version.LUCENE_30, "Ename", new StandardAnalyzer(Version.LUCENE_30));

            var hits = indexSearcher.Search(Utils.ParseQuery(searchString, parser), null, 10, Sort.RELEVANCE);
            //var hits = indexSearcher.Search(Utils.QueryMaker(searchString, searchfields), 10);
            //var hits = indexSearcher.Search(Utils.QueryMaker(searchString, searchfields), null, 10, Sort.RELEVANCE);

            for (int i = 0; i < hits.TotalHits; i++)
            {
                var resultItem = new EmployeeSearchResultItem();
                var doc = indexSearcher.Doc(hits.ScoreDocs[i].Doc);

                resultItem.Empno = doc.Get("Empno");
                resultItem.Ename = doc.Get("Ename");
                resultItem.Sal = doc.Get("Sal");
                resultItem.Deptno = doc.Get("Deptno");
                searchResults.Add(resultItem);
            }

            indexSearcher.Dispose();
            logger.Trace("Performing Search: Completed..");
            return searchResults;
        }
    }
}
