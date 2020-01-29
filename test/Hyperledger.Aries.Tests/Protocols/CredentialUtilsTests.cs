using System.Collections.Generic;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Hyperledger.Aries.Extensions;

namespace Hyperledger.Aries.Tests.Protocols
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

            var ex = Assert.Throws<AriesFrameworkException>(() => CredentialUtils.ValidateCredentialPreviewAttribute(attributeValue));
            Assert.True(ex.ErrorCode == ErrorCode.InvalidParameterFormat);
        }

        [Fact]
        public void CanDetectNoMimeTypeCredentialAttribute()
        {
            var attributeValue = new CredentialPreviewAttribute("first_name", "Test");

            attributeValue.MimeType = null;

            CredentialUtils.ValidateCredentialPreviewAttribute(attributeValue);
            Assert.True(true);
        }

        [Fact]
        public void CanFormatStringCredentialValues()
        {
            var attributeValues = new List<CredentialPreviewAttribute>
            {
                new CredentialPreviewAttribute("first_name","Test"),
                new CredentialPreviewAttribute("last_name","holder")
            };

            var expectedResult = new
            {
                first_name = new
                {
                    raw = "Test",
                    encoded = CredentialUtils.GetEncoded("Test")
                },
                last_name = new
                {
                    raw = "holder",
                    encoded = CredentialUtils.GetEncoded("holder")
                }
            }.ToJson();

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

        [Fact(DisplayName = "Encode random string values into big integer representation")]
        public void EncodeRawValue()
        {
            var value = "SLC";
            var expected = "101327353979588246869873249766058188995681113722618593621043638294296500696424";

            var actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            value = "101 Wilson Lane";
            expected = "68086943237164982734333428280784300550565381723532936263016368251445461241953";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            // null value
            value = null;
            expected = "102987336249554097029535212322581322789799900648198034993379397001115665086549";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            // empty string value
            value = string.Empty;
            expected = "102987336249554097029535212322581322789799900648198034993379397001115665086549";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Encode random integer values into big integer representation")]
        public void EncodeIntegerValues()
        {
            // int max
            var value = "2147483647";
            var expected = "2147483647";
            var actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            // int min
            value = "-2147483648";
            expected = "-2147483648";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            // int max + 1
            value = "2147483648";
            expected = "26221484005389514539852548961319751347124425277437769688639924217837557266135";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);

            // int min - 1
            value = "-2147483649";
            expected = "68956915425095939579909400566452872085353864667122112803508671228696852865689";
            actual = CredentialUtils.GetEncoded(value);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ValidateEncoding()
        {
            var raw = "SLC";
            var encoded = "101327353979588246869873249766058188995681113722618593621043638294296500696424";
            var valid = CredentialUtils.CheckValidEncoding(raw, encoded);
            Assert.True(valid);

            raw = "SLC";
            encoded = "invalid";
            valid = CredentialUtils.CheckValidEncoding(raw, encoded);
            Assert.False(valid);

            raw = "2147483648";
            encoded = "26221484005389514539852548961319751347124425277437769688639924217837557266135";
            valid = CredentialUtils.CheckValidEncoding(raw, encoded);
            Assert.True(valid);
        }
    }
}
