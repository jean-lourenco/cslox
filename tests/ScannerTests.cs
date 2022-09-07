using FluentAssertions;
using Lox;
using Xunit;

namespace LoxTests;

public class ScannerTests
{
    [Fact]
    public void Scanner_should_handle_double_char_tokens()
    {
        var program = @"!= == <= >=";
        var scanner = new Scanner(program);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(5);
        tokens[0].Type.Should().Be(TokenType.BangEqual);
        tokens[1].Type.Should().Be(TokenType.EqualEqual);
        tokens[2].Type.Should().Be(TokenType.LessEqual);
        tokens[3].Type.Should().Be(TokenType.GreaterEqual);
        tokens[4].Type.Should().Be(TokenType.EOF);
    }

    [Fact]
    public void Scanner_should_handle_literal_string()
    {
        var source = @"""John Hathorne""";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Type.Should().Be(TokenType.String);
        tokens[0].Literal.Should().Be("John Hathorne");
        tokens[1].Type.Should().Be(TokenType.EOF);
    }

    [Fact]
    public void Scanner_should_handle_multiline_literal_string()
    {
        var source = @"""my
multline
string""";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Type.Should().Be(TokenType.String);
        tokens[0].Lexeme.Should().Be(source);
    }

    [Fact(Skip = "LoxEntryPoint and Scanner should be refactored so they don't have static instances")]
    public void Scanner_should_handle_unterminated_strings_as_errors()
    {
        var source = @"""my unterminated string";
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(1);
        // LoxEntryPoint.HasErrors.Should().BeTrue();
    }

    [Theory]
    [InlineData("123")]
    [InlineData("456.789")]
    public void Scanner_should_handle_literal_numbers(string literalNumber)
    {
        var scanner = new Scanner(literalNumber);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(2);
        tokens[0].Type.Should().Be(TokenType.Number);
        tokens[0].Literal.Should().Be(double.Parse(literalNumber));
        tokens[1].Type.Should().Be(TokenType.EOF);
    }

    [Fact]
    public void Scanner_should_not_scan_dot_starting_number_literal_as_number()
    {
        var literalNumber = ".123";
        var scanner = new Scanner(literalNumber);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(3);
        tokens[0].Type.Should().Be(TokenType.Dot);
        tokens[1].Type.Should().Be(TokenType.Number);
        tokens[1].Literal.Should().Be(123);
    }

    [Fact]
    public void Scanner_should_not_scan_dot_ending_number_literal_as_number()
    {
        var literalNumber = "123.";
        var scanner = new Scanner(literalNumber);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(3);
        tokens[0].Literal.Should().Be(123);
        tokens[0].Type.Should().Be(TokenType.Number);
        tokens[1].Type.Should().Be(TokenType.Dot);
    }

    [Fact]
    public void Scanner_should_not_scan_dot_splited_number_literal_as_number()
    {
        var literalNumber = "123.4.5";
        var scanner = new Scanner(literalNumber);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(4);
        tokens[0].Literal.Should().Be(123.4);
        tokens[0].Type.Should().Be(TokenType.Number);
        tokens[1].Type.Should().Be(TokenType.Dot);
        tokens[2].Type.Should().Be(TokenType.Number);
        tokens[2].Literal.Should().Be(5);
    }

    [Fact]
    public void Scanner_should_handle_identifiers()
    {
        var program = @"test variable unvariable unfor foreach unforeach";
        var scanner = new Scanner(program);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(7);
        tokens.Take(6).All(x => x.Type == TokenType.Identifier).Should().BeTrue();
        tokens[0].Lexeme.Should().Be("test");
        tokens[1].Lexeme.Should().Be("variable");
        tokens[2].Lexeme.Should().Be("unvariable");
        tokens[3].Lexeme.Should().Be("unfor");
        tokens[4].Lexeme.Should().Be("foreach");
        tokens[5].Lexeme.Should().Be("unforeach");
    }

    [Fact]
    public void Scanner_should_handle_keywords()
    {
        var program = @"and class else false for fun if nil or print return super this true var while";
        var scanner = new Scanner(program);
        var tokens = scanner.ScanTokens().ToList();

        tokens.Should().HaveCount(17);
        tokens[0].Type.Should().Be(TokenType.And);
        tokens[1].Type.Should().Be(TokenType.Class);
        tokens[2].Type.Should().Be(TokenType.Else);
        tokens[3].Type.Should().Be(TokenType.False);
        tokens[4].Type.Should().Be(TokenType.For);
        tokens[5].Type.Should().Be(TokenType.Fun);
        tokens[6].Type.Should().Be(TokenType.If);
        tokens[7].Type.Should().Be(TokenType.Nil);
        tokens[8].Type.Should().Be(TokenType.Or);
        tokens[9].Type.Should().Be(TokenType.Print);
        tokens[10].Type.Should().Be(TokenType.Return);
        tokens[11].Type.Should().Be(TokenType.Super);
        tokens[12].Type.Should().Be(TokenType.This);
        tokens[13].Type.Should().Be(TokenType.True);
        tokens[14].Type.Should().Be(TokenType.Var);
        tokens[15].Type.Should().Be(TokenType.While);
    }
}