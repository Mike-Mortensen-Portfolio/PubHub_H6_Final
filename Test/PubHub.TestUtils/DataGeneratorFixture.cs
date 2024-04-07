using AutoFixture;

namespace PubHub.TestUtils
{
    public class DataGeneratorFixture
    {
        public Fixture Generator { get; private set; }

        public DataGeneratorFixture()
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
