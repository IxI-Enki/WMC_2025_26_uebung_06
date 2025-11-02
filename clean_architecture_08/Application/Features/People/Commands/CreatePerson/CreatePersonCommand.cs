using Application.Common.Results;
using Application.Features.Dtos;
using MediatR;

namespace Application.Features.People.Commands.CreatePerson;

public readonly record struct CreatePersonCommand(string LastName, string FirstName, string MailAddress)
    : IRequest<Result<GetPersonDto>>;

