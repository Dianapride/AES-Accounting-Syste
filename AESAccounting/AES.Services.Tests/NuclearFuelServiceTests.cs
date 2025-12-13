using AES.Core.Entities;
using AES.Data;
using AES.Services;
using Moq;
using Xunit;

namespace AES.Services.Tests
{
    public class NuclearFuelServiceTests
    {
        private readonly Mock<INuclearFuelRepository> _mockRepo;
        private readonly Mock<ISauzYamClient> _mockSauz;
        private readonly NuclearFuelService _service;

        public NuclearFuelServiceTests()
        {
            _mockRepo = new Mock<INuclearFuelRepository>();
            _mockSauz = new Mock<ISauzYamClient>();
            _service = new NuclearFuelService(_mockRepo.Object, _mockSauz.Object);
        }

        [Fact]
        public async Task RegisterFuelBatchAsync_ValidData_AllCallsMade_ReturnsSuccess()
        {
            // Arrange
            var batch = new NuclearFuelBatch
            {
                BatchNumber = "45871",
                TvzNumber = "ТВЗ-45871",
                Quantity = 100,
                UraniumMassKg = 45000,
                EnrichmentPercent = 4.5m,
                StorageZone = "Зона 1"
            };

            _mockRepo.Setup(r => r.AddBatchWithRecordAsync(batch)).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.UpdateBatchAsync(batch)).Returns(Task.CompletedTask);
            _mockSauz.Setup(c => c.RegisterMaterialAsync(It.IsAny<string>(), 45000, 4.5m))
                     .ReturnsAsync(new SauzYamResponse { IsSuccess = true });

            // Act
            var result = await _service.RegisterFuelBatchAsync(batch);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("Підтвердження від САУЗ ЯМ отримано", result.Message);

            _mockRepo.Verify(r => r.AddBatchWithRecordAsync(batch), Times.Once);
            _mockSauz.Verify(c => c.RegisterMaterialAsync(It.IsAny<string>(), 45000, 4.5m), Times.Once);
            _mockRepo.Verify(r => r.UpdateBatchAsync(batch), Times.Once);
        }

        [Fact]
        public async Task RegisterFuelBatchAsync_InvalidEnrichment_ReturnsFail_NoSauzCall()
        {
     
            var batch = new NuclearFuelBatch
            {
                BatchNumber = "45871",
                TvzNumber = "ТВЗ-45871",
                Quantity = 100,
                UraniumMassKg = 45000,
                EnrichmentPercent = 25m,      
                StorageZone = "Зона 1"
            };

 
            var result = await _service.RegisterFuelBatchAsync(batch);

        
            Assert.False(result.IsSuccess);
            Assert.Contains("Збагачення повинно бути в діапазоні 1–20%", result.Message);

            _mockRepo.Verify(r => r.AddBatchWithRecordAsync(It.IsAny<NuclearFuelBatch>()), Times.Never);
            _mockSauz.Verify(c => c.RegisterMaterialAsync(It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Never);
        }

        [Fact]
        public async Task RegisterFuelBatchAsync_SauzFails_ReturnsFail()
        {
            var batch = new NuclearFuelBatch
            {
                BatchNumber = "12345",
                TvzNumber = "ТВЗ-12345",
                Quantity = 50,
                UraniumMassKg = 20000,
                EnrichmentPercent = 3.6m,
                StorageZone = "Зона 2"
            };

            _mockRepo.Setup(r => r.AddBatchWithRecordAsync(batch)).Returns(Task.CompletedTask);
            _mockSauz.Setup(c => c.RegisterMaterialAsync(It.IsAny<string>(), 20000, 3.6m))
                     .ReturnsAsync(new SauzYamResponse { IsSuccess = false, ErrorMessage = "Дублювання" });

            var result = await _service.RegisterFuelBatchAsync(batch);

            Assert.False(result.IsSuccess);
            Assert.Contains("Помилка САУЗ ЯМ", result.Message);
        }
    }
}