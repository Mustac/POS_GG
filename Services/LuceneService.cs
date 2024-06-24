using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

public class LuceneService
{
    private readonly Lucene.Net.Store.Directory _indexDirectory;
    private readonly StandardAnalyzer _analyzer;
    private readonly IndexWriter _writer;
    private readonly ILogger<LuceneService> _logger;

    public LuceneService(ILogger<LuceneService> logger)
    {
        _indexDirectory = new RAMDirectory();
        _analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
        _writer = new IndexWriter(_indexDirectory, _analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        _logger = logger;
    }

    public void AddToIndex(ProductInfo product)
    {
        var doc = new Document();
        doc.Add(new Field("Name", product.Name.ToUpper(), Field.Store.YES, Field.Index.ANALYZED));
        doc.Add(new Field("CategoryName", product.CategoryName.ToUpper(), Field.Store.YES, Field.Index.ANALYZED));
        _writer.AddDocument(doc);
        _writer.Commit();
        _logger.LogInformation($"Indexed Product: Name={product.Name.ToUpper()}, CategoryName={product.CategoryName.ToUpper()}");
    }

    public List<string> Search(string query, float minSimilarity = 0.7f)
    {
        var results = new List<string>();
        var searcher = new IndexSearcher(_indexDirectory, readOnly: true);

        // Convert query to uppercase to ensure case-insensitive search
        query = query.ToUpper();
        _logger.LogInformation($"Searching for: {query}");

        // Exact match using TermQuery
        var termQuery = new TermQuery(new Term("Name", query));
        var termHits = searcher.Search(termQuery, 10).ScoreDocs;

        foreach (var hit in termHits)
        {
            var foundDoc = searcher.Doc(hit.Doc);
            results.Add(foundDoc.Get("Name"));
            _logger.LogInformation($"Term match found: {foundDoc.Get("Name")}");
        }

        // If no exact matches found, perform fuzzy search
        if (results.Count == 0)
        {
            var fuzzyQuery = new FuzzyQuery(new Term("Name", query), minSimilarity);
            var fuzzyHits = searcher.Search(fuzzyQuery, 10).ScoreDocs;

            foreach (var hit in fuzzyHits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                results.Add(foundDoc.Get("Name"));
                _logger.LogInformation($"Fuzzy match found: {foundDoc.Get("Name")}");
            }
        }

        if (results.Count == 0)
        {
            _logger.LogInformation("No matches found.");
        }

        return results;
    }
}
