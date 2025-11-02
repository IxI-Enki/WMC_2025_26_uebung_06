using Application.Common.Results;
using MediatR;

namespace Application.Features.Devices.Commands.DeleteDevice;

public readonly record struct DeleteDeviceCommand(int Id) : IRequest<Result<bool>>;

