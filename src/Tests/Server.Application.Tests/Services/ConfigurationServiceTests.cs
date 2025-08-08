using Common.Messages;
using Common.Messages.Configuration;
using Server.Application.Abstractions;
using Server.Application.Dtos.Configuration;
using Server.Application.Services;
using Server.Domain.Models;

namespace Server.Application.Tests.Services;

[TestFixture]
public class ConfigurationServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IConfigurationRepository> _mockConfigurationRepository = null!;
    private ConfigurationService _configurationService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockConfigurationRepository = new Mock<IConfigurationRepository>();
        _mockUnitOfWork.Setup(x => x.Configurations).Returns(_mockConfigurationRepository.Object);
        _configurationService = new ConfigurationService( _mockUnitOfWork.Object);
    }

    [Test]
    public async Task GetConfigurationsAsync_ShouldReturnAllConfigurations()
    {
        // Arrange
        var configurations = new List<Configuration>
        {
            new() { Id = 1, Name = "Config1", Agents = new List<Agent>(), Processes = new List<ProcessInConfiguration>(), Policies = new List<PolicyInConfiguration>() },
            new() { Id = 2, Name = "Config2", Agents = new List<Agent>(), Processes = new List<ProcessInConfiguration>(), Policies = new List<PolicyInConfiguration>() }
        };

        _mockConfigurationRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(configurations);

        // Act
        var result = (await _configurationService.GetConfigurationsAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ToList(), Has.Count.EqualTo(2));
        Assert.That(result.ToList().First().Name, Is.EqualTo("Config1"));
        Assert.That(result.ToList().Last().Name, Is.EqualTo("Config2"));

        _mockConfigurationRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetConfigurationsAsync_WhenNoConfigurationsExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockConfigurationRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Configuration>());

        // Act
        var result = await _configurationService.GetConfigurationsAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);

        _mockConfigurationRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetConfigurationAsync_WhenConfigurationExists_ShouldReturnConfiguration()
    {
        // Arrange
        var configurationId = 1L;
        var configuration = new Configuration
        {
            Id = configurationId,
            Name = "TestConfig",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };

        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(configuration);

        // Act
        var result = await _configurationService.GetConfigurationAsync(configurationId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(configurationId));
        Assert.That(result.Name, Is.EqualTo("TestConfig"));

        _mockConfigurationRepository.Verify(x => x.GetAsync(configurationId), Times.Once);
    }

    [Test]
    public async Task GetConfigurationAsync_WhenConfigurationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var configurationId = 1L;
        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync((Configuration?)null);

        // Act
        var result = await _configurationService.GetConfigurationAsync(configurationId);

        // Assert
        Assert.That(result, Is.Null);

        _mockConfigurationRepository.Verify(x => x.GetAsync(configurationId), Times.Once);
    }

    [Test]
    public async Task CreateConfigurationAsync_ShouldCreateAndReturnConfiguration()
    {
        // Arrange
        var request = new ConfigurationCreateRequest { Name = "NewConfig" };
        Configuration? createdConfiguration = null;

        _mockConfigurationRepository
                .Setup(x => x.CreateAsync(It.IsAny<Configuration>()))
                .Callback<Configuration>(conf => createdConfiguration = conf)
                .Returns(Task.CompletedTask);

        _mockConfigurationRepository
                .Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(() => createdConfiguration);

        _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

        // Act
        var result = await _configurationService.CreateConfigurationAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("NewConfig"));
        Assert.That(result.Id, Is.EqualTo(createdConfiguration!.Id));

        _mockConfigurationRepository.Verify(x => x.CreateAsync(It.IsAny<Configuration>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateConfigurationAsync_WhenConfigurationExists_ShouldUpdateAndReturnConfiguration()
    {
        // Arrange
        var configurationId = 1L;
        var existingConfiguration = new Configuration
        {
            Id = configurationId,
            Name = "OldName",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };
        var request = new ConfigurationModifyRequest { Name = "NewName" };
        var updatedConfiguration = new Configuration
        {
            Id = configurationId,
            Name = "NewName",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };

        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(existingConfiguration);
        _mockConfigurationRepository.Setup(x => x.ModifyAsync(It.IsAny<Configuration>())).Returns(Task.CompletedTask);
        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(updatedConfiguration);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _configurationService.UpdateConfigurationAsync(configurationId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(configurationId));
        Assert.That(result.Name, Is.EqualTo("NewName"));

        _mockConfigurationRepository.Verify(x => x.GetAsync(configurationId), Times.Exactly(2));
        _mockConfigurationRepository.Verify(x => x.ModifyAsync(It.IsAny<Configuration>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateConfigurationAsync_WhenConfigurationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var configurationId = 1L;
        var request = new ConfigurationModifyRequest { Name = "NewName" };

        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync((Configuration?)null);

        // Act
        var result = await _configurationService.UpdateConfigurationAsync(configurationId, request);

        // Assert
        Assert.That(result, Is.Null);

        _mockConfigurationRepository.Verify(x => x.GetAsync(configurationId), Times.Once);
        _mockConfigurationRepository.Verify(x => x.ModifyAsync(It.IsAny<Configuration>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreateConfigurationAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var request = new ConfigurationCreateRequest { Name = "TestConfig" };
        var createdConfiguration = new Configuration
        {
            Id = 1,
            Name = "TestConfig",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };

        _mockConfigurationRepository.Setup(x => x.CreateAsync(It.IsAny<Configuration>())).Returns(Task.CompletedTask);
        _mockConfigurationRepository.Setup(x => x.GetAsync(createdConfiguration.Id)).ReturnsAsync(createdConfiguration);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _configurationService.CreateConfigurationAsync(request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateConfigurationAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var configurationId = 1L;
        var existingConfiguration = new Configuration
        {
            Id = configurationId,
            Name = "OldName",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };
        var request = new ConfigurationModifyRequest { Name = "NewName" };
        var updatedConfiguration = new Configuration
        {
            Id = configurationId,
            Name = "NewName",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };

        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(existingConfiguration);
        _mockConfigurationRepository.Setup(x => x.ModifyAsync(It.IsAny<Configuration>())).Returns(Task.CompletedTask);
        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(updatedConfiguration);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _configurationService.UpdateConfigurationAsync(configurationId, request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetConfigurationAsync_WithAgentsAndProcessesAndPolicies_ShouldReturnCompleteConfiguration()
    {
        // Arrange
        var configurationId = 1L;
        var agents = new List<Agent> { new() { Id = Guid.NewGuid(), Name = "Agent1" } };
        var processes = new List<ProcessInConfiguration> { new() { ProcessId = 1, ProcessState = ProcessState.Active } };
        var policies = new List<PolicyInConfiguration> { new() { PolicyId = 1, RegistryValue = "TestValue" } };

        var configuration = new Configuration
        {
            Id = configurationId,
            Name = "TestConfig",
            Agents = agents,
            Processes = processes,
            Policies = policies
        };

        _mockConfigurationRepository.Setup(x => x.GetAsync(configurationId)).ReturnsAsync(configuration);

        // Act
        var result = await _configurationService.GetConfigurationAsync(configurationId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(configurationId));
        Assert.That(result.Name, Is.EqualTo("TestConfig"));
        Assert.That(result.AgentIds.ToList(), Has.Count.EqualTo(1));
        Assert.That(result.Processes.ToList(), Has.Count.EqualTo(1));
        Assert.That(result.Policies.ToList(), Has.Count.EqualTo(1));

        _mockConfigurationRepository.Verify(x => x.GetAsync(configurationId), Times.Once);
    }
}
