using MockDoor.Data.Models;
using MockDoor.Shared.Helper;
using MockDoor.Shared.Models.Enum;
using MockDoor.Shared.Models.Response;
using System.Security.Cryptography;

namespace MockDoor.Data.Helpers
{
    public static class ChecksumHelpers
    {
        public static string CreateChecksum(SupportedEncodingType encoding, string valueToChecksum)
        {
            using var md5 = MD5.Create();

            byte[] valueInBytes = ConvertHelper.ToByteArray(encoding, valueToChecksum);
            byte[] md5ChecksumBytes = md5.ComputeHash(valueInBytes);

            return BitConverter.ToString(md5ChecksumBytes).Replace("-", string.Empty).ToLower();
        }

        public static string CreateDefaultChecksum(MockResponse mockResponse)
        {
            using var md5 = MD5.Create();

            byte[] valueInBytes = ConvertHelper.ToByteArray(mockResponse.Encoding, $"{mockResponse.Body}-{mockResponse.Code}-{mockResponse.ContentType}");
            byte[] md5ChecksumBytes = md5.ComputeHash(valueInBytes);

            return BitConverter.ToString(md5ChecksumBytes).Replace("-", string.Empty).ToLower();
        }

        public static string CreateDefaultChecksum(UpdateMockResponseDto mockResponse)
        {
            using var md5 = MD5.Create();

            byte[] valueInBytes = ConvertHelper.ToByteArray(mockResponse.Encoding, $"{mockResponse.Body}-{mockResponse.Code}-{mockResponse.ContentType}");
            byte[] md5ChecksumBytes = md5.ComputeHash(valueInBytes);

            return BitConverter.ToString(md5ChecksumBytes).Replace("-", string.Empty).ToLower();
        }

        public static string CreateDefaultChecksum(MockResponseDto mockResponse)
        {
            using var md5 = MD5.Create();

            byte[] valueInBytes = ConvertHelper.ToByteArray(mockResponse.Encoding, $"{mockResponse.Body}-{mockResponse.Code}-{mockResponse.ContentType}");
            byte[] md5ChecksumBytes = md5.ComputeHash(valueInBytes);

            return BitConverter.ToString(md5ChecksumBytes).Replace("-", string.Empty).ToLower();
        }

    }
}
