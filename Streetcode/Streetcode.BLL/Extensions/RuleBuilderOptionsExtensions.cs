using FluentValidation;

namespace Streetcode.BLL.Extensions;

public static class RuleBuilderOptionsExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithFormatedMessage<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, string errorMessage, params object?[] constantValues)
    {
        rule.WithMessage(string.Format(errorMessage, constantValues));

        return rule;
    }
}
