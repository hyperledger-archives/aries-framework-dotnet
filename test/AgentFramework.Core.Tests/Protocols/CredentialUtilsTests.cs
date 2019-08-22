using System.Collections.Generic;
using AgentFramework.Core.Exceptions;
using AgentFramework.Core.Models.Credentials;
using AgentFramework.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AgentFramework.Core.Tests.Protocols
{
    public class CredentialUtilsTests
    {
        [Fact]
        public void CanCastStringAttribute()
        {
            object attributeValue = "tester";

            var output = CredentialUtils.CastAttribute(attributeValue, CredentialMimeTypes.TextMimeType);

            Assert.IsType<string>(output);
        }

        [Fact]
        public void CanSerializeDeserializeCredentialPreviewAttribute()
        {
            var credentialAttributePreview = new CredentialPreviewAttribute("test-attr", "testing");

            var jsonPayload = JsonConvert.SerializeObject(credentialAttributePreview);

            //TODO assert

            var output = JsonConvert.DeserializeObject<CredentialPreviewAttribute>(jsonPayload);

            Assert.Equal(output.Name, credentialAttributePreview.Name);
            Assert.Equal(output.Value, credentialAttributePreview.Value);
            Assert.Equal(output.MimeType, credentialAttributePreview.MimeType);
        }

        [Fact]
        public void CanValidateCredentialAttribute()
        {
            var attributeValue = new CredentialPreviewAttribute("first_name", "Test");
            
            CredentialUtils.ValidateCredentialPreviewAttribute(attributeValue);
        }

        [Fact]
        public void CanDetectBadMimeTypeCredentialAttribute()
        {
            var attributeValue = new CredentialPreviewAttribute("first_name", "Test");

            attributeValue.MimeType = "bad-mime-type";

            var ex = Assert.Throws<AgentFrameworkException>(() => CredentialUtils.ValidateCredentialPreviewAttribute(attributeValue));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidParameterFormat);
        }

        [Fact]
        public void CanDetectNoMimeTypeCredentialAttribute()
        {
            var attributeValue = new CredentialPreviewAttribute("first_name", "Test");

            attributeValue.MimeType = null;

            var ex = Assert.Throws<AgentFrameworkException>(() => CredentialUtils.ValidateCredentialPreviewAttribute(attributeValue));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidParameterFormat);
        }

        [Fact]
        public void CanFormatStringCredentialValues()
        {
            var attributeValues = new List<CredentialPreviewAttribute>
            {
                new CredentialPreviewAttribute("first_name","Test"),
                new CredentialPreviewAttribute("last_name","holder")
            };

            var expectedResult =
                "{\n  \"first_name\" : {\n    \"raw\" : \"Test\",\n    \"encoded\" : \"1234567890\"\n  },\n  \"last_name\" : {\n    \"raw\" : \"holder\",\n    \"encoded\" : \"1234567890\"\n  }\n}";

            var formatedCredentialUtils = CredentialUtils.FormatCredentialValues(attributeValues);

            var expectedResultObj = JObject.Parse(expectedResult);
            var formatedCredObj = JObject.Parse(formatedCredentialUtils);

            Assert.Equal(expectedResultObj, formatedCredObj);
        }

        [Fact]
        public void CanGetAttributes()
        {
            var expectedResult = new Dictionary<string, string>
            {
                {"first_name", "Test"},
                {"last_name", "holder"}
            };

            var attributesJson =
                "{\n  \"first_name\" : {\n    \"raw\" : \"Test\",\n    \"encoded\" : \"123456789\"\n  },\n  \"last_name\" : {\n    \"raw\" : \"holder\",\n    \"encoded\" : \"123456789\"\n  }\n}";

            var attributeValues = CredentialUtils.GetAttributes(attributesJson);

            Assert.Equal(expectedResult, attributeValues);
        }
    }
}
