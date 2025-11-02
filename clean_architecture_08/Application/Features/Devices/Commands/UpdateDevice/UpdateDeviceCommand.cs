using Application.Common.Results;
using Application.Features.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Devices.Commands.UpdateDevice;

public readonly record struct UpdateDeviceCommand(int Id, string SerialNumber, string DeviceName, DeviceType DeviceType)
    : IRequest<Result<GetDeviceDto>>;

