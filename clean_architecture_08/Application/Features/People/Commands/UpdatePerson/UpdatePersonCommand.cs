using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.People.Commands.UpdatePerson;

public readonly record struct UpdatePersonCommand(int Id, string LastName, string FirstName, string MailAddress)
    : IRequest<Result<GetPersonDto>>;

