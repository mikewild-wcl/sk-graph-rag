﻿using SK.GraphRag.Application.Chunkers;

namespace SK.GraphRag.Application.UnitTests.Chunkers;

public class TextChunkerTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;
    
    private static readonly string[] NoOverlapExpected = ["abcd", "efgh", "ij"];
    private static readonly string[] OverlapOneExpected = ["abcde", "defghi", "hij"];
    private static readonly string[] OverlapTwoExpected = ["abcdef", "cdefghij","ghij"];

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
    public void ChunkText_BasicChunking_WithOverlapOfOneCharacter()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkText(text, 4, 1);
        chunks.Should().BeEquivalentTo(OverlapOneExpected);
    }

    [Fact]
    public void ChunkText_BasicChunking_WithOverlapOfTwoCharacters()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkText(text, 4, 2);
        chunks.Should().BeEquivalentTo(OverlapTwoExpected);
    }

    [Theory]
    [InlineData("abcdef", 3, 0, "abc", "def")]
    [InlineData("abcdefg", 2, 1, "abc", "bcde", "defg", "fg")]
    [InlineData("abcd", 3, 2, "abcd", "bcd")]
    [InlineData("abcdefg", 3, 2, "abcde", "bcdefg", "efg")]
    [InlineData("a b c d", 3, 0, "a b", "c", "d")]
    [InlineData("the quick brown fox jumps over the lazy dog", 5, 2,
        "the qui",
        "quick br",
        "k brown f",
        "wn fox ju",
        "x jumps o",
        "ps over t",
        "er the la",
        "e lazy do",
        "y dog")]
    public void ChunkText_SplitOnTextOnly_TheoryCases(string text, int chunkSize, int overlap, params string[] expected)
    {
        var result = TextChunker.ChunkText(text, chunkSize, overlap);

        _output.WriteLine($"Actual chunks ({result.Length}):");
        foreach (var chunk in result)
        {
            _output.WriteLine($"  \"{chunk}\"");
        }

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_ReturnsEmptyArray_WhenTextIsNullOrEmpty()
    {
        TextChunker.ChunkTextOnWhitespaceOnly(null!).Should().BeEmpty();
        TextChunker.ChunkTextOnWhitespaceOnly("").Should().BeEmpty();
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_Throws_ArgumentException_WhenChunkSizeInvalid()
    {
        FluentActions.Invoking(() => TextChunker.ChunkTextOnWhitespaceOnly("abc", 0)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkTextOnWhitespaceOnly("abc", -1)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_Throws_ArgumentException__WhenOverlapInvalid()
    {
        FluentActions.Invoking(() => TextChunker.ChunkTextOnWhitespaceOnly("abc", 3, -1)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkTextOnWhitespaceOnly("abc", 3, 3)).Should().Throw<ArgumentOutOfRangeException>();
        FluentActions.Invoking(() => TextChunker.ChunkTextOnWhitespaceOnly("abc", 3, 4)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_BasicChunking_NoOverlap_WhenNoWhitespace()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkTextOnWhitespaceOnly(text, 4, 0);
        chunks.Should().BeEquivalentTo(text);
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_BasicChunking_WithOverlapOfOneCharacter_WhenNoWhitespace()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkTextOnWhitespaceOnly(text, 4, 1);
        chunks.Should().BeEquivalentTo(text);
    }

    [Fact]
    public void ChunkTextOnWhitespaceOnly_BasicChunking_WithOverlapOfTwoCharacters_WhenNoWhitespace()
    {
        var text = "abcdefghij";
        var chunks = TextChunker.ChunkTextOnWhitespaceOnly(text, 4, 2);
        chunks.Should().BeEquivalentTo(text);
    }

    [Theory]
    [InlineData("abc def", 3, 0, "abc", "def")]
    [InlineData("abcdefg", 2, 1, "abcdefg")]
    [InlineData("a b c d", 3, 0, "a b", "c d")]
    [InlineData("the quick brown fox jumps over the lazy dog", 5, 2,
        "the quick",
        "quick brown",
        "brown fox jumps",
        "jumps over the",
        "the lazy dog")]
    public void ChunkTextOnWhitespaceOnly_SplitOnTextOnly_TheoryCases(string text, int chunkSize, int overlap, params string[] expected)
    {
        var result = TextChunker.ChunkTextOnWhitespaceOnly(text, chunkSize, overlap);

        _output.WriteLine($"Actual chunks ({result.Length}):");
        foreach (var chunk in result)
        {
            _output.WriteLine($"  \"{chunk}\"");
        }

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Split_Test()
    {
        var text = "Einstein’s Patents and Inventions  Asis Kumar Chaudhuri Variable Energy Cyclotron Centre 1‐AF Bidhan Nagar, Kolkata‐700 064   Abstract: Times magazine selected Albert Einstein, the German born Jewish Scientist as the person of the 20th century. Undoubtedly, 20th century was the age of science and Einstein’s contributions in unravelling mysteries of nature was unparalleled. However, few are aware that Einstein was also a great inventor. He and his collaborators had patented a wide variety of";
        var result = TextChunker.ChunkTextOnWhitespaceOnly(text, 20, 8);

        _output.WriteLine($"Actual chunks ({result.Length}):");
        foreach (var chunk in result)
        {
            _output.WriteLine($"  \"{chunk}\"");
        }

        /*
"Einstein’s Patents and"
  "Patents and Inventions  Asis Kumar"
  "Asis Kumar Chaudhuri Variable Energy"
  "Energy Cyclotron Centre 1‐AF"
  "Centre 1‐AF Bidhan Nagar, Kolkata‐700"
  "Kolkata‐700 064   Abstract: Times"
  "Abstract: Times magazine selected Albert"
  "Albert Einstein, the German"
  "German born Jewish Scientist"
  "Scientist as the person of the"
  "of the 20th century. Undoubtedly,"
  "Undoubtedly, 20th century was the"
  "was the age of science and Einstein’s"
  "Einstein’s contributions in unravelling"
  "unravelling mysteries of nature was"
  "nature was unparalleled. However,"
  "However, few are aware that Einstein"
  "Einstein was also a great inventor."
  "inventor. He and his collaborators"
  "collaborators had patented a wide variety"
  "variety of"
*/
    string[] expected = [
        "Einstein’s Patents and",
        "Patents and Inventions  Asis Kumar",
        "Asis Kumar Chaudhuri Variable Energy",
        "Energy Cyclotron Centre 1‐AF",
        "Centre 1‐AF Bidhan Nagar, Kolkata‐700",
        "Kolkata‐700 064   Abstract: Times",
        "Abstract: Times magazine selected Albert",
        "Albert Einstein, the German",
        "German born Jewish Scientist",
        "Scientist as the person of the",
        "of the 20th century. Undoubtedly,",
        "Undoubtedly, 20th century was the",
        "was the age of science and Einstein’s",
        "Einstein’s contributions in unravelling",
        "unravelling mysteries of nature was",
        "nature was unparalleled. However,",
        "However, few are aware that Einstein",
        "Einstein was also a great inventor.",
        "inventor. He and his collaborators",
        "collaborators had patented a wide variety",
        "variety of"
        ];

        for(var i = 0; i < result.Length; i++)
        {
            _output.WriteLine($"Expected: \"{expected[i]}\"; Actual: \"{result[i]}\"");
            result[i].Should().Be(expected[i]);
        }
        result.Should().BeEquivalentTo(expected);
        /*
         */
    }
}
