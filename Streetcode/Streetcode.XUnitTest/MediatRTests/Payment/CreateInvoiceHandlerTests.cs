using Xunit;
using Moq;
using Streetcode.BLL.MediatR.Payment;
using Streetcode.BLL.Interfaces.Payment;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.BLL.DTO.Payment;
using Streetcode.DAL.Entities.Payment;
using Streetcode.BLL.Services.Payment.Exceptions;

namespace Streetcode.XUnitTest.MediatRTests.Payment;

public class CreateInvoiceHandlerTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock = new();
    private readonly Mock<ILoggerService> _loggerMock = new();
    private readonly CreateInvoiceHandler _handler;

    public CreateInvoiceHandlerTests() =>
        _handler = new(_paymentServiceMock.Object, _loggerMock.Object);

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenInvoiceCreated()
    {
        var invoiceInfo = new InvoiceInfo("12345", "https://invoice.url");
        _paymentServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(invoiceInfo);

        var command = new CreateInvoiceCommand(
            new PaymentDTO { Amount = 100, RedirectUrl = "https://test.com" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectInvoiceInfo_WhenInvoiceCreated()
    {
        var invoiceInfo = new InvoiceInfo("12345", "https://invoice.url");
        _paymentServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ReturnsAsync(invoiceInfo);

        var command = new CreateInvoiceCommand(
            new PaymentDTO { Amount = 100, RedirectUrl = "https://test.com" });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(invoiceInfo, result.Value);
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidRequestParameterException_WhenBadRequest()
    {
        _paymentServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ThrowsAsync(new InvalidRequestParameterException(new Error("400", "Bad Request")));

        var command = new CreateInvoiceCommand(
            new PaymentDTO { Amount = 100, RedirectUrl = "https://test.com" });

        await Assert.ThrowsAsync<InvalidRequestParameterException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidTokenException_WhenForbidden()
    {
        _paymentServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ThrowsAsync(new InvalidTokenException());

        var command = new CreateInvoiceCommand(
            new PaymentDTO { Amount = 100, RedirectUrl = "https://test.com" });

        await Assert.ThrowsAsync<InvalidTokenException>(
            () => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowNotSupportedException_WhenUnexpectedError()
    {
        _paymentServiceMock.Setup(s => s.CreateInvoiceAsync(It.IsAny<Invoice>()))
            .ThrowsAsync(new NotSupportedException());

        var command = new CreateInvoiceCommand(
            new PaymentDTO { Amount = 100, RedirectUrl = "https://test.com" });

        await Assert.ThrowsAsync<NotSupportedException>(
            () => _handler.Handle(command, CancellationToken.None));
    }
}
