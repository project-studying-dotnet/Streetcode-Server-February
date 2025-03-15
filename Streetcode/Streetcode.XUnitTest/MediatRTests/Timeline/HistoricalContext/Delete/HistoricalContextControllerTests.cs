using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Streetcode.BLL.MediatR.Timeline.HistoricalContext.Delete;
using Xunit;

namespace Streetcode.XUnitTest.MediatRTests.Timeline.HistoricalContext.Delete;

public class HistoricalContextControllerTests
{
    [Fact]
    public async Task DeleteHistoricalContext_ShouldCallMediator_WithCorrectCommand()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(
                m => m.Send(
                    It.IsAny<DeleteHistoryEventCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));
        var controller = new MockHistoricalContextController(mediatorMock.Object);
        int historicalContextId = 1;

        // Act
        await controller.DeleteHistoricalContext(historicalContextId);

        // Assert
        mediatorMock.Verify(
            m => m.Send(
                It.Is<DeleteHistoryEventCommand>(cmd => cmd.Id == historicalContextId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteHistoricalContext_ShouldReturnNoContent_WhenSuccessful()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(
                m => m.Send(
                    It.IsAny<DeleteHistoryEventCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(Unit.Value));
        var controller = new MockHistoricalContextController(mediatorMock.Object);
        int historicalContextId = 1;

        // Act
        var result = await controller.DeleteHistoricalContext(historicalContextId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteHistoricalContext_ShouldReturnNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(
                m => m.Send(
                    It.IsAny<DeleteHistoryEventCommand>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail(new Error("Cannot find historical event")));
        var controller = new MockHistoricalContextController(mediatorMock.Object);
        int historicalContextId = 1;

        // Act
        var result = await controller.DeleteHistoricalContext(historicalContextId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    // Mock controller class for testing
    private class MockHistoricalContextController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MockHistoricalContextController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> DeleteHistoricalContext(int id)
        {
            var result = await _mediator.Send(new DeleteHistoryEventCommand(id));
            if (result.IsFailed)
            {
                return NotFound(result.Errors);
            }

            return NoContent();
        }
    }
}