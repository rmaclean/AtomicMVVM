///-----------------------------------------------------------------------
/// Project: AtomicMVVM https://bitbucket.org/rmaclean/atomicmvvm
/// License: MS-PL http://www.opensource.org/licenses/MS-PL
/// Notes: These unit tests are checking for breaking changes in the API between versions.
///-----------------------------------------------------------------------

namespace AtomicMVVM
{
    using System;
    using AtomicMVVM.ViewModels;
    using Xunit;

    public class BootstrapperChecks
    {
        [Fact]
        public void BootStrapperConstructor()
        {
            var bootStrapper = new Bootstrapper();
            Assert.IsType<Bootstrapper>(bootStrapper);
            Assert.NotNull(bootStrapper.GlobalCommands);
        }

        [Fact]
        public void StarterNonGenericVersionNonDataVersion()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start(typeof(StubShell), typeof(CoreDataSample1));
            Assert.IsType<StubShell>(bootStrapper.CurrentShell);
            Assert.IsType<CoreDataSample1>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample1>(bootStrapper.CurrentView);
        }

        [Fact]
        public void StarterNonGenericVersionDataVersion()
        {
            var expected = "Hello";
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start(typeof(StubShell), typeof(CoreDataSample2), expected);
            Assert.IsType<StubShell>(bootStrapper.CurrentShell);
            Assert.IsType<CoreDataSample2>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample2>(bootStrapper.CurrentView);
            Assert.Equal(expected, (bootStrapper.CurrentViewModel as CoreDataSample2).Input);
        }

        [Fact]
        public void StarterGenericVersionNonDataVersion()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            Assert.IsType<StubShell>(bootStrapper.CurrentShell);
            Assert.IsType<CoreDataSample1>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample1>(bootStrapper.CurrentView);
        }

        [Fact]
        public void StarterGenericVersionDataVersion()
        {
            var expected = "Hello";
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample2, string>(expected);
            Assert.IsType<StubShell>(bootStrapper.CurrentShell);
            Assert.IsType<CoreDataSample2>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample2>(bootStrapper.CurrentView);
            Assert.Equal(expected, (bootStrapper.CurrentViewModel as CoreDataSample2).Input);
        }

        [Fact]
        public void StarterNullShell()
        {
            var bootStrapper = new Bootstrapper();
            var ex = Assert.Throws<ArgumentNullException>(() => bootStrapper.Start(null, typeof(CoreDataSample1)));
            Assert.Equal("shell", ex.ParamName);
            Assert.Equal("Value cannot be null.\r\nParameter name: shell", ex.Message);
        }

        [Fact]
        public void StarterNullContent()
        {
            var bootStrapper = new Bootstrapper();
            var ex = Assert.Throws<ArgumentNullException>(() => bootStrapper.Start(typeof(StubShell), null));
            Assert.Equal("content", ex.ParamName);
            Assert.Equal("Value cannot be null.\r\nParameter name: content", ex.Message);
        }

        [Fact]
        public void ChangeViewNullContent()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            var ex = Assert.Throws<ArgumentNullException>(() => bootStrapper.ChangeView(null));
            Assert.Equal("newContent", ex.ParamName);
            Assert.Equal("Value cannot be null.\r\nParameter name: newContent", ex.Message);
        }

        [Fact]
        public void ChangeViewGenericNoData()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            bootStrapper.ChangeView<CoreDataSample3>();
            Assert.IsType<CoreDataSample3>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample3>(bootStrapper.CurrentView);
        }

        [Fact]
        public void ChangeViewGenericData()
        {
            var expected = "Hello";
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            bootStrapper.ChangeView<CoreDataSample2, string>(expected);
            Assert.IsType<CoreDataSample2>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample2>(bootStrapper.CurrentView);
            Assert.Equal(expected, (bootStrapper.CurrentViewModel as CoreDataSample2).Input);
        }

        [Fact]
        public void ChangeViewNonGenericNoData()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            bootStrapper.ChangeView(typeof(CoreDataSample3));
            Assert.IsType<CoreDataSample3>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample3>(bootStrapper.CurrentView);
        }

        [Fact]
        public void ChangeViewNonGenericData()
        {
            var expected = "Hello";
            var bootStrapper = new Bootstrapper();
            bootStrapper.Start<StubShell, CoreDataSample1>();
            bootStrapper.ChangeView<string>(typeof(CoreDataSample2), expected);
            Assert.IsType<CoreDataSample2>(bootStrapper.CurrentViewModel);
            Assert.IsType<Views.CoreDataSample2>(bootStrapper.CurrentView);
            Assert.Equal(expected, (bootStrapper.CurrentViewModel as CoreDataSample2).Input);
        }

        [Fact]
        public void AddGlobalCommand()
        {
            var bootStrapper = new Bootstrapper();
            bootStrapper.GlobalCommands.Add("MintyFresh", () => { });
            bootStrapper.Start<StubShell, CoreDataSample1>();
            Assert.Equal(1, bootStrapper.GlobalCommands.Count);
        }

        [Fact]
        public void AddGlobalCommandNullCommandId()
        {
            var bootStrapper = new Bootstrapper();
            var ex = Assert.Throws<ArgumentNullException>(() => bootStrapper.GlobalCommands.Add(null, () => { }));
            Assert.Equal("commandId", ex.ParamName);
            Assert.Equal("Value cannot be null.\r\nParameter name: commandId", ex.Message);
        }

        [Fact]
        public void AddGlobalCommandNullAction()
        {
            var bootStrapper = new Bootstrapper();
            var ex = Assert.Throws<ArgumentNullException>(() => bootStrapper.GlobalCommands.Add("horse", null));
            Assert.Equal("action", ex.ParamName);
            Assert.Equal("Value cannot be null.\r\nParameter name: action", ex.Message);
        }       
    }
}
