import { Injectable } from '@angular/core';

export interface Stars {
  stars: number;
  shards: number;
}

const starBreakpoints = [
  10,
  25,
  50,
  80,
  145,
  230,
  330
];

@Injectable({
  providedIn: 'root',
})
export class ShardConverter {
  convertToStars(shards: number): Stars {
    for (let index = 0; index < starBreakpoints.length; index++) {
      if (shards < starBreakpoints[index]) {
        if (index === 0) {
          return {
            stars: 0,
            shards
          };
        }

        return {
          stars: index,
          shards: shards - starBreakpoints[index - 1]
        };
      }
    }

    return {
      stars: 7,
      shards: 0
    };
  }

  convertToShards(stars: Stars): number {
    return starBreakpoints[stars.stars] + stars.shards;
  }
}
