using Server.Application.Abstractions;
using Server.Application.Dtos.Process;
using Server.Application.Services;
using Server.Domain.Models;

namespace Server.Application.Tests.Services;

[TestFixture]
public class ProcessServiceTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork = null!;
    private Mock<IProcessRepository> _mockProcessRepository = null!;
    private ProcessService _processService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProcessRepository = new Mock<IProcessRepository>();
        _mockUnitOfWork.Setup(x => x.Processes).Returns(_mockProcessRepository.Object);
        _processService = new ProcessService(_mockUnitOfWork.Object);
    }

    [Test]
    public async Task GetProcessesAsync_ShouldReturnAllProcesses()
    {
        // Arrange
        var processes = new List<Process>
        {
            new() { Id = 1, Name = "Process1" },
            new() { Id = 2, Name = "Process2" }
        };

        _mockProcessRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(processes);

        // Act
        var result = (await _processService.GetProcessesAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.First().Name, Is.EqualTo("Process1"));
        Assert.That(result.Last().Name, Is.EqualTo("Process2"));
        
        _mockProcessRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetProcessesAsync_WhenNoProcessesExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockProcessRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Process>());

        // Act
        var result = await _processService.GetProcessesAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
        
        _mockProcessRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetProcessAsync_WhenProcessExists_ShouldReturnProcess()
    {
        // Arrange
        var processId = 1L;
        var process = new Process { Id = processId, Name = "TestProcess" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(process);

        // Act
        var result = await _processService.GetProcessAsync(processId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(processId));
        Assert.That(result.Name, Is.EqualTo("TestProcess"));
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Once);
    }

    [Test]
    public async Task GetProcessAsync_WhenProcessDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var processId = 1L;
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync((Process?)null);

        // Act
        var result = await _processService.GetProcessAsync(processId);

        // Assert
        Assert.That(result, Is.Null);
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Once);
    }

    [Test]
    public async Task CreateProcessAsync_ShouldCreateAndReturnProcess()
    {
        // Arrange
        var request = new ProcessCreateRequest { Name = "NewProcess" };
        Process? createdProcess = null;

        _mockProcessRepository
                .Setup(x => x.CreateAsync(It.IsAny<Process>()))
                .Callback<Process>(p => createdProcess = p)
                .Returns(Task.CompletedTask);

        _mockProcessRepository
                .Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(() => createdProcess);

        _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

        // Act
        var result = await _processService.CreateProcessAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo("NewProcess"));
        Assert.That(result.Id, Is.EqualTo(createdProcess!.Id));

        _mockProcessRepository.Verify(x => x.CreateAsync(It.IsAny<Process>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
    
    [Test]
    public async Task UpdateProcessAsync_WhenProcessExists_ShouldUpdateAndReturnProcess()
    {
        // Arrange
        var processId = 1L;
        var existingProcess = new Process { Id = processId, Name = "OldName" };
        var request = new ProcessModifyRequest { Name = "NewName" };
        var updatedProcess = new Process { Id = processId, Name = "NewName" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(existingProcess);
        _mockProcessRepository.Setup(x => x.ModifyAsync(It.IsAny<Process>())).Returns(Task.CompletedTask);
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(updatedProcess);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _processService.UpdateProcessAsync(processId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(processId));
        Assert.That(result.Name, Is.EqualTo("NewName"));
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Exactly(2));
        _mockProcessRepository.Verify(x => x.ModifyAsync(It.IsAny<Process>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateProcessAsync_WhenProcessDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var processId = 1L;
        var request = new ProcessModifyRequest { Name = "NewName" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync((Process?)null);

        // Act
        var result = await _processService.UpdateProcessAsync(processId, request);

        // Assert
        Assert.That(result, Is.Null);
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Once);
        _mockProcessRepository.Verify(x => x.ModifyAsync(It.IsAny<Process>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task CreateProcessAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var request = new ProcessCreateRequest { Name = "TestProcess" };
        var createdProcess = new Process { Id = 1, Name = "TestProcess" };
        
        _mockProcessRepository.Setup(x => x.CreateAsync(It.IsAny<Process>())).Returns(Task.CompletedTask);
        _mockProcessRepository.Setup(x => x.GetAsync(createdProcess.Id)).ReturnsAsync(createdProcess);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _processService.CreateProcessAsync(request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateProcessAsync_ShouldCallSaveChanges()
    {
        // Arrange
        var processId = 1L;
        var existingProcess = new Process { Id = processId, Name = "OldName" };
        var request = new ProcessModifyRequest { Name = "NewName" };
        var updatedProcess = new Process { Id = processId, Name = "NewName" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(existingProcess);
        _mockProcessRepository.Setup(x => x.ModifyAsync(It.IsAny<Process>())).Returns(Task.CompletedTask);
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(updatedProcess);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _processService.UpdateProcessAsync(processId, request);

        // Assert
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task GetProcessAsync_WithLongId_ShouldReturnProcess()
    {
        // Arrange
        var processId = long.MaxValue;
        var process = new Process { Id = processId, Name = "LargeIdProcess" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(process);

        // Act
        var result = await _processService.GetProcessAsync(processId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(processId));
        Assert.That(result.Name, Is.EqualTo("LargeIdProcess"));
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Once);
    }

    [Test]
    public async Task GetProcessesAsync_WithMultipleProcesses_ShouldReturnAllProcesses()
    {
        // Arrange
        var processes = new List<Process>
        {
            new() { Id = 1, Name = "Process1" },
            new() { Id = 2, Name = "Process2" },
            new() { Id = 3, Name = "Process3" },
            new() { Id = 4, Name = "Process4" }
        };

        _mockProcessRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(processes);

        // Act
        var result = (await _processService.GetProcessesAsync()).ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(4));
        Assert.That(result.ElementAt(0).Name, Is.EqualTo("Process1"));
        Assert.That(result.ElementAt(1).Name, Is.EqualTo("Process2"));
        Assert.That(result.ElementAt(2).Name, Is.EqualTo("Process3"));
        Assert.That(result.ElementAt(3).Name, Is.EqualTo("Process4"));
        
        _mockProcessRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task CreateProcessAsync_WithEmptyName_ShouldCreateProcess()
    {
        // Arrange
        var request = new ProcessCreateRequest { Name = "" };
        Process? createdProcess = null;

        _mockProcessRepository
                .Setup(x => x.CreateAsync(It.IsAny<Process>()))
                .Callback<Process>(p => createdProcess = p)
                .Returns(Task.CompletedTask);

        _mockProcessRepository
                .Setup(x => x.GetAsync(It.IsAny<long>()))
                .ReturnsAsync(() => createdProcess);

        _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

        // Act
        var result = await _processService.CreateProcessAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Name, Is.EqualTo(""));
        Assert.That(result.Id, Is.EqualTo(createdProcess!.Id));

        _mockProcessRepository.Verify(x => x.CreateAsync(It.IsAny<Process>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateProcessAsync_WithEmptyName_ShouldUpdateProcess()
    {
        // Arrange
        var processId = 1L;
        var existingProcess = new Process { Id = processId, Name = "OldName" };
        var request = new ProcessModifyRequest { Name = "" };
        var updatedProcess = new Process { Id = processId, Name = "" };
        
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(existingProcess);
        _mockProcessRepository.Setup(x => x.ModifyAsync(It.IsAny<Process>())).Returns(Task.CompletedTask);
        _mockProcessRepository.Setup(x => x.GetAsync(processId)).ReturnsAsync(updatedProcess);
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _processService.UpdateProcessAsync(processId, request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(processId));
        Assert.That(result.Name, Is.EqualTo(""));
        
        _mockProcessRepository.Verify(x => x.GetAsync(processId), Times.Exactly(2));
        _mockProcessRepository.Verify(x => x.ModifyAsync(It.IsAny<Process>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
} 