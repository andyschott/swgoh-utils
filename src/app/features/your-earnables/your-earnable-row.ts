import { Earnable } from '../../apiModels/earnable';
import { EarnableLocation } from '../../apiModels/earnable-location';
import { FarmingStatus } from '../../apiModels/farming-status';
import { ShardConverter } from '../../earnables/shard-converter';

export interface YourEarnableRow {
  id: string;
  name: string;
  stars: number;
  currentShards: number;
  shardsRemaining: number;
  farmingStatus: string;
  locations: ReadonlyArray<EarnableLocation>;
}

interface SortableYourEarnableRow extends YourEarnableRow {
  farmingStatusOrder: number;
}

const MaximumEarnableShards = 330;
const BacklogSortOrder = 1;
const farmingStatusSortOrder = new Map<FarmingStatus, number>([
  [FarmingStatus.Active, 0],
  [FarmingStatus.Backlog, BacklogSortOrder],
  [FarmingStatus.Done, 2],
]);

export function toSortedYourEarnableRows(
  earnables: ReadonlyArray<Earnable>,
  shardConverter: ShardConverter,
): ReadonlyArray<YourEarnableRow> {
  return earnables
    .map((earnable) => toSortableRow(earnable, shardConverter))
    .sort(
      (left, right) =>
        left.farmingStatusOrder - right.farmingStatusOrder || left.name.localeCompare(right.name, 'en-US'),
    )
    .map(({ farmingStatusOrder, ...row }) => row);
}

function toSortableRow(earnable: Earnable, shardConverter: ShardConverter): SortableYourEarnableRow {
  const shards = earnable.shards?.shards ?? 0;
  const stars = shardConverter.convertToStars(shards);
  const farmingStatus = earnable.shards?.farmingStatus;

  return {
    id: earnable.id,
    name: earnable.name,
    stars: stars.stars,
    currentShards: stars.shards,
    shardsRemaining: MaximumEarnableShards - shards,
    farmingStatusOrder: getFarmingStatusOrder(farmingStatus),
    farmingStatus: formatFarmingStatus(farmingStatus),
    locations: earnable.locations,
  };
}

function getFarmingStatusOrder(farmingStatus: FarmingStatus | null | undefined): number {
  return farmingStatusSortOrder.get(farmingStatus ?? FarmingStatus.Backlog) ?? BacklogSortOrder;
}

function formatFarmingStatus(farmingStatus: FarmingStatus | null | undefined): string {
  switch (farmingStatus) {
    case FarmingStatus.Active:
      return 'Active';
    case FarmingStatus.Done:
      return 'Done';
    case FarmingStatus.Backlog:
      return 'Backlog';
    default:
      return 'Backlog';
  }
}
