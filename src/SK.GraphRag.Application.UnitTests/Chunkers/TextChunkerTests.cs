using SK.GraphRag.Application.Chunkers;

namespace SK.GraphRag.Application.UnitTests.Chunkers;

public class TextChunkerTests
{
    private static readonly string[] NoOverlapExpected = ["abcd", "efgh", "ij"];
    private static readonly string[] OverlapExpected = ["abcd", "cdef", "efgh", "ghij"];

    [Fact]
    public void ChunkText_ReturnsEmptyArray_WhenTextIsNullOrEmpty()
    {
        TextChunker.ChunkText(null!).Should().BeEmpty();
        TextChunker.ChunkText("").Should().BeEmpty();
    }

    [Fact]
    public void ChunkText_Throws_ArgumentException_WhenChunkSizeInvalid()
    {
        FluentActions.Invoking(() => TextChunker.ChunkText("abc", 0)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkText("abc", -1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChunkText_Throws_ArgumentException__WhenOverlapInvalid()
    {
        FluentActions.Invoking(() => TextChunker.ChunkText("abc", 3, -1)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkText("abc", 3, 3)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkText("abc", 3, 4)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChunkText_BasicChunking_NoOverlap()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkText(text, 4, 0);
        chunks.Should().BeEquivalentTo(NoOverlapExpected);
    }

    [Fact]
    public void ChunkText_BasicChunking_WithOverlap()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkText(text, 4, 2);
        chunks.Should().BeEquivalentTo(OverlapExpected);
    }

    [Theory]
    [InlineData("abcdef", 3, 0, "abc", "def")]
    [InlineData("abcdefg", 2, 1, "ab", "bc", "cd", "de", "ef", "fg")]
    [InlineData("abcd", 3, 2, "abc", "bcd")]
    [InlineData("abcdefg", 3, 2, "abc", "bcd", "cde", "def", "efg")]
    [InlineData("a b c d", 3, 0, "a b", " c ", "d")]
    [InlineData("the quick brown fox jumps over the lazy dog", 5, 2,
        "the q",
        " quic",
        "ick b",
        " brow",
        "own f",
        " fox ",
        "x jum",
        "umps ",
        "s ove",
        "ver t",
        " the ",
        "e laz",
        "azy d",
        " dog")]
    public void ChunkText_SplitOnTextOnly_TheoryCases(string text, int chunkSize, int overlap, params string[] expected)
    {
        var result = TextChunker.ChunkText(text, chunkSize, overlap, false);
        result.Should().BeEquivalentTo(expected);
    }

    /*
    [Theory]
    [InlineData("abcdef", 3, 0, new[] { "abc", "def" })]
    [InlineData("abcdefg", 2, 1, new[] { "ab", "bc", "cd", "de", "ef", "fg" })]
    [InlineData("a b c d", 3, 0, new[] { "a b", " c ", "d" })]
    //[InlineData("the quick brown fox jumps over the lazy dog", 4, 2, new[] { "a b", " c ", "d" })]
    //[InlineData("the quick brown fox jumps over the lazy dog", 4, 2, new[] 
    //            { "the ", " qui", "d" })]
    //[InlineData("012345678901234567890012345678901234567890", 4, 2, new[] { "a b", " c ", "d" })]
    public void ChunkText_SplitOnWhitespaceOnly_TheoryCases(string text, int chunkSize, int overlap, string[] expected)
    {
        var result = TextChunker.ChunkText(text, chunkSize, overlap, true);
        result.Should().BeEquivalentTo(expected);
    }
    */
}
