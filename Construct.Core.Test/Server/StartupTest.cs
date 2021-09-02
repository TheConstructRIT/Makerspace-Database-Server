using Construct.Core.Attribute;
using Construct.Core.Server;
using NUnit.Framework;

namespace Construct.Core.Test.Server
{
    public class SinglePathController
    {
        public void Test0()
        {
            
        }
        
        [Path("/test1")]
        public void Test1()
        {
            
        }
        
        [Path("/test2")]
        public void Test2()
        {
            
        }
    }

    public class MultiplePathController
    {
        [Path("/test3/1")]
        [Path("/test3/2")]
        public void Test3()
        {
            
        }
        
        [Path("/test4/1")]
        [Path("/test4/2")]
        [Path("/test4/3")]
        public void Test4()
        {
            
        }
    }
    
    public class StartupTest
    {
        /// <summary>
        /// Tests the GetRequestHandlerMethods method with types.
        /// </summary>
        [Test]
        public void TestGetRequestHandlerMethodsTypes()
        {
            // Test with single paths.
            var singlePaths = Startup.GetRequestHandlerMethods(typeof(SinglePathController));
            Assert.AreEqual(2, singlePaths.Count);
            Assert.AreEqual(singlePaths[0].Item1, "/test1");
            Assert.AreEqual(singlePaths[0].Item2.Name, "Test1");
            Assert.AreEqual(singlePaths[1].Item1, "/test2");
            Assert.AreEqual(singlePaths[1].Item2.Name, "Test2");
            
            // Test with multiple paths.
            var multiplePaths = Startup.GetRequestHandlerMethods(typeof(MultiplePathController));
            Assert.AreEqual(5, multiplePaths.Count);
            Assert.AreEqual(multiplePaths[0].Item1, "/test3/1");
            Assert.AreEqual(multiplePaths[0].Item2.Name, "Test3");
            Assert.AreEqual(multiplePaths[1].Item1, "/test3/2");
            Assert.AreEqual(multiplePaths[1].Item2.Name, "Test3");
            Assert.AreEqual(multiplePaths[2].Item1, "/test4/1");
            Assert.AreEqual(multiplePaths[2].Item2.Name, "Test4");
            Assert.AreEqual(multiplePaths[3].Item1, "/test4/2");
            Assert.AreEqual(multiplePaths[3].Item2.Name, "Test4");
            Assert.AreEqual(multiplePaths[4].Item1, "/test4/3");
            Assert.AreEqual(multiplePaths[4].Item2.Name, "Test4");
        }
        
        /// <summary>
        /// Tests the GetRequestHandlerMethods method.
        /// </summary>
        [Test]
        public void TestGetRequestHandlerMethods()
        {
            // Test the entire assembly.
            var paths = Startup.GetRequestHandlerMethods();
            Assert.AreEqual(7, paths.Count);
            Assert.AreEqual(paths[0].Item1, "/test1");
            Assert.AreEqual(paths[0].Item2.Name, "Test1");
            Assert.AreEqual(paths[1].Item1, "/test2");
            Assert.AreEqual(paths[1].Item2.Name, "Test2");
            Assert.AreEqual(paths[2].Item1, "/test3/1");
            Assert.AreEqual(paths[2].Item2.Name, "Test3");
            Assert.AreEqual(paths[3].Item1, "/test3/2");
            Assert.AreEqual(paths[3].Item2.Name, "Test3");
            Assert.AreEqual(paths[4].Item1, "/test4/1");
            Assert.AreEqual(paths[4].Item2.Name, "Test4");
            Assert.AreEqual(paths[5].Item1, "/test4/2");
            Assert.AreEqual(paths[5].Item2.Name, "Test4");
            Assert.AreEqual(paths[6].Item1, "/test4/3");
            Assert.AreEqual(paths[6].Item2.Name, "Test4");
        }
    }
}