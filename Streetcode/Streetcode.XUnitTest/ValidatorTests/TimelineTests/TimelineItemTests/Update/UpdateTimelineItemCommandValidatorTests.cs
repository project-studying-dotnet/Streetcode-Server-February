using FluentValidation.TestHelper;
using Moq;
using Streetcode.BLL.DTO.Timeline.TimelineItem;
using Streetcode.BLL.MediatR.Timeline.TimelineItem.Update;
using Streetcode.BLL.Validators.Timeline.TimelineItem;
using Streetcode.DAL.Enums;
using Xunit;

namespace Streetcode.XUnitTest.ValidatorTests.TimelineTests.TimelineItemTests.Update;
public class UpdateTimelineItemCommandValidatorTests
{
    private readonly UpdateTimelineItemCommandValidator _validator;

    public UpdateTimelineItemCommandValidatorTests()
    {
        var baseValidator = new BaseTimelineItemValidator();
        _validator = new UpdateTimelineItemCommandValidator(baseValidator);
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldFail_WhenIdIsInvalid(int invalidId)
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = invalidId,
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimelineItemDTO.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("This title is way too long and exceeds the maximum allowed length of 255 characters. " +
            "This title is way too long and exceeds the maximum allowed length of 255 characters. " +
            "This title is way too long and exceeds the maximum allowed length of 255 characters.")]
    public void Validate_ShouldFail_WhenTitleIsInvalid(string invalidTitle)
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = invalidTitle,
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimelineItemDTO.Title);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDateIsMissing()
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = "Valid Title",
            Description = "Valid Description",
            Date = default, // Дата не встановлена
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = 2
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimelineItemDTO.Date);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDateViewPatternIsInvalid()
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = (DateViewPattern)999, // Некоректне значення
            StreetcodeId = 2
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimelineItemDTO.DateViewPattern);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldFail_WhenStreetcodeIdIsInvalid(int invalidStreetcodeId)
    {
        // Arrange
        var timelineDto = new TimelineItemUpdateDTO
        {
            Id = 1,
            Title = "Valid Title",
            Description = "Valid Description",
            Date = DateTime.UtcNow,
            DateViewPattern = DateViewPattern.Year,
            StreetcodeId = invalidStreetcodeId
        };

        var command = new UpdateTimelineItemCommand(timelineDto);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TimelineItemDTO.StreetcodeId);
    }
}
