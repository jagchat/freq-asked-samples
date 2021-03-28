using Microsoft.Extensions.Configuration;
using Validation;

namespace Microsoft.Diagnostics.EventFlow.Inputs
{
    public class MoreNLogInputFactory : IPipelineItemFactory<MoreNLogInput>
    {
        /// <inheritdoc/>
        public MoreNLogInput CreateItem(IConfiguration configuration, IHealthReporter healthReporter)
        {
            Requires.NotNull(configuration, nameof(configuration));
            Requires.NotNull(healthReporter, nameof(healthReporter));

            return new MoreNLogInput(healthReporter);
        }
    }
}
