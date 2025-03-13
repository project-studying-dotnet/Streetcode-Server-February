using Streetcode.BLL.DTO.Media.Audio;
using Streetcode.BLL.MediatR.Media.Audio.Create;
using Streetcode.BLL.MediatR.Media.Audio.Delete;
using Streetcode.BLL.MediatR.Media.Audio.GetAll;
using Streetcode.BLL.MediatR.Media.Audio.GetBaseAudio;
using Streetcode.BLL.MediatR.Media.Audio.GetById;
using Streetcode.BLL.MediatR.Media.Audio.GetByStreetcodeId;
using MediatR;

namespace Streetcode.WebApi.Extensions;

public static class MinimalApiExtensions
{
    public static WebApplication RegisterMinimalApis(this WebApplication app)
    {
        var audioGroup = app.MapGroup("/api/minimal/audio");

        audioGroup.MapGet("/", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAllAudiosQuery())));

        audioGroup.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAudioByIdQuery(id))));

        audioGroup.MapGet("/streetcode/{streetcodeId:int}", async (int streetcodeId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetAudioByStreetcodeIdQuery(streetcodeId))));

        audioGroup.MapGet("/base/{id:int}", async (int id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetBaseAudioQuery(id))));

        audioGroup.MapPost("/", async (AudioFileBaseCreateDTO audio, IMediator mediator) =>
            Results.Ok(await mediator.Send(new CreateAudioCommand(audio))));

        audioGroup.MapDelete("/{id:int}", async (int id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new DeleteAudioCommand(id))));

        return app;
    }
}
