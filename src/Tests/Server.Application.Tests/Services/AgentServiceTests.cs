using Common.Messages.Agent.Sync;
using Server.Application.Abstractions.Providers;
using Server.Application.Abstractions.Repositories;
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
    private Mock<IPasswordHasher> _passwordHasherMock = null!;
    private ILongPollingDispatcher<Guid, ServerSyncMessage> _mockLongPollingDispatcher = null!;
    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAgentRepository = new Mock<IAgentRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _passwordHasherMock
            .Setup(x => x.CreatePasswordHash(It.IsAny<string>()))
            .Returns((new byte[] { 1 }, new byte[] { 2 }));
        _mockUnitOfWork.Setup(x => x.Agents).Returns(_mockAgentRepository.Object);
        _mockLongPollingDispatcher = new Mock<ILongPollingDispatcher<Guid, ServerSyncMessage>>().Object;
        _agentService = new AgentService(_mockLongPollingDispatcher, _passwordHasherMock.Object, _mockUnitOfWork.Object);
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
        var agent = new Agent
        {
            Id = agentId, Name = "TestAgent",
            ConfigurationId = 1,
            Configuration = new Configuration { Id = 1, Name = "Default" },
            Hardware = new AgentHardware(),
        };

        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(agent);

        // Act
        var result = await _agentService.GetAgentAsync(agentId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("TestAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(1));
        Assert.That(result.Configuration.Name, Is.EqualTo("Default"));
        Assert.That(result.Hardware, Is.Not.Null);

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

        _mockAgentRepository.SetupSequence(x => x.GetAsync(agentId))
            .ReturnsAsync(existingAgent)
            .ReturnsAsync(updatedAgent);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _agentService.UpdateAgentAsync(agentId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("NewName"));
        Assert.That(result.ConfigurationId, Is.EqualTo(2));

        _mockAgentRepository.Verify(x => x.GetAsync(agentId), Times.Exactly(2));
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

        _mockAgentRepository.SetupSequence(x => x.GetAsync(agentId))
            .ReturnsAsync(existingAgent)
            .ReturnsAsync(updatedAgent);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _agentService.UpdateAgentAsync(agentId, request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAgentAsync_ShouldReturnResponseWithIdAndSecret_AndMarkNotSynchronized()
    {
        // Arrange
        var request = new AgentCreateRequest { Name = "AgentX", ConfigurationId = 42 };
        Agent? createdAgent = null;

        _mockAgentRepository
            .Setup(x => x.CreateAsync(It.IsAny<Agent>()))
            .Callback<Agent>(a => createdAgent = a)
            .Returns(Task.CompletedTask);

        _mockAgentRepository
            .Setup(x => x.GetAsync(It.IsAny<Guid>()))
            .ReturnsAsync(() => createdAgent);

        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var response = await _agentService.CreateAgentAsync(request);

        // Assert
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Id, Is.EqualTo(createdAgent!.Id));
        Assert.That(response.Secret, Is.Not.Null.And.Not.Empty);
        Assert.That(createdAgent.Status, Is.EqualTo(AgentStatus.NotSynchronized));
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task SyncAgentAsync_WhenAgentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync((Agent?)null);

        var message = new Common.Messages.Agent.Sync.AgentSyncMessage
        {
            Hardware = new Common.Messages.Agent.Sync.AgentHardwareMessage
            {
                Cpu = new Common.Messages.Agent.Sync.Hardware.CpuInfoMessage(),
                Gpu = new Common.Messages.Agent.Sync.Hardware.GpuInfoMessage(),
                Ram = new Common.Messages.Agent.Sync.Hardware.RamInfoMessage(),
                Disk = new Common.Messages.Agent.Sync.Hardware.DiskInfoMessage()
            }
        };

        // Act
        var result = await _agentService.SyncAgentAsync(agentId, message);

        // Assert
        Assert.That(result, Is.Null);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task SyncAgentAsync_WhenAgentExists_ShouldUpdateHardwareAndStatusAndSave()
    {
      // Arrange
      var agentId = Guid.NewGuid();
      var existing = new Agent
      {
          Id = agentId,
          Name = "A",
          ConfigurationId = 1,
          Hardware = new AgentHardware(),
          Configuration = new Configuration { Id = 1, Name = "C" }
      };

      _mockAgentRepository.Setup(x => x.GetAsync(agentId)).ReturnsAsync(existing);
      _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

      var message = new Common.Messages.Agent.Sync.AgentSyncMessage
      {
          Hardware = new Common.Messages.Agent.Sync.AgentHardwareMessage
          {
              Cpu = new Common.Messages.Agent.Sync.Hardware.CpuInfoMessage { CpuModel = "m", CpuCores = 4 },
              Gpu = new Common.Messages.Agent.Sync.Hardware.GpuInfoMessage { GpuModel = "g", GpuMemoryMb = 2048 },
              Ram = new Common.Messages.Agent.Sync.Hardware.RamInfoMessage { RamModel = "r", TotalMemoryMb = 8192 },
              Disk = new Common.Messages.Agent.Sync.Hardware.DiskInfoMessage { DiskModel = "d", TotalDiskMb = 256000 }
          }
      };

      // Act
      var result = await _agentService.SyncAgentAsync(agentId, message);

      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(existing.Status, Is.EqualTo(AgentStatus.Ok));
      _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}
