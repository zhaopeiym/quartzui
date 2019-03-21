import { ExplainModule } from './explain.module';

describe('ExplainModule', () => {
  let explainModule: ExplainModule;

  beforeEach(() => {
    explainModule = new ExplainModule();
  });

  it('should create an instance', () => {
    expect(explainModule).toBeTruthy();
  });
});
