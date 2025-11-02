using Application.Common.Results;
using MediatR;

namespace Application.Features.People.Commands.DeletePerson;

public readonly record struct DeletePersonCommand(int Id) : IRequest<Result<bool>>;

