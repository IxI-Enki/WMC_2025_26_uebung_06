using Application.Common.Results;
using Application.Features.Dtos;
using Domain.Entities;
using MediatR;

namespace Application.Features.Devices.Commands.CreateDevice;

public readonly record struct CreateDeviceCommand(string SerialNumber, string DeviceName, DeviceType DeviceType)
    : IRequest<Result<GetDeviceDto>>;

