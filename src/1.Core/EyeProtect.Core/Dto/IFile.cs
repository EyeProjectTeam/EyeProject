using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EyeProject.Core.Dto
{
    /// <summary>
    /// RemoteService FileInfo
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// 获取文件字节数
        /// </summary>
        long Length { get; }
        
        /// <summary>
        /// 获取文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 获取文件流
        /// </summary>
        Stream OpenReadStream();

        /// <summary>
        /// 拷贝到指定流
        /// </summary>
        void CopyTo(Stream target);

        /// <summary>
        /// 拷贝到指定流
        /// </summary>
        Task CopyToAsync(Stream target, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 文件流
    /// </summary>
    public class StreamFile : IFile
    {
        private readonly Stream _stream;

        /// <inheritdoc cref="StreamFile"/>
        public StreamFile(string fileName, Stream stream)
        {
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public Stream OpenReadStream() => _stream;

        /// <inheritdoc />
        public void CopyTo(Stream target)
            => _stream.CopyTo(target);

        /// <inheritdoc />
        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
            => await _stream.CopyToAsync(target, cancellationToken).ConfigureAwait(false);

        /// <inheritdoc />
        public long Length => _stream.Length;
    }
}
