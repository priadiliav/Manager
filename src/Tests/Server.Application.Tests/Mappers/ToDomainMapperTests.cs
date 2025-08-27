using Server.Application.Dtos;
using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;
using Server.Domain.Models;

namespace Server.Application.Tests.Mappers;

[TestFixture]
public class ToDomainMapperTests
{
    [Test]
    public void ToDomain_AgentDto_ShouldMapCorrectly()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var agentDto = new AgentDto
        {
            Id = agentId,
            Name = "TestAgent",
            ConfigurationId = 123
        };

        // Act
        var result = agentDto.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("TestAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(123));
    }

    [Test]
    public void ToDomain_AgentCreateRequest_ShouldMapCorrectly()
    {
        // Arrange
        var request = new AgentCreateRequest
        {
            Name = "NewAgent",
            ConfigurationId = 456
        };

        // Act
        var result = request.ToDomain(new byte[] { }, new byte[] { });

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("NewAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(456));
        Assert.That(result.Id, Is.EqualTo(Guid.Empty)); // Should be default for new entity
    }

    [Test]
    public void ToDomain_AgentModifyRequest_ShouldMapCorrectly()
    {
        // Arrange
        var agentId = Guid.NewGuid();
        var request = new AgentModifyRequest
        {
            Name = "UpdatedAgent",
            ConfigurationId = 789
        };

        // Act
        var result = request.ToDomain(agentId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(agentId));
        Assert.That(result.Name, Is.EqualTo("UpdatedAgent"));
        Assert.That(result.ConfigurationId, Is.EqualTo(789));
    }

    [Test]
    public void ToDomain_ConfigurationDto_ShouldMapCorrectly()
    {
        // Arrange
        var configDto = new ConfigurationDto
        {
            Id = 123,
            Name = "TestConfig",
            AgentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            Processes = new List<ProcessInConfigurationDto>
            {
                new() { ProcessId = 1, ProcessState = ProcessState.Active },
                new() { ProcessId = 2, ProcessState = ProcessState.Banned }
            },
            Policies = new List<PolicyInConfigurationDto>
            {
                new() { PolicyId = 1, RegistryValue = "Value1" },
                new() { PolicyId = 2, RegistryValue = "Value2" }
            }
        };

        // Act
        var result = configDto.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(123));
        Assert.That(result.Name, Is.EqualTo("TestConfig"));
        Assert.That(result.Agents, Has.Count.EqualTo(2));
        Assert.That(result.Processes, Has.Count.EqualTo(2));
        Assert.That(result.Policies, Has.Count.EqualTo(2));
    }

    [Test]
    public void ToDomain_ConfigurationCreateRequest_ShouldMapCorrectly()
    {
        // Arrange
        var request = new ConfigurationCreateRequest
        {
            Name = "NewConfig",
            Processes = new List<ProcessInConfigurationDto>
            {
                new() { ProcessId = 1, ProcessState = ProcessState.Active }
            },
            Policies = new List<PolicyInConfigurationDto>
            {
                new() { PolicyId = 1, RegistryValue = "Value1" }
            }
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("NewConfig"));
        Assert.That(result.Processes, Has.Count.EqualTo(1));
        Assert.That(result.Policies, Has.Count.EqualTo(1));
        Assert.That(result.Id, Is.EqualTo(0)); // Should be default for new entity
    }

    [Test]
    public void ToDomain_ConfigurationModifyRequest_ShouldMapCorrectly()
    {
        // Arrange
        var configId = 999L;
        var request = new ConfigurationModifyRequest
        {
            Name = "UpdatedConfig",
            Processes = new List<ProcessInConfigurationDto>
            {
                new() { ProcessId = 1, ProcessState = ProcessState.Active }
            },
            Policies = new List<PolicyInConfigurationDto>
            {
                new() { PolicyId = 1, RegistryValue = "Value1" }
            }
        };

        // Act
        var result = request.ToDomain(configId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(configId));
        Assert.That(result.Name, Is.EqualTo("UpdatedConfig"));
        Assert.That(result.Processes, Has.Count.EqualTo(1));
        Assert.That(result.Policies, Has.Count.EqualTo(1));
    }

