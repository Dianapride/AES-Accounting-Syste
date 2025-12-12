// Файл: AES.DataAccess.Tests/NuclearFuelRepositoryTests.cs
using AES.Core.Entities;
using AES.DataAccess;
using Moq;
using Xunit;

namespace AES.DataAccess.Tests
{
    public class NuclearFuelRepositoryTests
    {
        private readonly Mock<INuclearFuelRepository> _mockRepo;

        public NuclearFuelRepositoryTests()
        {
            _mockRepo = new Mock<INuclearFuelRepository>();
        }

        [Fact]
        public async Task AddBatchWithRecordAsync_ValidBatch_CallsRepositoryOnce()
        {
            // Arrange
            var batch = new NuclearFuelBatch
            {
                BatchNumber = "BATCH-2025-001",
                StorageZone = "Зона А1"
            };

            // Act
            await _mockRepo.Object.AddBatchWithRecordAsync(batch);

            // Assert
            _mockRepo.Verify(r => r.AddBatchWithRecordAsync(batch), Times.Once);
        }

        [Fact]
        public async Task GetByBatchNumberAsync_ExistingBatch_ReturnsBatch()
        {
            // Arrange
            var expectedBatch = new NuclearFuelBatch
            {
                BatchNumber = "BATCH-2025-001",
                StorageZone = "Зона А1"
            };

            _mockRepo.Setup(r => r.GetByBatchNumberAsync("BATCH-2025-001"))
                     .ReturnsAsync(expectedBatch);

            // Act
            var result = await _mockRepo.Object.GetByBatchNumberAsync("BATCH-2025-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("BATCH-2025-001", result.BatchNumber);
        }

        [Fact]
        public async Task GetBatchesInZoneAsync_ZoneExists_ReturnsCollection()
        {
            // Arrange
            var expectedBatches = new List<NuclearFuelBatch>
            {
                new() { BatchNumber = "B-001", StorageZone = "Зона А1" },
                new() { BatchNumber = "B-002", StorageZone = "Зона А1" }
            };

            _mockRepo.Setup(r => r.GetBatchesInZoneAsync("Зона А1"))
                     .ReturnsAsync(expectedBatches);

            // Act
            var result = await _mockRepo.Object.GetBatchesInZoneAsync("Зона А1");

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}