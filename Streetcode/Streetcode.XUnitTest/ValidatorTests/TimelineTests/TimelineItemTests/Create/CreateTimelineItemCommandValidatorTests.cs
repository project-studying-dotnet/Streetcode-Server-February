using FluentValidation.TestHelper;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Create;
using Streetcode.BLL.Validators.Timeline.TimelineItem;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests.TimelineTests.TimelineItemTests.Create;

public class CreateTimelineItemCommandValidatorTests
{
    private readonly CreateTimelineItemCommandValidator _validator;

    public CreateTimelineItemCommandValidatorTests()
    {
        var baseValidator = new BaseTimelineItemValidator();
        _validator = new CreateTimelineItemCommandValidator(baseValidator);
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var timelineDto = new TimelineItemCreateDTO
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new CreateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("This title is way too long and exceeds the maximum allowed length of 255 characters. " +
            "This title is way too long and exceeds the maximum allowed length of 255 characters. " +
            "This title is way too long and exceeds the maximum allowed length of 255 characters.")]
    public void Validate_ShouldFail_WhenTitleIsInvalid(string invalidTitle)
    {
        // Arrange
        var timelineDto = new TimelineItemCreateDTO
        {
            Title = invalidTitle,
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new CreateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewTimelineItem.Title);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDateIsMissing()
    {
        // Arrange
        var timelineDto = new TimelineItemCreateDTO
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Date = default, // Дата не встановлена
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new CreateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewTimelineItem.Date);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDateViewPatternIsInvalid()
    {
        // Arrange
        var timelineDto = new TimelineItemCreateDTO
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = (DateViewPattern)999, // Некоректне значення
            StreetcodeId = 2
        };

        var command = new CreateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewTimelineItem.DateViewPattern);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldFail_WhenStreetcodeIdIsInvalid(int invalidStreetcodeId)
    {
        // Arrange
        var timelineDto = new TimelineItemCreateDTO
        {
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = invalidStreetcodeId
        };

        var command = new CreateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewTimelineItem.StreetcodeId);
    }
}
