using Server.Application.Abstractions;
using Server.Application.Dtos.Policy;
using Server.Application.Services;
using Server.Domain.Models;

namespace Server.Application.Tests.Services;

[TestFixture]
public class PolicyServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IPolicyRepository> _mockPolicyRepository = null!;
    private PolicyService _policyService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockPolicyRepository = new Mock<IPolicyRepository>();
        _mockUnitOfWork.Setup(x => x.Policies).Returns(_mockPolicyRepository.Object);
        _policyService = new PolicyService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task GetPoliciesAsync_ShouldReturnAllPolicies()
    {
        // Arrange
        var policies = new List<Policy>
        {
            new() { Id = 1, Name = "Policy1", Description = "Test Policy 1", RegistryPath = "HKLM\\Test1", RegistryValueType = RegistryValueType.String, RegistryKeyType = RegistryKeyType.Hklm, RegistryKey = "TestKey1" },
            new() { Id = 2, Name = "Policy2", Description = "Test Policy 2", RegistryPath = "HKLM\\Test2", RegistryValueType = RegistryValueType.Dword, RegistryKeyType = RegistryKeyType.Hklm, RegistryKey = "TestKey2" }
        };

        _mockPolicyRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(policies);

        // Act
        var result = (await _policyService.GetPoliciesAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Policy1"));
        Assert.That(result.Last().Name, Is.EqualTo("Policy2"));
        
        _mockPolicyRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetPoliciesAsync_WhenNoPoliciesExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockPolicyRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Policy>());

        // Act
        var result = (await _policyService.GetPoliciesAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
        
        _mockPolicyRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetPolicyAsync_WhenPolicyExists_ShouldReturnPolicy()
    {
        // Arrange
        var policyId = 1L;
        var policy = new Policy 
        { 
            Id = policyId, 
            Name = "TestPolicy", 
            Description = "Test Description",
            RegistryPath = "HKLM\\Test",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "TestKey"
        };
        
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(policy);

        // Act
        var result = await _policyService.GetPolicyAsync(policyId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(policyId));
        Assert.That(result.Name, Is.EqualTo("TestPolicy"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Test"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.String));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("TestKey"));
        
        _mockPolicyRepository.Verify(x => x.GetAsync(policyId), Times.Once);
    }

    [Test]
    public async Task GetPolicyAsync_WhenPolicyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var policyId = 1L;
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync((Policy?)null);

        // Act
        var result = await _policyService.GetPolicyAsync(policyId);

        // Assert
        Assert.That(result, Is.Null);
        
        _mockPolicyRepository.Verify(x => x.GetAsync(policyId), Times.Once);
    }

    [Test]
    public async Task CreatePolicyAsync_ShouldCreateAndReturnPolicy()
    {
        // Arrange
        var request = new PolicyCreateRequest 
        { 
                Name = "NewPolicy", 
                Description = "New Description",
                RegistryPath = "HKLM\\New",
                RegistryValueType = RegistryValueType.String,
                RegistryKeyType = RegistryKeyType.Hklm,
                RegistryKey = "NewKey"
        };

        Policy? createdPolicy = null;

        _mockPolicyRepository
                .Setup(x => x.CreateAsync(It.IsAny<Policy>()))
                .Callback<Policy>(p => createdPolicy = p)
                .Returns(Task.CompletedTask);

        _mockPolicyRepository
                .Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(() => createdPolicy);

        _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

        // Act
        var result = await _policyService.CreatePolicyAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("NewPolicy"));
        Assert.That(result.Description, Is.EqualTo("New Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\New"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.String));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("NewKey"));
        Assert.That(result.Id, Is.EqualTo(createdPolicy!.Id)); // тут вже дійсний Id

        _mockPolicyRepository.Verify(x => x.CreateAsync(It.IsAny<Policy>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdatePolicyAsync_WhenPolicyExists_ShouldUpdateAndReturnPolicy()
    {
        // Arrange
        var policyId = 1L;
        var existingPolicy = new Policy 
        { 
            Id = policyId, 
            Name = "OldName", 
            Description = "Old Description",
            RegistryPath = "HKLM\\Old",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "OldKey"
        };
        var request = new PolicyModifyRequest 
        { 
            Name = "NewName", 
            Description = "New Description",
            RegistryPath = "HKLM\\New",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };
        var updatedPolicy = new Policy 
        { 
            Id = policyId, 
            Name = "NewName", 
            Description = "New Description",
            RegistryPath = "HKLM\\New",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };
        
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(existingPolicy);
        _mockPolicyRepository.Setup(x => x.ModifyAsync(It.IsAny<Policy>())).Returns(Task.CompletedTask);
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(updatedPolicy);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _policyService.UpdatePolicyAsync(policyId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(policyId));
        Assert.That(result.Name, Is.EqualTo("NewName"));
        Assert.That(result.Description, Is.EqualTo("New Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\New"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.Dword));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("NewKey"));
        
        _mockPolicyRepository.Verify(x => x.GetAsync(policyId), Times.Exactly(2));
        _mockPolicyRepository.Verify(x => x.ModifyAsync(It.IsAny<Policy>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdatePolicyAsync_WhenPolicyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var policyId = 1L;
        var request = new PolicyModifyRequest 
        { 
            Name = "NewName", 
            Description = "New Description",
            RegistryPath = "HKLM\\New",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };
        
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync((Policy?)null);

        // Act
        var result = await _policyService.UpdatePolicyAsync(policyId, request);

        // Assert
        Assert.That(result, Is.Null);
        
        _mockPolicyRepository.Verify(x => x.GetAsync(policyId), Times.Once);
        _mockPolicyRepository.Verify(x => x.ModifyAsync(It.IsAny<Policy>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreatePolicyAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var request = new PolicyCreateRequest 
        { 
            Name = "TestPolicy", 
            Description = "Test Description",
            RegistryPath = "HKLM\\Test",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "TestKey"
        };
        var createdPolicy = new Policy 
        { 
            Id = 1, 
            Name = "TestPolicy", 
            Description = "Test Description",
            RegistryPath = "HKLM\\Test",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "TestKey"
        };
        
        _mockPolicyRepository.Setup(x => x.CreateAsync(It.IsAny<Policy>())).Returns(Task.CompletedTask);
        _mockPolicyRepository.Setup(x => x.GetAsync(createdPolicy.Id)).ReturnsAsync(createdPolicy);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _policyService.CreatePolicyAsync(request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdatePolicyAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var policyId = 1L;
        var existingPolicy = new Policy 
        { 
            Id = policyId, 
            Name = "OldName", 
            Description = "Old Description",
            RegistryPath = "HKLM\\Old",
            RegistryValueType = RegistryValueType.String,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "OldKey"
        };
        var request = new PolicyModifyRequest 
        { 
            Name = "NewName", 
            Description = "New Description",
            RegistryPath = "HKLM\\New",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };
        var updatedPolicy = new Policy 
        { 
            Id = policyId, 
            Name = "NewName", 
            Description = "New Description",
            RegistryPath = "HKLM\\New",
            RegistryValueType = RegistryValueType.Dword,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "NewKey"
        };
        
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(existingPolicy);
        _mockPolicyRepository.Setup(x => x.ModifyAsync(It.IsAny<Policy>())).Returns(Task.CompletedTask);
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(updatedPolicy);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _policyService.UpdatePolicyAsync(policyId, request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetPolicyAsync_WithAllProperties_ShouldReturnCompletePolicy()
    {
        // Arrange
        var policyId = 1L;
        var policy = new Policy 
        { 
            Id = policyId, 
            Name = "CompletePolicy", 
            Description = "Complete Description",
            RegistryPath = "HKLM\\Software\\Test",
            RegistryValueType = RegistryValueType.Binary,
            RegistryKeyType = RegistryKeyType.Hklm,
            RegistryKey = "CompleteKey"
        };
        
        _mockPolicyRepository.Setup(x => x.GetAsync(policyId)).ReturnsAsync(policy);

        // Act
        var result = await _policyService.GetPolicyAsync(policyId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(policyId));
        Assert.That(result.Name, Is.EqualTo("CompletePolicy"));
        Assert.That(result.Description, Is.EqualTo("Complete Description"));
        Assert.That(result.RegistryPath, Is.EqualTo("HKLM\\Software\\Test"));
        Assert.That(result.RegistryValueType, Is.EqualTo(RegistryValueType.Binary));
        Assert.That(result.RegistryKeyType, Is.EqualTo(RegistryKeyType.Hklm));
        Assert.That(result.RegistryKey, Is.EqualTo("CompleteKey"));
        
        _mockPolicyRepository.Verify(x => x.GetAsync(policyId), Times.Once);
    }
} 