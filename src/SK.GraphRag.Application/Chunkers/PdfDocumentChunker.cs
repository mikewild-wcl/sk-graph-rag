using SK.GraphRag.Application.Chunkers.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace SK.GraphRag.Application.Chunkers;

public class PdfDocumentChunker : IDocumentChunker
{
    public async IAsyncEnumerable<string> StreamTextChunks(
        string filePath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            yield break;
        }

        /*
         * Improvement: track the page number for each chunk, possibly by having a dictionary with the index 
         * in documentText (.Length before the text is appended?) of the text , 
         * and return the details as metadata in a tuple with the text:
         *  IAsyncEnumerable<(int PageNumber, int IndexOnPage, string Text)>
         */

        var documentText = new StringBuilder();
        List<(int, IPdfImage)> imageList = [];

        using (var document = PdfDocument.Open(filePath))
        {
            foreach (var page in document.GetPages())
            {
                var letters = page.Letters;
                var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);
                var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);
                var pageText = string.Join(
                    string.Empty,
                    textBlocks.Select(t => t.Text.ReplaceLineEndings(" ")).ToArray());

                //documentText.Append(string.Join(string.Empty, letters.Select(x => x.Value)));
                documentText.Append(pageText);

                var images = page.GetImages();
                images?.ToList().ForEach(img => imageList.Add((page.Number, img)));
            }
        }

        foreach (var chunk in TextChunker.ChunkTextOnWhitespaceOnly(documentText.ToString()))
        {
            yield return chunk;
        }
    }

    /*
    public async IAsyncEnumerable<DocumentChunk> StreamChunks(Stream stream, string documentUri)
    {
        //if (string.IsNullOrEmpty(documentUri))
        //{
        //    yield break;
        //}

        //if(!File.Exists(documentUri))
        //{
        //    yield break;
        //}

        using var pdf = PdfDocument.Open(stream);
        var pages = pdf.GetPages();
        var paragraphs = pages.SelectMany(GetPageParagraphs);

        foreach (var paragraph in paragraphs)
        {
            yield return new DocumentChunk
            {
                Key = Guid.NewGuid().ToString(),
                DocumentUri = documentUri,
                //PageNumber = paragraph.PageNumber,
                //Index = paragraph.IndexOnPage,
                ParagraphId = string.Empty,
                Text = paragraph.Text
            };
        }
    }

    private static IEnumerable<(int PageNumber, int IndexOnPage, string Text)> GetPageParagraphs(Page pdfPage)
    {
        var letters = pdfPage.Letters;
        var words = NearestNeighbourWordExtractor.Instance.GetWords(letters);

        //TODO: use a filter on PdfDocument.Open to do this
        var sep = Convert.ToChar(61623);
        var cleanedWords = words
            .Where(word => !(word.Letters?.Count > 0 &&
                             word.Letters[0]?.Value?.Length == 1 &&
                             word.Letters[0].Value[0] == sep))
            .ToList();

        //foreach (var word in words)
        //{
        //    if (word.Letters?.Count == 1 && word.Letters[0]?.Value?.Length == 1 && word.Letters[0].Value[0] == Convert.ToChar(61623))
        //    {
        //        // Skip single 'x' letters, which are often used as checkboxes or similar markers.
        //    }
        //}

        var textBlocks = DocstrumBoundingBoxes.Instance.GetBlocks(cleanedWords);
        var pageText = string.Join(Environment.NewLine + Environment.NewLine,
            textBlocks.Select(t => t.Text.ReplaceLineEndings(" ")));

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only
        return TextChunker.SplitPlainTextParagraphs([pageText], Constants.MaxTokensPerParagraph)
            .Select((text, index) => (pdfPage.Number, index, text));
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only
    }
    */
}
