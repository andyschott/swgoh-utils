using AutoFixture;
using Microsoft.AspNetCore.Http.HttpResults;
using SwgohApi.Endpoints;
using SwgohApi.Infrastructure;
using SwgohApi.Mapping;
using SwgohApi.Models.Earnables;
using SwgohApi.TestUtilities.Customizations;
using InternalMarquee = SwgohApi.Infrastructure.Models.Marquee;

namespace SwgohApi.Tests.Endpoints;

public sealed class MarqueeEndpointTests : IDisposable
{
  private readonly MockRepository _mockRepository = new(MockBehavior.Strict);

  private readonly Mock<IMarqueeRepository> _mockMarqueeRepository;
  private readonly Mock<IMapper<InternalMarquee, MarqueeDate>> _mockMarqueeDateMapper;

  public MarqueeEndpointTests()
  {
    _mockMarqueeRepository = _mockRepository.Create<IMarqueeRepository>();
    _mockMarqueeDateMapper = _mockRepository.Create<IMapper<InternalMarquee, MarqueeDate>>();
  }

  public void Dispose() => _mockRepository.VerifyAll();

  [Theory, AutoDomainData]
  public async Task GetMarquees_Successful(InternalMarquee[] internalMarquees,
    MarqueeDate[] marqueeDates)
  {
    _mockMarqueeRepository.Setup(repository => repository.GetMarquees())
      .ReturnsAsync(internalMarquees);
    foreach (var (src, dest) in internalMarquees.Zip(marqueeDates))
    {
      _mockMarqueeDateMapper.Setup(mapper => mapper.MapTo(src))
        .Returns(dest);
    }

    var response = await MarqueeEndpoints.GetMarquees(
      _mockMarqueeRepository.Object,
      _mockMarqueeDateMapper.Object);

    var result = Assert.IsType<Ok<IEnumerable<MarqueeDate>>>(response);

    Assert.Equal(marqueeDates, result.Value);
  }

  class AutoDomainDataAttribute : AutoDataAttribute
  {
    public AutoDomainDataAttribute()
      : base(Customize)
    {
    }

    private static IFixture Customize()
    {
      var fixture = new Fixture();

      fixture.Customize(new MarqueeCustomization());

      return fixture;
    }
  }
}
