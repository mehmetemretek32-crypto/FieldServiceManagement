using AutoMapper;
using FSM.Application.Mappings;
using Microsoft.Extensions.Logging.Abstractions;

namespace FSM.Tests.TestUtilities;

/// <summary>
/// Builds a real <see cref="IMapper"/> instance configured with the
/// application's <see cref="MappingProfile"/> so handler tests exercise the
/// same mapping behaviour as production code.
/// </summary>
public static class MapperFactory
{
    public static IMapper Create()
    {
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);

        return configuration.CreateMapper();
    }
}
