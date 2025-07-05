using System;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core.Parser;
using System.Linq.Dynamic.Core;

namespace IntegrationApp.Services
{
    public static class FunctionParser
    {
        public static Func<double, double> Parse(string functionStr)
        {
            try
            {
                if (!functionStr.Contains("=>"))
                    functionStr = "x => " + functionStr;

                var paramX = Expression.Parameter(typeof(double), "x");

                var body = DynamicExpressionParser.ParseLambda(new[] { paramX }, null, functionStr.Split("=>")[1].Trim()).Body;

                var lambda = Expression.Lambda<Func<double, double>>(body, paramX);
                return lambda.Compile();
            }
            catch (Exception ex)
            {
                throw new FormatException($"Ошибка разбора функции: {ex.Message}");
            }
        }
    }
}