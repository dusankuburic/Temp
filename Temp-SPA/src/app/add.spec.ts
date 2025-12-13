import { add } from './add';

describe('add', () => {
  it('should add two positive numbers', () => {
    expect(add(2, 3)).toBe(5);
  });

  it('should add negative and positive number', () => {
    expect(add(-2, 3)).toBe(1);
  });
});

