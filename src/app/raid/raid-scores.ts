import { Injectable } from '@angular/core';

export interface RaidTier {
  tier: number;
  score: number;
  relicLevel: number;
}

export interface RaidTeamPlan {
  targetScore: number;
  achievedScore: number;
  totalTeams: number;
  tier0Teams: number;
  tier1Teams: number;
  tier2Teams: number;
  tier3Teams: number;
  tier4Teams: number;
  tier5Teams: number;
  tier6Teams: number;
  tier7Teams: number;
}

const raidTiers: RaidTier[] = [
  { tier: 0, score: 300000, relicLevel: 0 },
  { tier: 1, score: 450000, relicLevel: 0 },
  { tier: 2, score: 600000, relicLevel: 1 },
  { tier: 3, score: 900000, relicLevel: 3 },
  { tier: 4, score: 1200000, relicLevel: 5 },
  { tier: 5, score: 1800000, relicLevel: 7 },
  { tier: 6, score: 2700000, relicLevel: 8 },
  { tier: 7, score: 3600000, relicLevel: 9 },
];

@Injectable({
  providedIn: 'root',
})
export class RaidScores {
  getRaidTiers(): ReadonlyArray<RaidTier> {
    return raidTiers.map((tier) => ({ ...tier }));
  }

  calculateScore(tier0Teams?: number,
    tier1Teams?: number,
    tier2Teams?: number,
    tier3Teams?: number,
    tier4Teams?: number,
    tier5Teams?: number,
    tier6Teams?: number,
    tier7Teams?: number
  ): number {
    const teamCounts = [
      tier0Teams,
      tier1Teams,
      tier2Teams,
      tier3Teams,
      tier4Teams,
      tier5Teams,
      tier6Teams,
      tier7Teams,
    ].map((teamCount, index) => {
      if (teamCount === undefined) {
        return 0;
      }

      if (!Number.isFinite(teamCount)) {
        throw new TypeError(`tier${index}Teams must be a positive number.`);
      }

      if (teamCount <= 0) {
        throw new RangeError(`tier${index}Teams must be a positive number.`);
      }

      return teamCount;
    });

    const totalTeams = teamCounts.reduce((total, count) => total + count, 0);
    if (totalTeams < 1 || totalTeams > 5) {
      throw new RangeError('The total number of teams must be between 1 and 5.');
    }

    return teamCounts.reduce((totalScore, teamCount, tier) => {
      const scorePerTeam = raidTiers[tier]?.score ?? 0;
      return totalScore + (teamCount * scorePerTeam);
    }, 0);
  }

  calculateTeamsForTarget(targetScore: number): RaidTeamPlan {
    if (!Number.isFinite(targetScore)) {
      throw new TypeError('targetScore must be a finite number.');
    }

    if (targetScore <= 0) {
      throw new RangeError('targetScore must be greater than 0.');
    }

    const maxTierScore = raidTiers[raidTiers.length - 1]?.score ?? 0;
    const maxScore = maxTierScore * 5;
    if (targetScore > maxScore) {
      throw new RangeError(`targetScore must be less than or equal to ${maxScore}.`);
    }

    const scoreByTier = raidTiers.map((tier) => tier.score);
    const findBestCandidate = (
      tierIndex: number,
      teamsUsed: number,
      runningScore: number,
      counts: number[],
    ): { counts: number[]; score: number } | null => {
      if (tierIndex === scoreByTier.length) {
        if (teamsUsed < 1 || teamsUsed > 5 || runningScore < targetScore) {
          return null;
        }

        return { counts: [...counts], score: runningScore };
      }

      let bestCandidate: { counts: number[]; score: number } | null = null;
      const teamsRemaining = 5 - teamsUsed;
      for (let teamCount = 0; teamCount <= teamsRemaining; teamCount += 1) {
        counts[tierIndex] = teamCount;
        const candidate = findBestCandidate(
          tierIndex + 1,
          teamsUsed + teamCount,
          runningScore + (teamCount * scoreByTier[tierIndex]),
          counts,
        );

        if (
          candidate !== null &&
          (
            bestCandidate === null ||
            candidate.score < bestCandidate.score ||
            (candidate.score === bestCandidate.score
              && this.prefersLowerTiers(candidate.counts, bestCandidate.counts))
          )
        ) {
          bestCandidate = candidate;
        }
      }

      return bestCandidate;
    };

    const candidate = findBestCandidate(0, 0, 0, new Array<number>(raidTiers.length).fill(0));
    if (candidate === null) {
      throw new RangeError('Unable to find a team composition for the provided targetScore.');
    }

    return {
      targetScore,
      achievedScore: candidate.score,
      totalTeams: candidate.counts.reduce((total: number, count: number) => total + count, 0),
      tier0Teams: candidate.counts[0],
      tier1Teams: candidate.counts[1],
      tier2Teams: candidate.counts[2],
      tier3Teams: candidate.counts[3],
      tier4Teams: candidate.counts[4],
      tier5Teams: candidate.counts[5],
      tier6Teams: candidate.counts[6],
      tier7Teams: candidate.counts[7],
    };
  }

  private prefersLowerTiers(candidateCounts: number[], currentBestCounts: number[]): boolean {
    for (let index = 0; index < candidateCounts.length; index += 1) {
      if (candidateCounts[index] !== currentBestCounts[index]) {
        return candidateCounts[index] > currentBestCounts[index];
      }
    }

    return false;
  }
}
