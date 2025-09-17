using Microsoft.Win32;
using Server.Application.Dtos;
using Server.Domain.Models;

namespace Server.Application.Tests.Mappers;

[TestFixture]
public class ToDtoMapperTests
{
    [Test]
    public void ToDto_Agent_ShouldMapCorrectly()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = new Agent
        {
            Id = agentId,
            Name = "TestAgent",
            ConfigurationId = 123
        };

        // Act
        var result = agent.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("TestAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(123));
    }

    [Test]
    public void ToDto_Configuration_ShouldMapCorrectly()
    {
        // Arrange
        var configuration = new Configuration
        {
            Id = 456,
            Name = "TestConfig",
            Agents = new List<Agent>
            {
                new() { Id = Guid.NewGuid(), Name = "Agent1" },
                new() { Id = Guid.NewGuid(), Name = "Agent2" }
            },
            Processes = new List<ProcessInConfiguration>
            {
                new() { ProcessId = 1, ProcessState = ProcessState.Active },
                new() { ProcessId = 2, ProcessState = ProcessState.Banned }
            },
            Policies = new List<PolicyInConfiguration>
            {
                new() { PolicyId = 1, RegistryValue = "Value1" },
                new() { PolicyId = 2, RegistryValue = "Value2" }
            }
        };

        // Act
        var result = configuration.ToDetailedDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(456));
        Assert.That(result.Name, Is.EqualTo("TestConfig"));
    }

    [Test]
    public void ToDto_Configuration_WithEmptyCollections_ShouldMapCorrectly()
    {
        // Arrange
        var configuration = new Configuration
        {
            Id = 789,
            Name = "EmptyConfig",
            Agents = new List<Agent>(),
            Processes = new List<ProcessInConfiguration>(),
            Policies = new List<PolicyInConfiguration>()
        };

        // Act
        var result = configuration.ToDetailedDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(789));
        Assert.That(result.Name, Is.EqualTo("EmptyConfig"));
    }

    [Test]
    public void ToDto_Process_ShouldMapCorrectly()
    {
        // Arrange
        var process = new Process
        {
            Id = 999,
            Name = "TestProcess"
        };

        // Act
        var result = process.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(999));
        Assert.That(result.Name, Is.EqualTo("TestProcess"));
    }

    [Test]
    public void ToDto_ProcessInConfiguration_ShouldMapCorrectly()
    {
        // Arrange
        var processInConfig = new ProcessInConfiguration
        {
            ProcessId = 123,
            ProcessState = ProcessState.Active
        };

        // Act
        var result = processInConfig.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessId, Is.EqualTo(123));
        Assert.That(result.ProcessState, Is.EqualTo(ProcessState.Active));
    }

    [Test]
    public void ToDto_Policy_ShouldMapCorrectly()
    {
        // Arrange
        var policy = new Policy
        {
            Id = 555,
            Name = "TestPolicy",
            Description = "Test Description",
            RegistryPath = "HKLM\\Software\\Test",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hkcu,
            RegistryKey = "TestKey"
        };

        // Act
        var result = policy.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(555));
        Assert.That(result.Name, Is.EqualTo("TestPolicy"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Software\\Test"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.Dword));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hkcu));
        Assert.That(result.RegistryKey, Is.EqualTo("TestKey"));
    }

    [Test]
    public void ToDto_PolicyInConfiguration_ShouldMapCorrectly()
    {
        // Arrange
        var policyInConfig = new PolicyInConfiguration
        {
            PolicyId = 777,
            RegistryValue = "TestRegistryValue"
        };

        // Act
        var result = policyInConfig.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PolicyId, Is.EqualTo(777));
        Assert.That(result.RegistryValue, Is.EqualTo("TestRegistryValue"));
    }

    [Test]
    public void ToDto_Agent_WithEmptyName_ShouldMapCorrectly()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agent = new Agent
        {
            Id = agentId,
            Name = "",
            ConfigurationId = 0
        };

        // Act
        var result = agent.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo(""));
        Assert.That(result.ConfigurationId, Is.EqualTo(0));
    }

    [Test]
    public void ToDto_Process_WithEmptyName_ShouldMapCorrectly()
    {
        // Arrange
        var process = new Process
        {
            Id = 0,
            Name = ""
        };

        // Act
        var result = process.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(0));
        Assert.That(result.Name, Is.EqualTo(""));
    }

    [Test]
    public void ToDto_Policy_WithEmptyStrings_ShouldMapCorrectly()
    {
        // Arrange
        var policy = new Policy
        {
            Id = 0,
            Name = "",
            Description = "",
            RegistryPath = "",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = ""
        };

        // Act
        var result = policy.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(0));
        Assert.That(result.Name, Is.EqualTo(""));
        Assert.That(result.Description, Is.EqualTo(""));
        Assert.That(result.RegistryPath, Is.EqualTo(""));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.String));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo(""));
    }

    [Test]
    public void ToDto_ProcessInConfiguration_WithEmptyState_ShouldMapCorrectly()
    {
        // Arrange
        var processInConfig = new ProcessInConfiguration
        {
            ProcessId = 0,
            ProcessState = ProcessState.Active
        };

        // Act
        var result = processInConfig.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.ProcessId, Is.EqualTo(0));
        Assert.That(result.ProcessState, Is.EqualTo(ProcessState.Active));
    }

    [Test]
    public void ToDto_PolicyInConfiguration_WithEmptyValue_ShouldMapCorrectly()
    {
        // Arrange
        var policyInConfig = new PolicyInConfiguration
        {
            PolicyId = 0,
            RegistryValue = ""
        };

        // Act
        var result = policyInConfig.ToDto();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.PolicyId, Is.EqualTo(0));
        Assert.That(result.RegistryValue, Is.EqualTo(""));
    }
}
