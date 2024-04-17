using AutoFixture;

namespace PubHub.TestUtils
{
    public class DataGeneratorFixture
    {
        public Fixture Generator { get; private set; } = new();

        public DataGeneratorFixture()
        {
            RebuildGenerator();
        }

        /// <summary>
        /// Build <see cref="Generator"/> with all customizations.
        /// </summary>
        /// <remarks>
        /// When overriding this, remember to call the base method first as it will renew the <see cref="Generator"/> and thus remove all customizations.
        /// </remarks>
        public virtual void RebuildGenerator()
        {
            Generator = new Fixture();

            Generator.Behaviors.Remove(new ThrowingRecursionBehavior());
            Generator.Behaviors.Add(new OmitOnRecursionBehavior());

            // Add support for otherwise unsupported types.
            Generator.Customize<DateOnly>(composer => composer.FromFactory<DateTime>(DateOnly.FromDateTime));

            // Model customizations.
            // None at the moment...
        }
    }
}
