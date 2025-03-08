using System.Linq.Expressions;
using Moq;
using Xunit;
using AutoMapper;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.MediatR.Streetcode.Fact.Reorder;
using Streetcode.DAL.Entities.Streetcode.TextContent;
using Streetcode.DAL.Repositories.Interfaces.Base;
using Streetcode.BLL.DTO.Streetcode.TextContent.Fact;

namespace Streetcode.XUnitTest.MediatRTests.Facts.Reorder;

public class ReorderFactsHandlerTests
{
    private readonly Mock<IRepositoryWrapper> _mockRepo;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ReorderFactsHandler _handler;

    public ReorderFactsHandlerTests()
    {
        _mockRepo = new Mock<IRepositoryWrapper>();
        _mockLogger = new Mock<ILoggerService>();
        _mockMapper = new Mock<IMapper>();
        _handler = new ReorderFactsHandler(
            _mockRepo.Object,
            _mockLogger.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_WhenFactsExist_ReturnsSuccess()
    {
        // Arrange
        var facts = GetTestFacts();
        SetupMocks();

        // Act
        var result = await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_WhenFactsExist_UpdatesCorrectNumberOfFacts()
    {
        // Arrange
        var facts = GetTestFacts();
        var updatedFacts = new List<Fact>();
        SetupMocks(updatedFacts);

        // Act
        await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.Equal(3, updatedFacts.Count);
    }

    [Fact]
    public async Task Handle_WhenFactsExist_UpdatesCorrectIndexes()
    {
        // Arrange
        var facts = GetTestFacts();
        var updatedFacts = new List<Fact>();
        SetupMocks(updatedFacts);

        // Act
        await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.Equal(2, updatedFacts.First(f => f.Id == 1).Index);
        Assert.Equal(0, updatedFacts.First(f => f.Id == 2).Index);
        Assert.Equal(1, updatedFacts.First(f => f.Id == 3).Index);
    }

    [Fact]
    public async Task Handle_WhenFactsExist_CallsUpdateCorrectNumberOfTimes()
    {
        // Arrange
        var facts = GetTestFacts();
        SetupMocks();

        // Act
        await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        _mockRepo.Verify(
            r => r.FactRepository.Update(It.IsAny<Fact>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task Handle_WhenFactsExist_CallsMapperCorrectNumberOfTimes()
    {
        // Arrange
        var facts = GetTestFacts();
        SetupMocks();

        // Act
        await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        _mockMapper.Verify(
            m => m.Map<Fact>(It.IsAny<ReorderFactDTO>()),
            Times.Exactly(3));
    }

    [Fact]
    public async Task Handle_WhenInputIsEmpty_ReturnsFailure()
    {
        // Arrange
        var facts = new List<ReorderFactDTO>();

        // Act
        var result = await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<ReorderFactsCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenIndexIsNegative_ReturnsFailure()
    {
        // Arrange
        var facts = new List<ReorderFactDTO>
        {
            new() { Id = 1, Index = -1 }
        };

        // Act
        var result = await _handler.Handle(
            new ReorderFactsCommand(facts),
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        _mockLogger.Verify(
            l => l.LogError(
                It.IsAny<ReorderFactsCommand>(),
                It.IsAny<string>()),
            Times.Once);
    }

    private List<ReorderFactDTO> GetTestFacts()
    {
        return new List<ReorderFactDTO>
        {
            new() { Id = 1, Index = 2 },
            new() { Id = 2, Index = 0 },
            new() { Id = 3, Index = 1 }
        };
    }

    private void SetupMocks(List<Fact>? updatedFacts = null)
    {
        var fact1 = new Fact { Id = 1, Index = 0 };
        var fact2 = new Fact { Id = 2, Index = 1 };
        var fact3 = new Fact { Id = 3, Index = 2 };

        _mockMapper.Setup(m => m.Map<Fact>(It.IsAny<ReorderFactDTO>()))
            .Returns<ReorderFactDTO>(dto => new Fact { Id = dto.Id, Index = dto.Index });

        _mockRepo.Setup(r => r.FactRepository
            .GetFirstOrDefaultAsync(
                It.Is<Expression<Func<Fact, bool>>>(
                    expr => expr.Compile().Invoke(fact1) ||
                           expr.Compile().Invoke(fact2) ||
                           expr.Compile().Invoke(fact3)),
                null))
            .ReturnsAsync((Expression<Func<Fact, bool>> expr, string? _) =>
            {
                if (expr.Compile().Invoke(fact1))
                {
                    return fact1;
                }

                if (expr.Compile().Invoke(fact2))
                {
                    return fact2;
                }

                if (expr.Compile().Invoke(fact3))
                {
                    return fact3;
                }

                return null;
            });

        if (updatedFacts != null)
        {
            _mockRepo.Setup(r => r.FactRepository.Update(It.IsAny<Fact>()))
                .Callback<Fact>(f => updatedFacts.Add(f));
        }

        _mockRepo.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
    }
}
