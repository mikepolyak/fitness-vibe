using AutoMapper;
using FitnessVibe.Application.Commands.Challenges;
using FitnessVibe.Application.DTOs.Challenges;
using FitnessVibe.Application.Handlers.Challenges;
using FitnessVibe.Domain.Entities.Challenges;
using FitnessVibe.Domain.Repositories;
using Moq;
using Xunit;

namespace FitnessVibe.Application.Tests.Handlers.Challenges;

public class CreateChallengeCommandHandlerTests
{
    private readonly Mock<IChallengeRepository> _challengeRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateChallengeCommandHandler _handler;

    public CreateChallengeCommandHandlerTests()
    {
        _challengeRepositoryMock = new Mock<IChallengeRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateChallengeCommandHandler(_challengeRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesChallenge()
    {
        // Arrange
        var command = new CreateChallengeCommand
        {
            Name = "Test Challenge",
            Description = "Test Description",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(30),
            GoalValue = 100,
            ChallengeType = Domain.Enums.ChallengeType.Distance
        };

        var challenge = new Challenge
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            GoalValue = command.GoalValue,
            ChallengeType = command.ChallengeType
        };

        var expectedResponse = new ChallengeResponse
        {
            Id = challenge.Id,
            Name = challenge.Name,
            Description = challenge.Description,
            StartDate = challenge.StartDate,
            EndDate = challenge.EndDate,
            GoalValue = challenge.GoalValue,
            ChallengeType = challenge.ChallengeType
        };

        _challengeRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Challenge>()))
            .ReturnsAsync(challenge);

        _mapperMock.Setup(x => x.Map<ChallengeResponse>(It.IsAny<Challenge>()))
            .Returns(expectedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(challenge.Id, result.Id);
        Assert.Equal(command.Name, result.Name);
        Assert.Equal(command.Description, result.Description);
        Assert.Equal(command.StartDate, result.StartDate);
        Assert.Equal(command.EndDate, result.EndDate);
        Assert.Equal(command.GoalValue, result.GoalValue);
        Assert.Equal(command.ChallengeType, result.ChallengeType);

        _challengeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Challenge>()), Times.Once);
        _mapperMock.Verify(x => x.Map<ChallengeResponse>(It.IsAny<Challenge>()), Times.Once);
    }
}
