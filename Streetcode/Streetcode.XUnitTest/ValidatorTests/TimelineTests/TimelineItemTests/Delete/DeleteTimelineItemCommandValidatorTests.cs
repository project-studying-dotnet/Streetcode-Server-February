using FluentValidation.TestHelper;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Delete;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests.TimelineTests.TimelineItemTests.Delete;

public class DeleteTimelineItemCommandValidatorTests
{
    private readonly DeleteTimelineItemCommandValidator _validator;

    public DeleteTimelineItemCommandValidatorTests()
    {
        _validator = new DeleteTimelineItemCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenIdIsValid()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(1); // Коректний Id

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldFail_WhenIdIsZeroOrNegative(int invalidId)
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(invalidId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_ShouldFail_WhenIdIsDefault()
    {
        // Arrange
        var command = new DeleteTimelineItemCommand(default); // Значення за замовчуванням = 0

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
