using Server.Application.Abstractions;
using Server.Application.Dtos.Agent;
using Server.Application.Services;
using Server.Domain.Models;

namespace Server.Application.Tests.Services;

[TestFixture]
public class AgentServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IAgentRepository> _mockAgentRepository = null!;
    private AgentService _agentService = null!;
    private IPasswordHasher _passwordHasher = null!;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAgentRepository = new Mock<IAgentRepository>();
        _mockUnitOfWork.Setup(x => x.Agents).Returns(_mockAgentRepository.Object);
        _agentService = new AgentService(_passwordHasher, _mockUnitOfWork.Object);
    }

    [Test]
    public async Task GetAgentsAsync_ShouldReturnAllAgents()
    {
        // Arrange
        var agents = new List<Agent>
        {
            new() { Id = Guid.NewGuid(), Name = "Agent1", ConfigurationId = 1 },
            new() { Id = Guid.NewGuid(), Name = "Agent2", ConfigurationId = 2 }
        };

        _mockAgentRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(agents);

        // Act
        var result = (await _agentService.GetAgentsAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Agent1"));
        Assert.That(result.Last().Name, Is.EqualTo("Agent2"));

        _mockAgentRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAgentsAsync_WhenNoAgentsExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockAgentRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Agent>());

        // Act
        var result = await _agentService.GetAgentsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        _mockAgentRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAgentAsync_WhenAgentExists_ShouldReturnAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = new Agent { Id = agentId, Name = "TestAgent", ConfigurationId = 1 };

        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(agent);

        // Act
        var result = await _agentService.GetAgentAsync(agentId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("TestAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(1));

        _mockAgentRepository.Verify(x => x.GetAsync(agentId), Times.Once);
    }

    [Test]
    public async Task GetAgentAsync_WhenAgentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync((Agent?)null);

        // Act
        var result = await _agentService.GetAgentAsync(agentId);

        // Assert
        Assert.That(result, Is.Null);

        _mockAgentRepository.Verify(x => x.GetAsync(agentId), Times.Once);
    }

    [Test]
    public async Task CreateAgentAsync_ShouldCreateAndReturnAgent()
    {
        // Arrange
        var request = new AgentCreateRequest { Name = "NewAgent", ConfigurationId = 1 };
        Agent? createdAgent = null;

        _mockAgentRepository
                .Setup(x => x.CreateAsync(It.IsAny<Agent>()))
                .Callback<Agent>(agent => createdAgent = agent)
                .Returns(Task.CompletedTask);

        _mockAgentRepository
                .Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => createdAgent);

        _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

        // Act
        var result = await _agentService.CreateAgentAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("NewAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(1));

        _mockAgentRepository.Verify(x => x.CreateAsync(It.IsAny<Agent>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAgentAsync_WhenAgentExists_ShouldUpdateAndReturnAgent()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var existingAgent = new Agent { Id = agentId, Name = "OldName", ConfigurationId = 1 };
        var request = new AgentModifyRequest { Name = "NewName", ConfigurationId = 2 };
        var updatedAgent = new Agent { Id = agentId, Name = "NewName", ConfigurationId = 2 };

        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(existingAgent);
        _mockAgentRepository.Setup(x => x.ModifyAsync(It.IsAny<Agent>())).Returns(Task.CompletedTask);
        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(updatedAgent);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _agentService.UpdateAgentAsync(agentId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("NewName"));
        Assert.That(result.ConfigurationId, Is.EqualTo(2));

        _mockAgentRepository.Verify(x => x.GetAsync(agentId), Times.Exactly(2));
        _mockAgentRepository.Verify(x => x.ModifyAsync(It.IsAny<Agent>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAgentAsync_WhenAgentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var request = new AgentModifyRequest { Name = "NewName", ConfigurationId = 2 };

        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync((Agent?)null);

        // Act
        var result = await _agentService.UpdateAgentAsync(agentId, request);

        // Assert
        Assert.That(result, Is.Null);

        _mockAgentRepository.Verify(x => x.GetAsync(agentId), Times.Once);
        _mockAgentRepository.Verify(x => x.ModifyAsync(It.IsAny<Agent>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreateAgentAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var request = new AgentCreateRequest { Name = "TestAgent", ConfigurationId = 1 };
        var createdAgent = new Agent { Id = Guid.NewGuid(), Name = "TestAgent", ConfigurationId = 1 };

        _mockAgentRepository.Setup(x => x.CreateAsync(It.IsAny<Agent>())).Returns(Task.CompletedTask);
        _mockAgentRepository.Setup(x => x.GetAsync(createdAgent.Id)).ReturnsAsync(createdAgent);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _agentService.CreateAgentAsync(request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAgentAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var existingAgent = new Agent { Id = agentId, Name = "OldName", ConfigurationId = 1 };
        var request = new AgentModifyRequest { Name = "NewName", ConfigurationId = 2 };
        var updatedAgent = new Agent { Id = agentId, Name = "NewName", ConfigurationId = 2 };

        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(existingAgent);
        _mockAgentRepository.Setup(x => x.ModifyAsync(It.IsAny<Agent>())).Returns(Task.CompletedTask);
        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(updatedAgent);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _agentService.UpdateAgentAsync(agentId, request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
