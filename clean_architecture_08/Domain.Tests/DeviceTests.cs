using Domain.Entities;
using Domain.Exceptions;
using Xunit;

namespace Domain.Tests;

/// <summary>
/// Unit-Tests für Device-Entität und deren Validierungen.
/// </summary>
public class DeviceTests
{
    [Fact]
    public void Create_Succeeds_WithValidData()
    {
        // Arrange & Act
        var device = Device.Create("12345", "Samsung Galaxy", DeviceType.SmartPhone);

        // Assert
        Assert.Equal("12345", device.SerialNumber);
        Assert.Equal("Samsung Galaxy", device.DeviceName);
        Assert.Equal(DeviceType.SmartPhone, device.DeviceType);
    }

    [Theory]
    [InlineData(null, "Device", "Seriennummer darf nicht leer sein.")]
    [InlineData("", "Device", "Seriennummer darf nicht leer sein.")]
    [InlineData("12", "Device", "Seriennummer muss mindestens 3 Zeichen haben.")]
    [InlineData("123", "", "Gerätename darf nicht leer sein.")]
    [InlineData("123", "D", "Gerätename muss mindestens 2 Zeichen haben.")]
    public void Create_InvalidRules_Throws(string? serialNumber, string? deviceName, string expectedMessage)
    {
        // Arrange & Act
        var ex = Assert.Throws<DomainValidationException>(() => 
            Device.Create(serialNumber ?? string.Empty, deviceName ?? string.Empty, DeviceType.Tablet));

        // Assert
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void Update_ChangesValues_WhenValid()
    {
        // Arrange
        var device = Device.Create("123", "Old Name", DeviceType.Tablet);

        // Act
        device.Update("456", "New Name", DeviceType.SmartPhone);

        // Assert
        Assert.Equal("456", device.SerialNumber);
        Assert.Equal("New Name", device.DeviceName);
        Assert.Equal(DeviceType.SmartPhone, device.DeviceType);
    }

    [Fact]
    public void Update_InvalidSerialNumber_Throws()
    {
        // Arrange
        var device = Device.Create("123", "Device", DeviceType.Tablet);

        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() => 
            device.Update("12", "Device", DeviceType.Tablet));
        Assert.Equal("Seriennummer muss mindestens 3 Zeichen haben.", ex.Message);
    }

    [Fact]
    public void Update_SameValues_NoChange()
    {
        // Arrange
        var device = Device.Create("123", "Device", DeviceType.Tablet);
        var originalToString = device.ToString();

        // Act
        device.Update("123", "Device", DeviceType.Tablet);

        // Assert
        Assert.Equal("123", device.SerialNumber);
        Assert.Equal("Device", device.DeviceName);
        Assert.Equal(DeviceType.Tablet, device.DeviceType);
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var device = Device.Create("123", "Test Device", DeviceType.Notebook);

        // Act
        var result = device.ToString();

        // Assert
        Assert.Equal("Test Device (123)", result);
    }
}

