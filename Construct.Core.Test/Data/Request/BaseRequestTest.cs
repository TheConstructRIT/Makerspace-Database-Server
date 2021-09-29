using Construct.Core.Data.Attribute;
using Construct.Core.Data.Request;
using NUnit.Framework;

namespace Construct.Core.Test.Data.Request
{
    public class BaseRequestTestRequest : BaseRequest
    {
        public string TestProperty1 { get; set; }
        
        [NotEmpty("property-2-missing")]
        public string TestProperty2 { get; set; }
        
        [NotEmpty("property-3-missing")]
        public string TestProperty3 { get; set; }
    }
    
    public class BaseRequestTest
    {
        /// <summary>
        /// Request to test with.
        /// </summary>
        private BaseRequestTestRequest _request;

        /// <summary>
        /// Sets up the test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this._request = new BaseRequestTestRequest();
        }
        
        /// <summary>
        /// Tests with no fields invalid.
        /// </summary>
        [Test]
        public void TestNoErrors()
        {
            this._request.TestProperty2 = "Value";
            this._request.TestProperty3 = "Value";
            Assert.Null(this._request.GetValidationErrorResponse());
        }

        /// <summary>
        /// Tests the first field being null.
        /// </summary>
        [Test]
        public void TestFirstFieldNull()
        {
            this._request.TestProperty2 = null;
            this._request.TestProperty3 = "Value";
            Assert.AreEqual("property-2-missing", this._request.GetValidationErrorResponse().Status);
        }

        /// <summary>
        /// Tests the first field being empty.
        /// </summary>
        [Test]
        public void TestFirstFieldEmpty()
        {
            this._request.TestProperty2 = "";
            this._request.TestProperty3 = "Value";
            Assert.AreEqual("property-2-missing", this._request.GetValidationErrorResponse().Status);
        }

        /// <summary>
        /// Tests the second field being null.
        /// </summary>
        [Test]
        public void TestSecondFieldNull()
        {
            this._request.TestProperty2 = "Value";
            this._request.TestProperty3 = null;
            Assert.AreEqual("property-3-missing", this._request.GetValidationErrorResponse().Status);
        }

        /// <summary>
        /// Tests the second field being empty.
        /// </summary>
        [Test]
        public void TestSecondFieldEmpty()
        {
            this._request.TestProperty2 = "Value";
            this._request.TestProperty3 = "";
            Assert.AreEqual("property-3-missing", this._request.GetValidationErrorResponse().Status);
        }
    }
}