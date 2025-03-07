﻿using AutoMapper;
using FluentResults;
using MediatR;
using Streetcode.BLL.DTO.Team;
using Streetcode.BLL.Interfaces.Logging;
using Streetcode.DAL.Repositories.Interfaces.Base;

namespace Streetcode.BLL.MediatR.Team.TeamMembersLinks.Create;

public class CreateTeamMemberLinkHandler
    : IRequestHandler<CreateTeamMemberLinkCommand, Result<TeamMemberLinkDTO>>
{
    private readonly IMapper _mapper;
    private readonly IRepositoryWrapper _repository;
    private readonly ILoggerService _logger;

    public CreateTeamMemberLinkHandler(IMapper mapper, IRepositoryWrapper repository, ILoggerService logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<TeamMemberLinkDTO>> Handle(CreateTeamMemberLinkCommand request, CancellationToken cancellationToken)
    {
        var teamMemberLink = _mapper.Map<DAL.Entities.Team.TeamMemberLink>(request.TeamMember);

        if (teamMemberLink is null)
        {
            const string errorMsg = "Cannot convert null to team link";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        var createdTeamLink = _repository.TeamLinkRepository.Create(teamMemberLink);

        if (createdTeamLink is null)
        {
            const string errorMsg = "Cannot create team link";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        var resultIsSuccess = await _repository.SaveChangesAsync() > 0;

        if (!resultIsSuccess)
        {
            const string errorMsg = "Failed to create a team";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }

        var createdTeamLinkDto = _mapper.Map<TeamMemberLinkDTO>(createdTeamLink);

        if(createdTeamLinkDto != null)
        {
            return Result.Ok(createdTeamLinkDto);
        }
        else
        {
            const string errorMsg = "Failed to map created team link";
            _logger.LogError(request, errorMsg);

            return Result.Fail(new Error(errorMsg));
        }
    }
}
