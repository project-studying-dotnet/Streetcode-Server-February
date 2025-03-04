using Xunit;
using Moq;
using Streetcode.BLL.MediatR.Email;
using Streetcode.BLL.Interfaces.Email;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Email;
using Streetcode.DAL.Entities.AdditionalContent.Email;

namespace Streetcode.XUnitTest.MediatRTests.Email;

public class SendEmailHandlerTests
{
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly SendEmailHandler _handler;

    public SendEmailHandlerTests() =>
        _handler = new(_emailServiceMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEmailSentSuccessfully()
    {
        _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
            .ReturnsAsync(true);

        var command = new SendEmailCommand(
            new EmailDTO { From = "test@test.com", Content = "Test content" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailServiceMock.Verify(
            s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenEmailSendingFails()
    {
        _emailServiceMock.Setup(s => s.SendEmailAsync(It.IsAny<Message>()))
            .ReturnsAsync(false);

        var command = new SendEmailCommand(
            new EmailDTO { From = "test@test.com", Content = "Test content" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        _emailServiceMock.Verify(
            s => s.SendEmailAsync(It.IsAny<Message>()), Times.Once);
        const string errorMsg = $"Failed to send email message";
        _loggerMock.Verify(
            l => l.LogError(command, errorMsg), Times.Once);
    }
}
