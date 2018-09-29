using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Moq;
using Xunit;
using Zipkin.NET.Models;
using Zipkin.NET.Reporters;
using Zipkin.NET.Senders;

namespace Zipkin.NET.Tests.Reporters
{
    public class ReporterTests
    {
        private readonly IFixture _fixture;

        public ReporterTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ReportAsync_Success()
        {
            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.SendSpansAsync(It.IsAny<IEnumerable<Span>>()))
                .Returns(Task.CompletedTask);

            var span = _fixture.Create<Span>();
            var reporter = new ZipkinReporter(mockSender.Object);
            await reporter.ReportAsync(span);

            mockSender.Verify(
                s => s.SendSpansAsync(
                    It.Is<IEnumerable<Span>>(l => l.First() == span)), Times.Once);
        }
    }
}
