import { Pipe, PipeTransform } from '@angular/core';
import { EarnableLocation } from '../apiModels/earnable-location';

@Pipe({
  name: 'earnableLocations',
})
export class EarnableLocationsPipe implements PipeTransform {
  private static readonly locationNames = new Map<string, string>([
    ['MarqueeEvent', 'Marquee Event'],
    ['CrystalShipments', 'Crystal Shipments'],
    ['DarkSide', 'Dark Side'],
    ['LightSide', 'Light Side'],
    ['Cantina', 'Cantina'],
    ['Fleet', 'Fleet'],
    ['CantinaShipments', 'Cantina Shipments'],
    ['GuildTokenShipments', 'Guild Token Shipments'],
    ['RaidMark1Shipments', 'Raid Mark 1 Shipments'],
    ['RaidMark2Shipments', 'Raid Mark 2 Shipments'],
    ['SquadArenaShipments', 'Squad Arena Shipments'],
    ['GalacticWarShipments', 'Galactic War Shipments'],
    ['FleetArenaShipments', 'Fleet Arena Shipments'],
    ['GuildEventMark1Shipments', 'Guild Event Mark 1 Shipments'],
    ['GuildEventMark2Shipments', 'Guild Event Mark 2 Shipments'],
    ['GuildEventMark3Shipments', 'Guild Event Mark 3 Shipments'],
    ['ShardShopCurrency', 'Shard Shop Currency'],
    ['LegendTokens', 'Legend Tokens'],
    ['ConquestMainReward', 'Conquest Main Reward'],
    ['ConquestSecondaryReward', 'Conquest Secondary Reward'],
    ['ConquestShipments', 'Conquest Shipments'],
    ['ProvingGrounds', 'Proving Grounds'],
    ['JourneyGuide', 'Journey Guide'],
  ]);

  public transform(locations: ReadonlyArray<EarnableLocation> | null | undefined): string {
    if (!locations || locations.length === 0) {
      return 'None';
    }

    return locations
      .map((location) => EarnableLocationsPipe.locationNames.get(String(location)) ?? 'Unknown')
      .sort((left, right) => left.localeCompare(right, 'en-US'))
      .join(', ');
  }
}
