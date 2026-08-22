using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using ConquestRewardPhase = SwgohApi.Infrastructure.Models.ConquestRewardPhase;
using Earnable = SwgohApi.Infrastructure.Models.Earnable;
using EarnableLocation = SwgohApi.Infrastructure.Models.EarnableLocation;
using InternalConquestReward = SwgohApi.Infrastructure.Models.ConquestReward;

namespace SwgohApi.Endpoints;

public static class ConquestRewardEndpoints
{
  public static WebApplication MapConquestRewardEndpoints(this WebApplication app)
  {
    var conquestRewards = app.MapGroup("/api/conquestRewards");

    conquestRewards.MapGet(string.Empty, GetConquestRewards)
      .AllowAnonymous();

    return app;
  }

  public static async Task<Ok<IEnumerable<ConquestRewardDate>>> GetConquestRewards(
    IConquestRewardRepository conquestRewardRepository,
    IMapper<InternalConquestReward, ConquestRewardDate> conquestRewardMapper)
  {
    var conquestRewards = await conquestRewardRepository.GetConquestRewards();

    return TypedResults.Ok(conquestRewards.Select(conquestRewardMapper.MapTo));
  }

  public static async Task<Results<Ok<CreateConquestRewardResponse>, ProblemHttpResult>> CreateConquestReward(
    CreateConquestRewardRequest request,
    IConquestRewardRepository conquestRewardRepository,
    ICharacterRepository characterRepository,
    IShipRepository shipRepository,
    IMapper<InternalConquestReward, ConquestRewardDate> conquestRewardMapper)
  {
    var conquestRewards = (await conquestRewardRepository.GetConquestRewards())
      .ToArray();

    // The next reward should in Proving Grounds should move there.
    var newProvingGroundsReward = conquestRewards.Where(cr => cr.RewardPhase is ConquestRewardPhase.ConquestShipments)
      .OrderBy(cr => cr.ProvingGroundsDate)
      .FirstOrDefault();
    if (newProvingGroundsReward is null)
    {
      return TypedResults.Problem("Could not Conquest Shipment reward to move to Proving Grounds",
        statusCode: (int)HttpStatusCode.BadRequest);
    }
    newProvingGroundsReward.RewardPhase = ConquestRewardPhase.ProvingGrounds;
    await conquestRewardRepository.SaveConquestReward(newProvingGroundsReward);
    await UpdateLocation(newProvingGroundsReward,
      EarnableLocation.ProvingGrounds,
      characterRepository,
      shipRepository);

    // The current Secondary Reward should be moved to Conquest Shipments.
    var newConquestShipmentReward =
      conquestRewards.FirstOrDefault(cr => cr.RewardPhase is ConquestRewardPhase.SecondaryReward);
    if (newConquestShipmentReward is null)
    {
      return TypedResults.Problem("Could not find Secondary Reward Conquest Reward",
        statusCode: (int)HttpStatusCode.BadRequest);
    }
    newConquestShipmentReward.RewardPhase = ConquestRewardPhase.ConquestShipments;
    await conquestRewardRepository.SaveConquestReward(newConquestShipmentReward);
    await UpdateLocation(newConquestShipmentReward,
      EarnableLocation.ConquestShipments,
      characterRepository,
      shipRepository);

    // The current Main Reward should become the secondary reward.
    var newSecondaryReward = conquestRewards.FirstOrDefault(cr => cr.RewardPhase is ConquestRewardPhase.SecondaryReward);
    if (newSecondaryReward is null)
    {
      return TypedResults.Problem("Could not find Main Reward Conquest Reward",
        statusCode: (int)HttpStatusCode.BadRequest);
    }
    newSecondaryReward.RewardPhase = ConquestRewardPhase.SecondaryReward;
    await conquestRewardRepository.SaveConquestReward(newSecondaryReward);
    await UpdateLocation(newSecondaryReward,
      EarnableLocation.ConquestSecondaryReward,
      characterRepository,
      shipRepository);

    // Add the new Main Reward
    Earnable newMainReward;
    if (request.NewRewardIsCharacter)
    {
      newMainReward = await characterRepository.CreateCharacter(request.Name,
        [EarnableLocation.ConquestMainReward],
        false);
    }
    else
    {
      newMainReward = await shipRepository.CreateShip(request.Name,
        [EarnableLocation.ConquestMainReward]);
    }

    var initialUnlockDate = request.FirstConquestStartDate.AddDays(10 * 7);
    var finalRewardCrateDate = initialUnlockDate.AddDays(12 * 7);
    var provingGroundsDate = initialUnlockDate.AddDays(40 * 7);
    newMainReward.ConquestReward = await conquestRewardRepository.CreateConquestReward(newMainReward,
      ConquestRewardPhase.MainReward,
      initialUnlockDate,
      finalRewardCrateDate,
      provingGroundsDate);

    var response = new CreateConquestRewardResponse(
      conquestRewardMapper.MapTo(newMainReward.ConquestReward),
      conquestRewardMapper.MapTo(newSecondaryReward),
      conquestRewardMapper.MapTo(newConquestShipmentReward),
      conquestRewardMapper.MapTo(newProvingGroundsReward));
    return TypedResults.Ok(response);
  }

  private static async Task UpdateLocation(InternalConquestReward conquestReward,
    EarnableLocation newLocation,
    ICharacterRepository characterRepository,
    IShipRepository shipRepository)
  {
    if (conquestReward.Character is not null)
    {
      conquestReward.Character.Locations = [newLocation];
      await characterRepository.SaveEarnable(conquestReward.Character);
    }
    else if (conquestReward.Ship is not null)
    {
      conquestReward.Ship.Locations = [newLocation];
      await shipRepository.SaveEarnable(conquestReward.Ship);
    }
  }
}
