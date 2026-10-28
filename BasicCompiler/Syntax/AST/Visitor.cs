namespace BasicCompiler.Syntax.AST;

public interface IExpressionVisitor<out TResult>
{
    TResult Visit(BinaryExpression expression);
    TResult Visit(UnaryExpression expression);
    TResult Visit(LiteralExpression expression);
    TResult Visit(VariableExpression expression);
}

public interface IStatementVisitor<out TResult>
{
    TResult Visit(VariableDeclaration statement);
    TResult Visit(AssignmentStatement statement);
    TResult Visit(IfStatement statement);
    TResult Visit(WhileStatement statement);
    TResult Visit(PrintStatement statement);
    TResult Visit(BlockStatement statement);
}
