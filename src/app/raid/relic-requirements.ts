import { Injectable } from '@angular/core';

export interface RelicLevel {
  level: number;
  fragmentedSignalData: number;
  incompleteSignalData: number;
  flawedSignalData: number;
  corruptedSignalData: number;
  carboniteCircuitBoard: number;
  bronziumWiring: number;
  chromiumTransistor: number;
  aurodiumHeatsink: number;
  electriumConductor: number;
  zinbiddleCard: number;
  impulseDetector: number;
  aeromagnifier: number;
  gyrdaKeypad: number;
  droidBrain: number;
  coaxialServomotors: number;
}

const materialKeys = [
  'fragmentedSignalData',
  'incompleteSignalData',
  'flawedSignalData',
  'corruptedSignalData',
  'carboniteCircuitBoard',
  'bronziumWiring',
  'chromiumTransistor',
  'aurodiumHeatsink',
  'electriumConductor',
  'zinbiddleCard',
  'impulseDetector',
  'aeromagnifier',
  'gyrdaKeypad',
  'droidBrain',
  'coaxialServomotors',
] as const satisfies ReadonlyArray<keyof Omit<RelicLevel, 'level'>>;

const relicLevels: RelicLevel[] = [
  {
    level: 1,
    fragmentedSignalData: 0,
    incompleteSignalData: 0,
    flawedSignalData: 0,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 40,
    bronziumWiring: 0,
    chromiumTransistor: 0,
    aurodiumHeatsink: 0,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 2,
    fragmentedSignalData: 15,
    incompleteSignalData: 0,
    flawedSignalData: 0,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 30,
    bronziumWiring: 40,
    chromiumTransistor: 0,
    aurodiumHeatsink: 0,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 3,
    fragmentedSignalData: 20,
    incompleteSignalData: 15,
    flawedSignalData: 0,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 30,
    bronziumWiring: 40,
    chromiumTransistor: 20,
    aurodiumHeatsink: 0,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 4,
    fragmentedSignalData: 20,
    incompleteSignalData: 25,
    flawedSignalData: 0,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 30,
    bronziumWiring: 40,
    chromiumTransistor: 40,
    aurodiumHeatsink: 0,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 5,
    fragmentedSignalData: 20,
    incompleteSignalData: 25,
    flawedSignalData: 15,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 30,
    bronziumWiring: 40,
    chromiumTransistor: 30,
    aurodiumHeatsink: 20,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 6,
    fragmentedSignalData: 20,
    incompleteSignalData: 25,
    flawedSignalData: 25,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 20,
    bronziumWiring: 30,
    chromiumTransistor: 30,
    aurodiumHeatsink: 20,
    electriumConductor: 20,
    zinbiddleCard: 0,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 7,
    fragmentedSignalData: 20,
    incompleteSignalData: 25,
    flawedSignalData: 35,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 20,
    bronziumWiring: 30,
    chromiumTransistor: 20,
    aurodiumHeatsink: 20,
    electriumConductor: 20,
    zinbiddleCard: 10,
    impulseDetector: 0,
    aeromagnifier: 0,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 8,
    fragmentedSignalData: 20,
    incompleteSignalData: 25,
    flawedSignalData: 45,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 0,
    bronziumWiring: 0,
    chromiumTransistor: 20,
    aurodiumHeatsink: 20,
    electriumConductor: 20,
    zinbiddleCard: 20,
    impulseDetector: 20,
    aeromagnifier: 20,
    gyrdaKeypad: 0,
    droidBrain: 0,
    coaxialServomotors: 0,
  },
  {
    level: 9,
    fragmentedSignalData: 0,
    incompleteSignalData: 30,
    flawedSignalData: 55,
    corruptedSignalData: 0,
    carboniteCircuitBoard: 0,
    bronziumWiring: 0,
    chromiumTransistor: 0,
    aurodiumHeatsink: 0,
    electriumConductor: 20,
    zinbiddleCard: 20,
    impulseDetector: 20,
    aeromagnifier: 20,
    gyrdaKeypad: 20,
    droidBrain: 20,
    coaxialServomotors: 0,
  },
  {
    level: 10,
    fragmentedSignalData: 0,
    incompleteSignalData: 25,
    flawedSignalData: 45,
    corruptedSignalData: 15,
    carboniteCircuitBoard: 0,
    bronziumWiring: 0,
    chromiumTransistor: 0,
    aurodiumHeatsink: 0,
    electriumConductor: 0,
    zinbiddleCard: 0,
    impulseDetector: 20,
    aeromagnifier: 20,
    gyrdaKeypad: 20,
    droidBrain: 20,
    coaxialServomotors: 20,
  },
];

@Injectable({
  providedIn: 'root',
})
export class RelicRequirements {
  getRequirements(startingLevel: number,
    targetLevel: number): RelicLevel {
    const maxLevel = relicLevels[relicLevels.length - 1]?.level ?? 0;

    if (startingLevel < 0 || startingLevel > maxLevel) {
      throw new RangeError(`startingLevel must be between 0 and ${maxLevel}.`);
    }

    if (targetLevel < 0 || targetLevel > maxLevel) {
      throw new RangeError(`targetLevel must be between 0 and ${maxLevel}.`);
    }

    if (targetLevel < startingLevel) {
      throw new RangeError('targetLevel must be greater than or equal to startingLevel.');
    }

    const requirements: RelicLevel = {
      level: targetLevel,
      fragmentedSignalData: 0,
      incompleteSignalData: 0,
      flawedSignalData: 0,
      corruptedSignalData: 0,
      carboniteCircuitBoard: 0,
      bronziumWiring: 0,
      chromiumTransistor: 0,
      aurodiumHeatsink: 0,
      electriumConductor: 0,
      zinbiddleCard: 0,
      impulseDetector: 0,
      aeromagnifier: 0,
      gyrdaKeypad: 0,
      droidBrain: 0,
      coaxialServomotors: 0,
    };

    for (const relicLevel of relicLevels) {
      if (relicLevel.level <= startingLevel || relicLevel.level > targetLevel) {
        continue;
      }

      for (const materialKey of materialKeys) {
        requirements[materialKey] += relicLevel[materialKey];
      }
    }

    return requirements;
  }
}
