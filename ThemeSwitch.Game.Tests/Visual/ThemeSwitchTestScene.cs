using osu.Framework.Testing;

namespace ThemeSwitch.Game.Tests.Visual
{
    public abstract partial class ThemeSwitchTestScene : TestScene
    {
        protected override ITestSceneTestRunner CreateRunner() => new ThemeSwitchTestSceneTestRunner();

        private partial class ThemeSwitchTestSceneTestRunner : ThemeSwitchGameBase, ITestSceneTestRunner
        {
            private TestSceneTestRunner.TestRunner runner;

            protected override void LoadAsyncComplete()
            {
                base.LoadAsyncComplete();
                Add(runner = new TestSceneTestRunner.TestRunner());
            }

            public void RunTestBlocking(TestScene test) => runner.RunTestBlocking(test);
        }
    }
}
