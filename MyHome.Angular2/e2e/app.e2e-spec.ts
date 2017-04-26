import { MyHomeCorePage } from './app.po';

describe('my-home-core App', function() {
  let page: MyHomeCorePage;

  beforeEach(() => {
    page = new MyHomeCorePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
