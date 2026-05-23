import { EarnableLocation } from '../apiModels/earnable-location';
import { EarnableLocationsPipe } from './earnable-locations.pipe';

describe('EarnableLocationsPipe', () => {
  const pipe = new EarnableLocationsPipe();

  it('should return None for empty locations', () => {
    expect(pipe.transform([])).toBe('None');
  });

  it('should map and sort known locations', () => {
    const locations = [
      'LightSide' as unknown as EarnableLocation,
      'CantinaShipments' as unknown as EarnableLocation,
    ];
    expect(pipe.transform(locations)).toBe('Cantina Shipments, Light Side');
  });

  it('should return Unknown for unknown locations', () => {
    expect(pipe.transform(['NotARealLocation' as unknown as EarnableLocation])).toBe('Unknown');
  });
});
