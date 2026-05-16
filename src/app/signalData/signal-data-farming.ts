import { Injectable } from '@angular/core';

export interface FarmingPlan {
  nodesToFarm: string[];
  days: number;
  sims: number;
  energySpent: number;
}

const Fragemented8C = 1.41;
const Incomplete8F = 0.91;
const Flawed8G = 0.61;

const Fragmented9B = 1.2845;
const Fragmented9D = 1.3171;
const Incomplete9B = 0.5522;
const Incomplete9F = 0.8546;
const Flawed9D = 0.3689;
const Flawed9F = 0.4984;

const Tier8SimsPerDay = 32;
const Tier9SimsPerDay = 26;

const Tier8Energy = 16;
const Tier9Energy = 20;

@Injectable({
  providedIn: 'root',
})
export class SignalDataFarming {
  getRemainingDays(neededFragmented: number,
    neededIncomplete: number,
    neededFlawed: number): FarmingPlan {
    const options = [
      this.calculateTier8(neededFragmented,
        neededIncomplete,
        neededFlawed
      ),
      this.calculate9F(neededFragmented,
        neededIncomplete,
        neededFlawed
      ),
      this.calculate9D(neededFragmented,
        neededIncomplete,
        neededFlawed
      ),
      this.calculate9B(neededFragmented,
        neededIncomplete,
        neededFlawed
      )
    ]

    const quickestOption = options.reduce((lowest, current) => {
      if (current.sims < lowest.sims) {
        return current;
      }
      if (lowest.sims < current.sims) {
        return lowest;
      }

      if (current.energySpent < lowest.energySpent) {
        return current;
      }
      if (lowest.energySpent < current.energySpent) {
        return lowest;
      }

      return lowest;
    });

    return quickestOption;
  }

  private calculateTier8(neededFragmented: number,
    neededIncomplete: number,
    neededFlawed: number): FarmingPlan {
    const fragmentedSims = Math.ceil(neededFragmented / Fragemented8C);
    const incompleteSims = Math.ceil(neededIncomplete / Incomplete8F);
    const flawedSims = Math.ceil(neededFlawed / Flawed8G);

    const sims = fragmentedSims + incompleteSims + flawedSims;
    const days = sims / Tier8SimsPerDay;

    return {
      days: Math.ceil(days),
      sims,
      nodesToFarm: ['8-G', '8-F', '8-C'],
      energySpent: sims * Tier8Energy
    };
  }