    [Test]
    public void ToDomain_ProcessDto_ShouldMapCorrectly()
    {
        // Arrange
        var processDto = new ProcessDto
        {
            Id = 555,
            Name = "TestProcess"
        };

        // Act
        var result = processDto.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(555));
        Assert.That(result.Name, Is.EqualTo("TestProcess"));
    }

    [Test]
    public void ToDomain_ProcessCreateRequest_ShouldMapCorrectly()
    {
        // Arrange
        var request = new ProcessCreateRequest
        {
            Name = "NewProcess"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("NewProcess"));
        Assert.That(result.Id, Is.EqualTo(0)); // Should be default for new entity
    }

    [Test]
    public void ToDomain_ProcessModifyRequest_ShouldMapCorrectly()
    {
        // Arrange
        var processId = 777L;
        var request = new ProcessModifyRequest
        {
            Name = "UpdatedProcess"
        };

        // Act
        var result = request.ToDomain(processId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(processId));
        Assert.That(result.Name, Is.EqualTo("UpdatedProcess"));
    }

    [Test]
    public void ToDomain_PolicyDto_ShouldMapCorrectly()
    {
        // Arrange
        var policyDto = new PolicyDto
        {
            Id = 888,
            Name = "TestPolicy",
            Description = "Test Description",
            RegistryPath = "HKLM\\Software\\Test",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "TestKey"
        };

        // Act
        var result = policyDto.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(888));
        Assert.That(result.Name, Is.EqualTo("TestPolicy"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Software\\Test"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.String));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("TestKey"));
    }

    [Test]
    public void ToDomain_PolicyCreateRequest_ShouldMapCorrectly()
    {
        // Arrange
        var request = new PolicyCreateRequest
        {
            Name = "NewPolicy",
            Description = "New Description",
            RegistryPath = "HKLM\\Software\\New",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };

        // Act
        var result = request.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("NewPolicy"));
        Assert.That(result.Description, Is.EqualTo("New Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Software\\New"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.Dword));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("NewKey"));
        Assert.That(result.Id, Is.EqualTo(0)); // Should be default for new entity
    }

    [Test]
    public void ToDomain_PolicyModifyRequest_ShouldMapCorrectly()
    {
        // Arrange
        var policyId = 666L;
        var request = new PolicyModifyRequest
        {
            Name = "UpdatedPolicy",
            Description = "Updated Description",
            RegistryPath = "HKLM\\Software\\Updated",
            RegistryValueType = RegistryValueType.Binary,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "UpdatedKey"
        };

        // Act
        var result = request.ToDomain(policyId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(policyId));
        Assert.That(result.Name, Is.EqualTo("UpdatedPolicy"));
        Assert.That(result.Description, Is.EqualTo("Updated Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Software\\Updated"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.Binary));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("UpdatedKey"));
    }

    [Test]
    public void ToDomain_WithEmptyStrings_ShouldMapCorrectly()
    {
        // Arrange
        var agentRequest = new AgentCreateRequest
        {
            Name = "",
            ConfigurationId = 0
        };

        var processRequest = new ProcessCreateRequest
        {
            Name = ""
        };

        var policyRequest = new PolicyCreateRequest
        {
            Name = "",
            Description = "",
            RegistryPath = "",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = ""
        };

        // Act
        var agentResult = agentRequest.ToDomain(new byte[] { }, new byte[] { });
        var processResult = processRequest.ToDomain();
        var policyResult = policyRequest.ToDomain();

        // Assert
        Assert.That(agentResult.Name, Is.EqualTo(""));
        Assert.That(agentResult.ConfigurationId, Is.EqualTo(0));

        Assert.That(processResult.Name, Is.EqualTo(""));

        Assert.That(policyResult.Name, Is.EqualTo(""));
        Assert.That(policyResult.Description, Is.EqualTo(""));
        Assert.That(policyResult.RegistryPath, Is.EqualTo(""));
        Assert.That(policyResult.RegistryValueType, Is.EqualTo(RegistryValueType.String));
        Assert.That(policyResult.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(policyResult.RegistryKey, Is.EqualTo(""));
    }


    [Test]
    public void ToDomain_WithEmptyCollections_ShouldMapCorrectly()
    {
        // Arrange
        var configDto = new ConfigurationDto
        {
            Id = 123,
            Name = "TestConfig",
            AgentIds = new List<Guid>(),
            Processes = new List<ProcessInConfigurationDto>(),
            Policies = new List<PolicyInConfigurationDto>()
        };

        // Act
        var result = configDto.ToDomain();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(123));
        Assert.That(result.Name, Is.EqualTo("TestConfig"));
        Assert.That(result.Agents, Is.Empty);
        Assert.That(result.Processes, Is.Empty);
        Assert.That(result.Policies, Is.Empty);
    }
}
