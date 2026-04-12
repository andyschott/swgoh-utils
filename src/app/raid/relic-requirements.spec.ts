import { TestBed } from '@angular/core/testing';

import { RelicLevel, RelicRequirements } from './relic-requirements';

describe('RelicRequirements', () => {
  let service: RelicRequirements;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RelicRequirements);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should sum requirements from starting level to target level', () => {
    const requirements = service.getRequirements(0, 3);
    const expected: RelicLevel = {
      level: 3,
      fragmentedSignalData: 35,
      incompleteSignalData: 15,
      flawedSignalData: 0,
      corruptedSignalData: 0,
      carboniteCircuitBoard: 100,
      bronziumWiring: 80,
      chromiumTransistor: 20,
      aurodiumHeatsink: 0,
      electriumConductor: 0,
      zinbiddleCard: 0,
      impulseDetector: 0,
      aeromagnifier: 0,
      gyrdaKeypad: 0,
      droidBrain: 0,
      coaxialServomotors: 0,
    };

    expect(requirements).toEqual(expected);
  });

  it('should return zero materials when starting and target levels are the same', () => {
    const requirements = service.getRequirements(5, 5);

    expect(requirements).toEqual({
      level: 5,
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
    });
  });

  it('should throw when starting level is below the minimum', () => {
    expect(() => service.getRequirements(-1, 1)).toThrow(RangeError);
  });

  it('should throw when target level is above the maximum', () => {
    expect(() => service.getRequirements(0, 11)).toThrow(RangeError);
  });

  it('should throw when target level is below starting level', () => {
    expect(() => service.getRequirements(4, 3)).toThrow(RangeError);
  });
});