  private calculate9F(neededFragmented: number,
    neededIncomplete: number,
    neededFlawed: number): FarmingPlan {
    const flawed9FSims = Math.ceil(neededFlawed / Flawed9F);
    const incomplete9FSims = Math.ceil(neededIncomplete / Incomplete9F);

    if (flawed9FSims > incomplete9FSims) {
      // Need more Flawed than Incomplete -> farm 9-D next
      const flawedFrom9F = Math.floor(neededFlawed - (incomplete9FSims * Flawed9F));
      const flawed9DSims = Math.ceil(flawedFrom9F / Flawed9D);
      const fragemnted9DSims = Math.ceil(neededFragmented / Fragmented9D);

      if (flawed9DSims > fragemnted9DSims) {
        // Need more Flawed than Fragemented -> farm 8-G last
        const flawedFrom9FAnd9D = Math.floor(flawedFrom9F - (fragemnted9DSims * Flawed9D));
        const flawed8GSims = Math.ceil(flawedFrom9FAnd9D / Flawed8G);

        const tier9Sims = incomplete9FSims + fragemnted9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = flawed8GSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + flawed8GSims,
          nodesToFarm: ['9-F', '9-D', '8-G'],
          energySpent: tier9Sims * Tier9Energy + flawed8GSims * Tier8Energy
        };
      } else if (fragemnted9DSims > flawed9DSims) {
        // Need more Fragemented than Flawed -> farm 8-C last
        const fragmentedFrom9D = Math.floor(neededFragmented - (flawed9DSims * Fragmented9D));
        const fragemented8CSims = Math.ceil(fragmentedFrom9D / Fragemented8C);

        const tier9Sims = incomplete9FSims + flawed9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = fragemented8CSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + fragemented8CSims,
          nodesToFarm: ['9-F', '9-D', '8-C'],
          energySpent: tier9Sims * Tier9Energy + fragemented8CSims * Tier8Energy
        };
      } else {
        // Needed equal amounts of Fragemented and Flawed, all done
        const tier9Sims = incomplete9FSims + flawed9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-F', '9-D'],
          energySpent: tier9Days * Tier9Energy
        };
      }
    } else if (incomplete9FSims > flawed9FSims) {
      // Need more Incomplete than Flawed -> farm 9-B next
      const incompleteFrom9F = Math.floor(neededIncomplete - (flawed9FSims * Incomplete9F));
      const incomplete9BSims = Math.ceil(incompleteFrom9F / Incomplete9B);
      const fragemented9BSims = Math.ceil(neededFragmented / Fragmented9B);

      if (incomplete9BSims > fragemented9BSims) {
        // Need more Incomplete than Fragmented -> farm 8-F last
        const incompleteFrom9FAnd9B = Math.floor(incompleteFrom9F - (fragemented9BSims * Incomplete9B));
        const incomplete8FSims = Math.ceil(incompleteFrom9FAnd9B / Incomplete8F);

        const tier9Sims = flawed9FSims + fragemented9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = incomplete8FSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + incomplete8FSims,
          nodesToFarm: ['9-F', '9-B', '8-F'],
          energySpent: tier9Days * Tier9Energy + incomplete8FSims * Tier8Energy
        };
      } else if (fragemented9BSims > incomplete9BSims) {
        // Need more Fragmented than Incomplete -> farm 8-C last
        const fragmentedFrom9B = Math.floor(neededFragmented - (incomplete9BSims * Fragmented9B));
        const fragmented8CSims = Math.ceil(fragmentedFrom9B / Fragemented8C);

        const tier9Sims = flawed9FSims + incomplete9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = fragmented8CSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + fragmented8CSims,
          nodesToFarm: ['9-F', '9-B', '8-C'],
          energySpent: tier9Sims * Tier9Energy + fragmented8CSims * Tier8Energy
        };
      } else {
        // Needed equal amounts of Fragmented and Incomplete, all done
        const tier9Sims = flawed9FSims + incomplete9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-F', '9-B'],
          energySpent: tier9Sims * Tier9Energy
        };
      }
    } else {
      // Needed equal amounts of Flawed and Incomplete -> farm 8-C last
      const fragmented8CSims = Math.ceil(neededFragmented / Fragemented8C);

      const tier9Days = flawed9FSims / Tier9SimsPerDay;
      const tier8Days = fragmented8CSims / Tier8SimsPerDay;

      return {
        days: Math.ceil(tier9Days + tier8Days),
        sims: flawed9FSims + fragmented8CSims,
        nodesToFarm: ['9-F', '9-B'],
        energySpent: flawed9FSims * Tier9Energy + fragmented8CSims * Tier8Energy
      };
    }
  }

  private calculate9D(neededFragmented: number,
    neededIncomplete: number,
    neededFlawed: number): FarmingPlan {
    const flawed9DSims = Math.ceil(neededFlawed / Flawed9D);
    const fragmented9DSims = Math.ceil(neededFragmented / Fragmented9D);

    if (flawed9DSims > fragmented9DSims) {
      // Need more Flawed than Fragmented -> farm 9-F next
      const flawedFrom9D = Math.floor(neededFlawed - (fragmented9DSims * Flawed9D));
      const flawed9FSims = Math.ceil(flawedFrom9D / Flawed9D);
      const incomplete9FSims = Math.ceil(neededIncomplete / Incomplete9F);

      if (flawed9FSims > incomplete9FSims) {
        // Need more Flawed than Incomplete -> farm 8-G last
        const flawedFrom9DAnd9F = Math.floor(flawedFrom9D - (incomplete9FSims * Flawed9F));
        const flawed8GSims = Math.ceil(flawedFrom9DAnd9F / Flawed8G);

        const tier9Sims = fragmented9DSims + incomplete9FSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = flawed8GSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + flawed8GSims,
          nodesToFarm: ['9-D', '9-F', '8-G'],
          energySpent: tier9Sims * Tier9Energy + flawed8GSims * Tier8Energy
        };
      } else if (incomplete9FSims > flawed9FSims) {
        // Need more Incomplete than Flawed -> farm 8-F last
        const incompleteFrom9F = Math.floor(neededIncomplete - (flawed9FSims * Incomplete9F));
        const incomplete8FSims = Math.ceil(incompleteFrom9F / Incomplete8F);

        const tier9Sims = fragmented9DSims + flawed9FSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = incomplete8FSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + incomplete8FSims,
          nodesToFarm: ['9-D', '9-F', '8-F'],
          energySpent: tier9Sims * Tier9Energy + incomplete8FSims * Tier8Energy
        };
      } else {
        // Needed equal amounts of Incomplete and Flawed, all done
        const tier9Sims = fragmented9DSims + flawed9FSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;

        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-D', '9-F'],
          energySpent: tier9Days * Tier9Energy
        };
      }
    } else if (fragmented9DSims > flawed9DSims) {
      // Need more Fragmented than Flawed -> Farm 9-B next
      const fragmentedFrom9D = Math.floor(neededFragmented - (flawed9DSims * Fragmented9D));
      const fragmented9BSims = Math.ceil(fragmentedFrom9D / Fragmented9B);
      const incomplete9BSims = Math.ceil(neededIncomplete / Incomplete9B);

      if (fragmented9BSims > incomplete9BSims) {
        // Need more Fragmented than Incomplete -> Farm 8-C last
        const fragmentedFrom9DAnd9B = Math.floor(fragmentedFrom9D - (incomplete9BSims * Fragmented9B));
        const fragmented8CSims = Math.ceil(fragmentedFrom9DAnd9B / Fragemented8C);

        const tier9Sims = flawed9DSims + incomplete9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = fragmented8CSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + fragmented8CSims,
          nodesToFarm: ['9-D', '9-B', '8-C'],
          energySpent: tier9Sims * Tier9Energy + fragmented8CSims * Tier8Energy
        };
      } else if (incomplete9BSims > fragmented9BSims) {
        // Need more Incomplete than Fragmented -> Farm 8-F last
        const incompleteFrom9B = Math.floor(neededIncomplete - (fragmented9BSims * Incomplete9B));
        const incomplete8FSims = Math.ceil(incompleteFrom9B / Incomplete8F);

        const tier9Sims = flawed9DSims + fragmented9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = incomplete8FSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + incomplete8FSims,
          nodesToFarm: ['9-D', '9-B', '8-F'],
          energySpent: tier9Sims * Tier9Energy + incomplete8FSims * Tier8Energy
        };
      } else {
        // Needed equal amounts of Incomplete and Fragmented -> all done
        const tier9Sims = flawed9DSims + fragmented9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;

        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-D', '9-B'],
          energySpent: tier9Sims * Tier9Energy
        };
      }
    } else {
      // Needed equal amounts of Flawed and Fragmented -> farm 8-F last
      const incomplete8FSims = Math.ceil(neededIncomplete / Incomplete8F);

      const tier9Days = flawed9DSims / Tier9SimsPerDay;
      const tier8Days = incomplete8FSims / Tier8SimsPerDay;

      return {
        days: Math.ceil(tier9Days + tier8Days),
        sims: flawed9DSims + incomplete8FSims,
        nodesToFarm: ['9-D', '8-F'],
        energySpent: flawed9DSims * Tier9Energy + incomplete8FSims * Tier8Energy
      };
    }
  }

  private calculate9B(neededFragmented: number,
    neededIncomplete: number,
    neededFlawed: number): FarmingPlan {
    const incomplete9BSims = Math.ceil(neededIncomplete / Incomplete9B);
    const fragmented9BSims = Math.ceil(neededFragmented / Fragmented9B);

    if (incomplete9BSims > fragmented9BSims) {
      // Need more Incomplete than Fragmented -> farm 9-F next
      const incompleteFrom9B = Math.floor(neededIncomplete - (fragmented9BSims * Incomplete9B));
      const incomplete9FSims = Math.ceil(incompleteFrom9B / Incomplete9F);
      const flawed9FSims = Math.ceil(neededFlawed / Flawed9F);

      if (incomplete9FSims > flawed9FSims) {
        // Need more Incomplete than Flawed -> farm 8-F last
        const incompleteFrom9BAnd9F = Math.floor(incompleteFrom9B - (flawed9FSims * Incomplete9F));
        const incomplete8FSims = Math.ceil(incompleteFrom9BAnd9F / Incomplete8F);

        const tier9Sims = fragmented9BSims + flawed9FSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = incomplete8FSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + incomplete8FSims,
          nodesToFarm: ['9-B', '9-F', '8-F'],
          energySpent: tier9Sims * Tier9Energy + incomplete8FSims * Tier8Energy
        };
      } else if (flawed9FSims > incomplete9BSims) {
        // Need more Flawed than Incomplete -> farm 8-G last
        const flawedFrom99F = Math.floor(neededFlawed - (incomplete9FSims * Flawed9F));
        const flawed8GSims = Math.ceil(flawedFrom99F / Flawed8G);

        const tier9Sims = fragmented9BSims + incomplete9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = flawed8GSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + flawed8GSims,
          nodesToFarm: ['9-B', '9-F', '8-G'],
          energySpent: tier9Sims * Tier9Energy + flawed8GSims * Tier8Energy
        };
      } else {
        // Needed equal amounts Flawed and Incomplete -> done
        const tier9Sims = fragmented9BSims + incomplete9BSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;

        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-B', '9-F'],
          energySpent: tier9Sims * Tier9Energy
        };
      }
    } else if (fragmented9BSims > incomplete9BSims) {
      // Need more Fragmented than Incomplete -> farm 9-D next
      const fragmentedFrom9B = Math.floor(neededFragmented - (incomplete9BSims * Fragmented9B));
      const fragmented9DSims = Math.ceil(fragmentedFrom9B / Fragmented9D);
      const flawed9DSims = Math.ceil(neededFlawed / Flawed9D);

      if (fragmented9DSims > flawed9DSims) {
        // Need more Fragmented than Flawed -> farm 8-C last
        const fragmentedFrom9BAnd9D = Math.floor(fragmentedFrom9B - (flawed9DSims * Fragmented9B));
        const fragmented8CSims = Math.ceil(fragmentedFrom9BAnd9D / Fragemented8C);

        const tier9Sims = incomplete9BSims + flawed9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = fragmented8CSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + fragmented8CSims,
          nodesToFarm: ['9-B', '9-D', '8-C'],
          energySpent: tier9Sims * Tier9Energy + fragmented8CSims * Tier8Energy
        };
      } else if (flawed9DSims > fragmented9BSims) {
        // Need more Flawed than Fragmented -> farm 8-G last
        const flawedFrom9D = Math.floor(neededFlawed - (fragmented9DSims * Flawed9D));
        const flawed8GSims = Math.ceil(flawedFrom9D / Flawed8G);

        const tier9Sims = incomplete9BSims + fragmented9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;
        const tier8Days = flawed8GSims / Tier8SimsPerDay;

        return {
          days: Math.ceil(tier9Days + tier8Days),
          sims: tier9Sims + flawed8GSims,
          nodesToFarm: ['9-B', '9-D', '8-G'],
          energySpent: tier9Sims * Tier9Energy + flawed8GSims * Tier8Energy
        };
      } else {
        // Needed equal amounts Flawed and Fragmented -> all done
        const tier9Sims = incomplete9BSims + flawed9DSims;
        const tier9Days = tier9Sims / Tier9SimsPerDay;

        return {
          days: Math.ceil(tier9Days),
          sims: tier9Sims,
          nodesToFarm: ['9-B', '9-D'],
          energySpent: tier9Sims * Tier9Energy
        };
      }
    } else {
      // Needed equal amounts Fragmented and Incomplete -> farm 8-G last
      const flawed8GSims = Math.ceil(neededFlawed / Flawed8G);

      const tier9Days = incomplete9BSims / Tier9SimsPerDay;
      const tier8Days = flawed8GSims / Tier8SimsPerDay;

      return {
        days: Math.ceil(tier9Days + tier8Days),
        sims: incomplete9BSims + flawed8GSims,
        nodesToFarm: ['9-B', '8-G'],
        energySpent: incomplete9BSims * Tier9Energy + flawed8GSims * Tier8Energy
      };
    }
  }
}
