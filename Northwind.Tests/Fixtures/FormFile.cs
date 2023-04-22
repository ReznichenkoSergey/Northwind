using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Tests.Fixtures
{
    internal class FormFile : IFormFile
    {
        public string ContentDisposition { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public IHeaderDictionary Headers { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
            => new MemoryStream(new byte[1]);
    }
}
