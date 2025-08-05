using Server.Application.Dtos.Agent;
using Server.Application.Dtos.Configuration;
using Server.Application.Dtos.Policy;
using Server.Application.Dtos.Process;
using Server.Domain.Models;

namespace Server.Application.Tests.Helpers;

public static class TestDataHelper
{
    public static class Agents
    {
        public static Agent CreateSampleAgent(Guid? id = null, string? name = null, long configurationId = 1)
        {
            return new Agent
            {
                Id = id ?? Guid.NewGuid(),
                Name = name ?? "TestAgent",
                ConfigurationId = configurationId
            };
        }

        public static AgentCreateRequest CreateSampleAgentCreateRequest(string? name = null, long configurationId = 1)
        {
            return new AgentCreateRequest
            {
                Name = name ?? "NewAgent",
                ConfigurationId = configurationId
            };
        }

        public static AgentModifyRequest CreateSampleAgentModifyRequest(string? name = null, long configurationId = 1)
        {
            return new AgentModifyRequest
            {
                Name = name ?? "UpdatedAgent",
                ConfigurationId = configurationId
            };
        }

        public static List<Agent> CreateSampleAgentList(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateSampleAgent(name: $"Agent{i}", configurationId: i))
                .ToList();
        }
    }

    public static class Configurations
    {
        public static Configuration CreateSampleConfiguration(long? id = null, string? name = null)
        {
            return new Configuration
            {
                Id = id ?? 1,
                Name = name ?? "TestConfig",
                Agents = new List<Agent>(),
                Processes = new List<ProcessInConfiguration>(),
                Policies = new List<PolicyInConfiguration>()
            };
        }

        public static ConfigurationCreateRequest CreateSampleConfigurationCreateRequest(string? name = null)
        {
            return new ConfigurationCreateRequest
            {
                Name = name ?? "NewConfig",
                Processes = new List<ProcessInConfigurationDto>(),
                Policies = new List<PolicyInConfigurationDto>()
            };
        }

        public static ConfigurationModifyRequest CreateSampleConfigurationModifyRequest(string? name = null)
        {
            return new ConfigurationModifyRequest
            {
                Name = name ?? "UpdatedConfig",
                Processes = new List<ProcessInConfigurationDto>(),
                Policies = new List<PolicyInConfigurationDto>()
            };
        }

        public static List<Configuration> CreateSampleConfigurationList(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateSampleConfiguration(id: i, name: $"Config{i}"))
                .ToList();
        }
    }

    public static class Processes
    {
        public static Process CreateSampleProcess(long? id = null, string? name = null)
        {
            return new Process
            {
                Id = id ?? 1,
                Name = name ?? "TestProcess"
            };
        }

        public static ProcessCreateRequest CreateSampleProcessCreateRequest(string? name = null)
        {
            return new ProcessCreateRequest
            {
                Name = name ?? "NewProcess"
            };
        }

        public static ProcessModifyRequest CreateSampleProcessModifyRequest(string? name = null)
        {
            return new ProcessModifyRequest
            {
                Name = name ?? "UpdatedProcess"
            };
        }

        public static List<Process> CreateSampleProcessList(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateSampleProcess(id: i, name: $"Process{i}"))
                .ToList();
        }
    }

    public static class Policies
    {
        public static Policy CreateSamplePolicy(long? id = null, string? name = null)
        {
            return new Policy
            {
                Id = id ?? 1,
                Name = name ?? "TestPolicy",
                Description = "Test Description",
                RegistryPath = "HKLM\\Software\\Test",
                RegistryValueType = RegistryValueType.String,
                RegistryKeyType = RegistryKeyType.Hklm,
                RegistryKey = "TestKey"
            };
        }

        public static PolicyCreateRequest CreateSamplePolicyCreateRequest(string? name = null)
        {
            return new PolicyCreateRequest
            {
                Name = name ?? "NewPolicy",
                Description = "New Description",
                RegistryPath = "HKLM\\Software\\New",
                RegistryValueType = RegistryValueType.Dword,
                RegistryKeyType = RegistryKeyType.Hklm,
                RegistryKey = "NewKey"
            };
        }

        public static PolicyModifyRequest CreateSamplePolicyModifyRequest(string? name = null)
        {
            return new PolicyModifyRequest
            {
                Name = name ?? "UpdatedPolicy",
                Description = "Updated Description",
                RegistryPath = "HKLM\\Software\\Updated",
                RegistryValueType = RegistryValueType.Binary,
                RegistryKeyType = RegistryKeyType.Hklm,
                RegistryKey = "UpdatedKey"
            };
        }

        public static List<Policy> CreateSamplePolicyList(int count = 3)
        {
            return Enumerable.Range(1, count)
                .Select(i => CreateSamplePolicy(id: i, name: $"Policy{i}"))
                .ToList();
        }
    }

    public static class ProcessInConfigurations
    {
        public static ProcessInConfiguration CreateSampleProcessInConfiguration(long processId = 1, ProcessState? processState = null, long configurationId = 1)
        {
            return new ProcessInConfiguration
            {
                ProcessId = processId,
                ProcessState = processState ?? ProcessState.Active,
                ConfigurationId = configurationId
            };
        }

        public static ProcessInConfigurationDto CreateSampleProcessInConfigurationDto(long processId = 1, ProcessState? processState = null)
        {
            return new ProcessInConfigurationDto
            {
                ProcessId = processId,
                ProcessState = processState ?? ProcessState.Active
            };
        }
    }

    public static class PolicyInConfigurations
    {
        public static PolicyInConfiguration CreateSamplePolicyInConfiguration(long policyId = 1, string? registryValue = null, long configurationId = 1)
        {
            return new PolicyInConfiguration
            {
                PolicyId = policyId,
                RegistryValue = registryValue ?? "TestValue",
                ConfigurationId = configurationId
            };
        }

        public static PolicyInConfigurationDto CreateSamplePolicyInConfigurationDto(long policyId = 1, string? registryValue = null)
        {
            return new PolicyInConfigurationDto
            {
                PolicyId = policyId,
                RegistryValue = registryValue ?? "TestValue"
            };
        }
    }

    public static class EdgeCases
    {
        public static AgentCreateRequest CreateEmptyAgentRequest()
        {
            return new AgentCreateRequest
            {
                Name = "",
                ConfigurationId = 0
            };
        }

        public static ProcessCreateRequest CreateEmptyProcessRequest()
        {
            return new ProcessCreateRequest
            {
                Name = ""
            };
        }

        public static PolicyCreateRequest CreateEmptyPolicyRequest()
        {
            return new PolicyCreateRequest
            {
                Name = "",
                Description = "",
                RegistryPath = "",
                RegistryValueType = RegistryValueType.String,
                RegistryKeyType = RegistryKeyType.Hklm,
                RegistryKey = ""
            };
        }

        public static ConfigurationCreateRequest CreateEmptyConfigurationRequest()
        {
            return new ConfigurationCreateRequest
            {
                Name = "",
                Processes = new List<ProcessInConfigurationDto>(),
                Policies = new List<PolicyInConfigurationDto>()
            };
        }
    }
} 