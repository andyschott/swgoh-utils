import { TestBed } from '@angular/core/testing';

import { ShardConverter } from './shard-converter';

describe('ShardConverter', () => {
  let service: ShardConverter;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShardConverter);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
